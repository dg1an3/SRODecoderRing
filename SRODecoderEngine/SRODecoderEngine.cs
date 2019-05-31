using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace SRODecoderEngine
{
    public class SequentialModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("layers")]
        public IList<Layer> Layers { get; set; }
    }

    public class Layer
    {
        [JsonProperty("class_name")]
        public string ClassName { get; set; }

        [JsonProperty("config")]
        public LayerConfiguration Configuration { get; set; }

        public IDictionary<string, double[,]> Variables { get; set; } = new Dictionary<string, double[,]>();
    }
    
    public class LayerConfiguration
    {
        public string Name { get; set; }

        public bool Trainable { get; set; }

        [JsonProperty("batch_input_shape")]
        public int?[] BatchInputShape { get; set; }

        [JsonProperty("dtype")]
        public string DType { get; set; }

        [JsonProperty("units")]
        public int Units { get; set; }

        [JsonProperty("activation")]
        public string Activation { get; set; }

        [JsonProperty("use_bias")]
        public bool UseBias { get; set; }

        [JsonProperty("kernel_initializer")]
        public JObject KernelInitializer { get; set; }

        [JsonProperty("bias_initializer")]
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
                Model = JsonConvert.DeserializeObject<SequentialModel>(json);
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
                            layer.Configuration.Name.CompareTo(layerName) == 0);
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
            var batchSize = Model.Layers[0].Configuration.BatchInputShape[1].Value;
            Trace.Assert(input.GetLength(0) == batchSize);

            var kernelTensor = Model.Layers[0].Variables["kernel"];
            var biasTensor = Model.Layers[0].Variables["bias"];
            var result = new double[batchSize, kernelTensor.GetLength(1)];
            for (int atBatch = 0; atBatch < batchSize; atBatch++)
            {
                Trace.Assert(kernelTensor.GetLength(0) == input.GetLength(1));
                for (int n = 0; n < kernelTensor.GetLength(0); n++)
                {
                    for (int m = 0; m < kernelTensor.GetLength(1); m++)
                    {
                        result[atBatch, n] += input[atBatch, n] * kernelTensor[n, m];
                    }
                }                
            }
            return result;
        }
    }
}
