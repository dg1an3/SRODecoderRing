using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace SRODecoderEngine
{
    public class SequentialModel
    {
        // [JsonProperty("name")]
        public string Name { get; set; }

        // [JsonProperty("layers")]
        public IList<Layer> Layers { get; set; }
    }

    public class Layer
    {
        // [JsonProperty("class_name")]
        public string ClassName { get; set; }

        // [JsonProperty("config")]
        public LayerConfiguration Config { get; set; }

        public IDictionary<string, double[,]> Variables { get; set; } = new Dictionary<string, double[,]>();
    }
    
    public class LayerConfiguration
    {
        public string Name { get; set; }

        public bool Trainable { get; set; }

        // [JsonProperty("batch_input_shape")]
        public int?[] BatchInputShape { get; set; }

        // [JsonProperty("dtype")]
        public string DType { get; set; }

        // [JsonProperty("units")]
        public int Units { get; set; }

        // [JsonProperty("activation")]
        public string Activation { get; set; }

        // [JsonProperty("use_bias")]
        public bool UseBias { get; set; }

        // [JsonProperty("kernel_initializer")]
        public JObject KernelInitializer { get; set; }

        // [JsonProperty("bias_initializer")]
        public JObject BiasInitializer { get; set; }
    }

    public class SRODecoderEngine
    {
        // read in the model and the weights
        public SRODecoderEngine(Stream modelStream, Stream weightsStream)
        {
            using (var modelReader = new StreamReader(modelStream, Encoding.UTF8))
            {
                var json = modelReader.ReadToEnd();
                Model = JsonConvert.DeserializeObject<SequentialModel>(json, 
                    new JsonSerializerSettings()
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy()
                        }
                    });
            }

            if (weightsStream == null)
                return;

            var tensorsDictionary = ReadTensorsCsv(weightsStream, 2, values => values[1]);
            foreach (var kvp in tensorsDictionary)
            {
                var layerName = LayerFromVariableLabel(kvp.Key);
                var currentLayer = 
                    Model.Layers
                        .FirstOrDefault(layer => 
                            layer.Config.Name.CompareTo(layerName) == 0);
                Trace.Assert(currentLayer != null, string.Format("unable to find layer with name {0}", layerName));

                var name = NameFromVariableLabel(kvp.Key);
                currentLayer.Variables.Add(name, kvp.Value);
            }
        }
        public static IDictionary<string, double[,]> ReadTensorsCsv(Stream stream, int precolumns, Func<string[], string> funcKey)
        {
            var dict = new Dictionary<string, double[,]>();
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    var key = funcKey(values);

                    var tensor_height = Convert.ToInt32(values[precolumns]);
                    var tensor_width = Convert.ToInt32(values[precolumns+1]);
                    double[,] tensor = null;
                    if (!dict.TryGetValue(key, out tensor))
                    {
                        tensor = new double[tensor_height, tensor_width];
                        dict.Add(key, tensor);
                    }

                    var tensor_row = Convert.ToInt32(values[precolumns+2]);
                    Trace.Assert(tensor_row < tensor_height);
                    Trace.Assert(values.Length - (precolumns + 3) == tensor_width);
                    for (int n = (precolumns + 3); n < values.Length; n++)
                    {
                        tensor[tensor_row, n - (precolumns + 3)] = Convert.ToDouble(values[n]);
                    }
                }
            }
            return dict;
        }

        public static string LayerFromVariableLabel(string label)
        {
            var variable_desc = label.Split(new char[] { '/', ':' });
            return variable_desc[0];
        }

        public static string NameFromVariableLabel(string label)
        {
            var variable_desc = label.Split(new char[] { '/', ':' });
            return variable_desc[1];
        }

        public SequentialModel Model { get; set; }

        public double[,] Predict(double[,] input)
        {
            var inSize = Model.Layers[0].Config.BatchInputShape[1].Value;
            Trace.Assert(input.GetLength(1) == inSize);

            var kernelTensor0 = Model.Layers[0].Variables["kernel"];
            var biasTensor0 = Model.Layers[0].Variables["bias"];
            Trace.Assert(input.GetLength(1) == kernelTensor0.GetLength(0));

            var batchCount = input.GetLength(0);
            var result0 = new double[batchCount, kernelTensor0.GetLength(1)];
            Trace.Assert(result0.GetLength(1) == biasTensor0.GetLength(1));
            for (int atBatch0 = 0; atBatch0 < batchCount; atBatch0++)
            {
                for (int m = 0; m < biasTensor0.GetLength(1); m++)
                {
                    result0[atBatch0, m] = biasTensor0[0, m];
                }

                for (int n = 0; n < kernelTensor0.GetLength(0); n++)
                {
                    for (int m = 0; m < kernelTensor0.GetLength(1); m++)
                    {
                        result0[atBatch0, m] += input[atBatch0, n] * kernelTensor0[n, m];
                    }
                }
            }

            var kernelTensor1 = Model.Layers[1].Variables["kernel"];
            Trace.Assert(kernelTensor1.GetLength(0) == result0.GetLength(1));

            var biasTensor1 = Model.Layers[1].Variables["bias"];
            var result1 = new double[batchCount, kernelTensor1.GetLength(1)];
            Trace.Assert(result1.GetLength(1) == biasTensor1.GetLength(1));
            for (int atBatch1 = 0; atBatch1 < batchCount; atBatch1++)
            {
                for (int m = 0; m < biasTensor1.GetLength(1); m++)
                {
                    result1[atBatch1, m] = biasTensor1[0, m];
                }

                for (int n = 0; n < kernelTensor1.GetLength(0); n++)
                {
                    for (int m = 0; m < kernelTensor1.GetLength(1); m++)
                    {
                        result1[atBatch1, m] += result0[atBatch1, n] * kernelTensor1[n, m];
                    }
                }
            }

            return result1;
        }
    }
}
