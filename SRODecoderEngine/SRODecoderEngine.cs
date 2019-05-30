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

            using (var reader = new StreamReader(weightsStream, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    var layer_name = values[0];
                    var currentLayer = 
                        Model.Layers.FirstOrDefault(layer => layer.Configuration.Name == values[0]);
                    Trace.Assert(currentLayer != null, string.Format("unable to find layer with name {0}", values[0]));

                    var variable_desc = values[1].Split(new char[] { '/', ':' });
                    Trace.Assert(variable_desc[0].CompareTo(layer_name) == 0);

                    var tensor_height = Convert.ToInt32(values[2]);
                    var tensor_width = Convert.ToInt32(values[3]);

                    double[,] tensor = null;
                    if (!currentLayer.Variables.TryGetValue(variable_desc[1], out tensor))
                    {
                        tensor = new double[tensor_height, tensor_width];
                        currentLayer.Variables.Add(variable_desc[1], tensor);
                    }

                    var tensor_row = Convert.ToInt32(values[4]);
                    Trace.Assert(tensor_row < tensor_height);

                    if (variable_desc[1].CompareTo("kernel") == 0)
                    {
                        if (currentLayer.Configuration.BatchInputShape != null)
                        {
                            Trace.Assert(tensor_height == currentLayer.Configuration.BatchInputShape[1].Value);
                        }
                    }
                    else if (variable_desc[1].CompareTo("bias") == 0)
                    {
                        Trace.Assert(tensor_height == 1);
                    }
                    else
                    {
                        Trace.Assert(false, "unrecognized layer variable");
                    }

                    Trace.Assert(tensor_width == currentLayer.Configuration.Units);
                    Trace.Assert(values.Length - 5 == tensor_width);
                    for (int n = 5; n < values.Length; n++)
                    {
                        tensor[tensor_row, n - 5] = Convert.ToDouble(values[n]);
                    }
                }
            }
        }

        public SequentialModel Model { get; set; }
    }
}
