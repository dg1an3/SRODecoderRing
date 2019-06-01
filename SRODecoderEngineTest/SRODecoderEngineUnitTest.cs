using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SRODecoderEngine;
using System;
using System.IO;
using System.Linq;
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
            var stream = GetType().Assembly.GetManifestResourceStream("SRODecoderEngineTest.config.json");
            var engine = new SRODecoderEngine.SRODecoderEngine(stream, null);

            Assert.IsTrue(engine.Model.Layers.Count == 2);

            // test that they are dense layers
            Assert.IsTrue(engine.Model.Layers
                .All(layer => layer.ClassName == "Dense"));

            // test that first has batch input shape
            Assert.IsTrue(engine.Model.Layers
                .First().Config.BatchInputShape != null);
            Assert.IsTrue(engine.Model.Layers
                .Skip(1).All(layer => layer.Config.BatchInputShape == null));

            // test that they have different names
            Assert.IsTrue(engine.Model.Layers
                .Select(layer => layer.Config.Name)
                .Distinct().Count() 
                    == engine.Model.Layers.Count());

            // test if they are trainable
            Assert.IsTrue(engine.Model.Layers.All(layer => layer.Config.Trainable));

            // test for activation values
            Assert.IsTrue(engine.Model.Layers.All(layer => layer.Config.Activation != null));
            Assert.IsTrue(engine.Model.Layers.All(layer => layer.Config.Activation.CompareTo("linear") == 0));
        }

        [TestMethod]
        public void TestLoadWeights()
        {
            var modelStream = GetType().Assembly.GetManifestResourceStream("SRODecoderEngineTest.config.json");
            var weightStream = GetType().Assembly.GetManifestResourceStream("SRODecoderEngineTest.weights_posttrain.csv");
            var engine = new SRODecoderEngine.SRODecoderEngine(modelStream, weightStream);

            // check that each layer has same output as previous input
            for (int nLayer = 0; nLayer < engine.Model.Layers.Count; nLayer++)
            {
                Layer currentLayer = engine.Model.Layers[nLayer];
                double[,] kernelTensor = currentLayer.Variables["kernel"];
                if (currentLayer.Config.UseBias)
                {
                    Assert.IsTrue(currentLayer.Variables.ContainsKey("bias"));
                    double[,] biasTensor = currentLayer.Variables["bias"];
                    Assert.AreEqual(biasTensor.GetLength(0), 1);
                    Assert.AreEqual(biasTensor.GetLength(1), kernelTensor.GetLength(1));
                }
                else
                {
                    Assert.IsFalse(currentLayer.Variables.ContainsKey("bias"));
                }

                Assert.AreEqual(currentLayer.Config.Units,
                    kernelTensor.GetLength(1));

                if (nLayer > 0)
                {
                    Assert.IsTrue(currentLayer.Config.BatchInputShape == null);
                    Layer previousLayer = engine.Model.Layers[nLayer - 1];
                    double[,] previousKernelTensor = previousLayer.Variables["kernel"];
                    Assert.AreEqual(previousKernelTensor.GetLength(1), 
                        kernelTensor.GetLength(0));
                }                
                else
                {
                    Assert.IsTrue(currentLayer.Config.BatchInputShape != null);
                    Assert.AreEqual(currentLayer.Config.BatchInputShape[1].Value,
                        kernelTensor.GetLength(0));
                }
            }
        }

        [TestMethod]
        public void TestDenseLayer()
        {
            var modelStream = GetType().Assembly.GetManifestResourceStream("SRODecoderEngineTest.config.json");
            var weightStream = GetType().Assembly.GetManifestResourceStream("SRODecoderEngineTest.weights_posttrain.csv");
            var engine = new SRODecoderEngine.SRODecoderEngine(modelStream, weightStream);

            var predictStream = GetType().Assembly.GetManifestResourceStream("SRODecoderEngineTest.predict_posttrain.csv");
            var predictions = SRODecoderEngine.SRODecoderEngine.ReadTensorsCsv(predictStream, 1, values => values[0]);
            var predictionInTensor = predictions["in"];
            var predictionOutTensor = predictions["out"];

            var actualOutput = engine.Predict(predictionInTensor);

            Assert.AreEqual(actualOutput.GetLength(0), predictionOutTensor.GetLength(0));
            Assert.AreEqual(actualOutput.GetLength(1), predictionOutTensor.GetLength(1));
            for (int n = 0; n < actualOutput.GetLength(0); n++)
            {
                for (int m = 0; m < actualOutput.GetLength(1); m++)
                {
                    Assert.IsTrue(Math.Abs(actualOutput[n,m] - predictionOutTensor[n,m]) < 1e-3);
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
    }
}
