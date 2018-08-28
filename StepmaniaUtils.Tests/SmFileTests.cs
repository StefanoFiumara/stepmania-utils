using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StepmaniaUtils.Core;
using StepmaniaUtils.Enums;
using StepmaniaUtils.StepGenerator;

namespace StepmaniaUtils.Tests
{
    [TestClass]
    public class SmFileTests
    {
        [TestMethod]
        public void Canary()
        {
            Assert.IsTrue(true);
        }

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
        
        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void GenerateLightsChart()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            var lightsData = StepChartBuilder.GenerateLightsChart(smFile);

            Assert.IsTrue(!string.IsNullOrEmpty(lightsData.Content));
        }

        [TestMethod]
        [DeploymentItem("TestData/Doubles/ArabianNights.sm", "TestData/Doubles/")]
        public void GenerateLightsChart_Doubles()
        {
            var smFile = new SmFile("TestData/Doubles/ArabianNights.sm");
            
            var lightsData = StepChartBuilder.GenerateLightsChart(smFile);

            Assert.IsTrue(!string.IsNullOrEmpty(lightsData.Content));
        }
        
        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void SaveChartData()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");

            bool hasLightsBeforeSave = smFile.ChartMetadata.GetSteps(PlayStyle.Lights, SongDifficulty.Easy) != null;

            var chart = StepChartBuilder.GenerateLightsChart(smFile);

            smFile.AddLightsChart(chart);
            smFile.Refresh();

            bool hasLightsAfterSave = smFile.ChartMetadata.GetSteps(PlayStyle.Lights, SongDifficulty.Easy) != null;
            
            Assert.IsFalse(hasLightsBeforeSave);
            Assert.IsTrue(hasLightsAfterSave, "SM file did not have lights chart after save");
        }

        [TestMethod]
        [DeploymentItem("TestData/Cupcake/BadTag.sm", "TestData/Cupcake/")]
        public void TestInvalidTag()
        {
            var smFile = new SmFile("TestData/Cupcake/BadTag.sm");
            Assert.AreEqual("Watch Out! Swing Up!", smFile.SongTitle);
            Assert.AreEqual("TestData", smFile.Group); //Group name is the name of the parent folder
            Assert.AreEqual("../Cupcake Timing Festival v2-bn.png", smFile.BannerPath);
            Assert.AreEqual("FAKE TYPE.", smFile.Artist);
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.png", "TestData/DDR1stMix/")]
        public void TestValidBannerPath()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            var fullPath = Path.Combine(smFile.Directory, smFile.BannerPath);
            Assert.IsTrue(File.Exists(fullPath), $"Banner path could not be found: {fullPath}");
        }

        [TestMethod]
        [DeploymentItem("TestData/Chris/gargoyle.sm", "TestData/Chris/")]
        [DeploymentItem("TestData/bn.png", "TestData/")]
        public void TestMissingSemicolon()
        {
            var smFile = new SmFile("TestData/Chris/gargoyle.sm");

            var fullPath = Path.Combine(smFile.Directory, smFile.BannerPath);
            //Banner tag is missing a semicolon, attempt to delimit the path by newline to get proper banner
            Assert.IsTrue(File.Exists(fullPath), $"Banner path could not be found: {fullPath}");
        }

        [TestMethod]
        [DeploymentItem("TestData/Doubles/ArabianNights.sm", "TestData/Doubles")]
        public void TestDisplayBpm()
        {
            var smFile = new SmFile("TestData/Doubles/ArabianNights.sm");

            Assert.AreEqual("138", smFile.DisplayBpm);
        }

        [TestMethod]
        [DeploymentItem("TestData/Chris/gargoyle.sm", "TestData/Chris")]
        public void TestDisplayBpmWithoutTag()
        {
            var smFile = new SmFile("TestData/Chris/gargoyle.sm");

            Assert.AreEqual("75-150", smFile.DisplayBpm);
        }
    }
}
