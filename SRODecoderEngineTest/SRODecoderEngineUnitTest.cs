using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SRODecoderEngine;
using System.IO;
using System.Reflection;
using System.Text;

namespace SRODecoderEngineTest
{
    [TestClass]
    public class SRODecoderEngineUnitTest
    {
        [TestMethod]
        public void TestDeserializeModel()
        {
            var stream = GetType().Assembly.GetManifestResourceStream("SRODecoderEngineTest.test_model_config.json");
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var json = reader.ReadToEnd();
                var sequentialModel = JsonConvert.DeserializeObject<SequentialModel>(json);
                Assert.IsTrue(sequentialModel.Layers.Count == 1);
            }
        }

        [TestMethod]
        public void TestLoadWeights()
        {
            var stream = GetType().Assembly.GetManifestResourceStream("SRODecoderEngineTest.test_model_weights.csv");
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                }
            }
        }

        [TestMethod]
        public void TestActivationRelu()
        {

        }

        [TestMethod]
        public void TestActivationSoftmax()
        {

        }

        [TestMethod]
        public void TestDenseLayer()
        {

        }
    }
}
