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
            string value = smFile[SmFileAttribute.TITLE];

            Assert.IsTrue(string.IsNullOrEmpty(value) == false);
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GetNonExistentAttribute()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            string value = smFile[SmFileAttribute.TIMESIGNATURES];

            Assert.IsTrue(string.IsNullOrEmpty(value));
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GetCommonAttributes()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            Assert.AreEqual("BUTTERFLY", smFile.SongTitle);
            Assert.AreEqual("TestData", smFile.Group); //Group name is the name of the parent folder
            Assert.AreEqual("BUTTERFLY.png", smFile.BannerPath);
            Assert.AreEqual("SMILE.dk", smFile.Artist);
        }
        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GetHighestDifficulty()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            var songDifficulty = smFile.ChartMetadata.GetHighestChartedDifficulty(PlayStyle.Single);

            Assert.IsTrue(songDifficulty == SongDifficulty.Challenge);
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GetChartData()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            Assert.IsTrue(smFile.ChartMetadata.StepCharts.Count > 0);
        }

        //TODO: Tests to verify correct stepchart metadata

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GenerateLightsChart()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            var lightsData = StepChartBuilder.GenerateLightsChart(smFile);

            Assert.IsTrue(lightsData);
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GenerateLightsChart_Doubles()
        {
            //TODO: introduce a different stepchart with only doubles charts for this test
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            
            var lightsData = StepChartBuilder.GenerateLightsChart(smFile);

            Assert.IsTrue(lightsData);
        }
        
        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void SaveChartData()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");

            bool hasLightsBeforeSave = smFile.ChartMetadata.GetSteps(PlayStyle.Lights, SongDifficulty.Easy) != null;
            var success = StepChartBuilder.GenerateLightsChart(smFile);

            var newSmFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");

            bool hasLightsAfterSave = newSmFile.ChartMetadata.GetSteps(PlayStyle.Lights, SongDifficulty.Easy) != null;

            Assert.IsTrue(success);
            Assert.IsFalse(hasLightsBeforeSave);
            Assert.IsTrue(hasLightsAfterSave);
        }
    }
}
