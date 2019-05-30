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
            var json = SRODecoderEngineTest.Properties.Resources.ModelTestJson;
            var sequentialModel = JsonConvert.DeserializeObject<SequentialModel>(json);
            Assert.IsTrue(sequentialModel.Layers.Count == 4);
        }

        [TestMethod]
        public void TestLoadWeights()
        {

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
