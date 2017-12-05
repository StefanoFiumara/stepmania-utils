using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StepmaniaUtils.Core;
using StepmaniaUtils.Enums;

namespace StepmaniaUtils.Tests
{
    [TestClass]
    public class SmFileTests
    { 
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NonExistentFile()
        {
            var smFile = new SmFile("TestData/wrongFile.sm");
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GetTitleAttribute()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            string value = smFile.GetAttribute(SmFileAttribute.TITLE);

            Assert.IsTrue(string.IsNullOrEmpty(value) == false);
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GetNonExistentAttribute()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            string value = smFile.GetAttribute(SmFileAttribute.TIMESIGNATURES);

            Assert.IsTrue(string.IsNullOrEmpty(value));
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GetCommonAttributes()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            Assert.AreEqual("BUTTERFLY", smFile.SongName);
            Assert.AreEqual("TestData", smFile.Group); //Group name is the name of the parent folder
            Assert.AreEqual("BUTTERFLY.png", smFile.BannerPath);
            Assert.AreEqual("SMILE.dk", smFile.Artist);
        }
        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GetHighestDifficulty()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            using (var data = smFile.ExtractChartData())
            {
                var songDifficulty = data.GetHighestChartedDifficulty(PlayStyle.Single);

                Assert.IsTrue(songDifficulty == SongDifficulty.Challenge);
            }
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GetChartData()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            using (var data = smFile.ExtractChartData())
            {
                var stepData = data.GetSteps(PlayStyle.Single, SongDifficulty.Challenge);
                Assert.IsTrue(stepData.Measures.All(m => m.Notes.Count % 4 == 0));
            }
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GenerateLightsChart()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            using (var data = smFile.ExtractChartData())
            {
                var reference = data.GetSteps(PlayStyle.Single, SongDifficulty.Challenge);
                var lightsData = StepChartBuilder.GenerateLightsChart(reference);

                Assert.IsTrue(lightsData != null);
            }
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GenerateLightsChart_Doubles()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            using (var data = smFile.ExtractChartData())
            {
                var reference = data.GetSteps(PlayStyle.Double, SongDifficulty.Challenge);
                var lightsData = StepChartBuilder.GenerateLightsChart(reference);

                Assert.IsTrue(lightsData != null);
            }
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GenerateLightsChart_NoReference()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            using (var data = smFile.ExtractChartData())
            {
                var reference = data.GetSteps(PlayStyle.Undefined, SongDifficulty.Challenge);
                StepChartBuilder.GenerateLightsChart(reference);
            }
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void SaveChartData()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");

            bool hasLightsDataBeforeSave;
            bool hasLightsDataAfterSave;

            using (var data = smFile.ExtractChartData())
            {
                hasLightsDataBeforeSave = data.GetSteps(PlayStyle.Lights, SongDifficulty.Easy) != null;

                var reference = data.GetSteps(PlayStyle.Single, SongDifficulty.Hard);
                var lightsData = StepChartBuilder.GenerateLightsChart(reference);

                data.AddNewStepchart(lightsData);
            }

            var newSmFileReference = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");

            using (var data = newSmFileReference.ExtractChartData())
            {
                hasLightsDataAfterSave = data.GetSteps(PlayStyle.Lights, SongDifficulty.Easy) != null;
            }

            Assert.IsFalse(hasLightsDataBeforeSave);
            Assert.IsTrue(hasLightsDataAfterSave);
        }
    }
}
