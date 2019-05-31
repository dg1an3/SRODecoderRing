﻿using System;
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

            Func<string[], string> computeModelKey = values => values[1];

            var tensorsDictionary = ReadTensorsCsv(weightsStream, 2, computeModelKey);
            foreach (var kvp in tensorsDictionary)
            {
                var currentLayer = 
                    Model.Layers
                        .FirstOrDefault(layer => 
                            layer.Configuration.Name.CompareTo(LayerFromVariableLabel(kvp.Key)) == 0);
                Trace.Assert(currentLayer != null, string.Format("unable to find layer with name {0}", kvp.Key[0]));

                var name = NameFromVariableLabel(kvp.Key);
                currentLayer.Variables.Add(name, kvp.Value);
            }
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

        public SequentialModel Model { get; set; }
    }
}
