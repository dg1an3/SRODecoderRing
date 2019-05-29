using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public IDictionary<string, object> Variables { get; set; }
    }
    
    public class LayerConfiguration
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("trainable")]
        public bool Trainable { get; set; }

        [JsonProperty("batch_input_shape")]
        public int[] BatchInputShape { get; set; }

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
        public SRODecoderEngine(string json)
        {
            var sequentialModel = JsonConvert.DeserializeObject<SequentialModel>(json);
        }
    }
}
