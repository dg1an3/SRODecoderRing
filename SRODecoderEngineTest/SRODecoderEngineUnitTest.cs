using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SRODecoderEngine;

namespace SRODecoderEngineTest
{
    [TestClass]
    public class SRODecoderEngineUnitTest
    {
        [TestMethod]
        public void TestDeserializeModel()
        {
            var json = 
                @"{ ""name"": ""sequential_1"",
                    ""layers"": [ 
                        { ""class_name"": ""Dense"", 
                            ""config"": {
                            ""name"": ""dense_1"", ""trainable"": true,
                            ""batch_input_shape"": [ null, 12 ],
                            ""dtype"": ""float32"",
                            ""units"": 64,
                            ""activation"": ""relu"",
                            ""use_bias"": true } } ] }";

            var sequentialModel = JsonConvert.DeserializeObject<SequentialModel>(json);
        }
    }
}
