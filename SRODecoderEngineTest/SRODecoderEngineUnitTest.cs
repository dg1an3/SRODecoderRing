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
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var json = reader.ReadToEnd();
                var sequentialModel = JsonConvert.DeserializeObject<SequentialModel>(json);
                Assert.IsTrue(sequentialModel.Layers.Count == 2);

                // test that they are dense layers
                Assert.IsTrue(sequentialModel.Layers
                    .All(layer => layer.ClassName == "Dense"));

                // test that first has batch input shape
                Assert.IsTrue(sequentialModel.Layers
                    .First().Configuration.BatchInputShape != null);
                Assert.IsTrue(sequentialModel.Layers
                    .Skip(1).All(layer => layer.Configuration.BatchInputShape == null));

                // test that they have different names
                Assert.IsTrue(sequentialModel.Layers
                    .Select(layer => layer.Configuration.Name)
                    .Distinct().Count() 
                        == sequentialModel.Layers.Count());

                // test if they are trainable
                Assert.IsTrue(sequentialModel.Layers.All(layer => layer.Configuration.Trainable));

                // test for activation values
                Assert.IsTrue(sequentialModel.Layers.All(layer => layer.Configuration.Activation != null));
            }
        }

        [TestMethod]
        public void TestLoadWeights()
        {
            SequentialModel sequentialModel = null;
            var modelStream = GetType().Assembly.GetManifestResourceStream("SRODecoderEngineTest.config.json");
            using (var modelReader = new StreamReader(modelStream, Encoding.UTF8))
            {
                var json = modelReader.ReadToEnd();
                sequentialModel = JsonConvert.DeserializeObject<SequentialModel>(json);
            }

            var stream = GetType().Assembly.GetManifestResourceStream("SRODecoderEngineTest.weights_posttrain.csv");
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    var layer_name = values[0];
                    var currentLayer = sequentialModel.Layers.First(layer => layer.Configuration.Name == values[0]);                    
                    var variable_desc = values[1].Split(new char[] { '/', ':' });
                    Assert.IsTrue(variable_desc[0].CompareTo(layer_name) == 0);

                    var tensor_height = Convert.ToInt32(values[2]);
                    var tensor_width = Convert.ToInt32(values[3]);

                    double[,] tensor = null;
                    if (!currentLayer.Variables.TryGetValue(variable_desc[1], out tensor))
                    {
                        tensor = new double[tensor_height, tensor_width];
                        currentLayer.Variables.Add(variable_desc[1], tensor);
                    }

                    var tensor_row = Convert.ToInt32(values[4]);
                    Assert.IsTrue(tensor_row < tensor_height);

                    if (variable_desc[1].CompareTo("kernel") == 0)
                    {
                        if (currentLayer.Configuration.BatchInputShape != null)
                        {
                            Assert.IsTrue(tensor_height == currentLayer.Configuration.BatchInputShape[1].Value);
                        }
                    }
                    else if (variable_desc[1].CompareTo("bias") == 0)
                    {
                        Assert.IsTrue(tensor_height == 1);
                    }
                    else
                    {
                        Assert.Fail("unrecognized layer variable");
                    }
                    Assert.IsTrue(tensor_width == currentLayer.Configuration.Units);
                    Assert.IsTrue(values.Length - 5 == tensor_width);
                    for (int n = 5; n < values.Length; n++)
                    {
                        tensor[tensor_row, n - 5] = Convert.ToDouble(values[n]);
                    }
                }
            }
        }

        [TestMethod]
        public void TestDenseLayer()
        {
            var engine = new SRODecoderEngine.SRODecoderEngine("");
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
