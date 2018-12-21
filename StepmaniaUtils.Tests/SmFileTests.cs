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
        [ExpectedException(typeof(ArgumentException))]
        public void SmFile_Throws_Exception_When_File_Does_Not_Exist()
        {
            var smFile = new SmFile("TestData/wrongFile.sm");
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void SmFile_Correctly_Populates_Title_Tag()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            string value = smFile[SmFileAttribute.TITLE];

            Assert.IsTrue(string.IsNullOrEmpty(value) == false);
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void SmFile_Returns_Empty_String_For_NonExistent_Attribute()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            string value = smFile[SmFileAttribute.TIMESIGNATURES];

            Assert.IsTrue(string.IsNullOrEmpty(value));
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void SmFile_Common_Attributes_Are_Populated()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            Assert.AreEqual("BUTTERFLY", smFile.SongTitle);
            Assert.AreEqual("TestData", smFile.Group); //Group name is the name of the parent folder
            Assert.AreEqual("BUTTERFLY.png", smFile.BannerPath);
            Assert.AreEqual("SMILE.dk", smFile.Artist);
        }
        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void SmFile_GetHighestChartedDifficulty_Returns_The_Highest_Difficulty()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            var songDifficulty = smFile.ChartMetadata.GetHighestChartedDifficulty(PlayStyle.Single);

            Assert.IsTrue(songDifficulty == SongDifficulty.Challenge);
        }

        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void SmFile_ChartMetadata_Populates_All_Stepcharts()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            Assert.IsTrue(smFile.ChartMetadata.StepCharts.Count == 16);
        }
        
        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void SmFile_GenerateLightsChart_Generates_Lights_Data_Content()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            var lightsData = StepChartBuilder.GenerateLightsChart(smFile);

            Assert.IsTrue(!string.IsNullOrEmpty(lightsData.Content));
        }

        [TestMethod]
        [DeploymentItem("TestData/Doubles/ArabianNights.sm", "TestData/Doubles/")]
        public void SmFile_GenerateLightsChart_Generates_Lights_Data_Content_For_Doubles_Only_Chart()
        {
            var smFile = new SmFile("TestData/Doubles/ArabianNights.sm");
            
            var lightsData = StepChartBuilder.GenerateLightsChart(smFile);

            Assert.IsTrue(!string.IsNullOrEmpty(lightsData.Content));
        }
        
        [TestMethod]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        public void SmFile_Saves_Light_Chart_Data_To_File()
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
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.sm", "TestData/DDR1stMix/")]
        [DeploymentItem("TestData/DDR1stMix/BUTTERFLY.png", "TestData/DDR1stMix/")]
        public void SmFile_BannerPath_Points_To_Valid_File()
        {
            var smFile = new SmFile("TestData/DDR1stMix/BUTTERFLY.sm");
            var fullPath = Path.Combine(smFile.Directory, smFile.BannerPath);
            Assert.IsTrue(File.Exists(fullPath), $"Banner path could not be found: {fullPath}");
        }

        [TestMethod]
        [DeploymentItem("TestData/Chris/gargoyle.sm", "TestData/Chris/")]
        [DeploymentItem("TestData/bn.png", "TestData/")]
        public void SmFile_With_Missing_Semicolon_Still_Reads_Proper_Banner_Path()
        {
            var smFile = new SmFile("TestData/Chris/gargoyle.sm");

            var fullPath = Path.Combine(smFile.Directory, smFile.BannerPath);
            
            Assert.IsTrue(File.Exists(fullPath), $"Banner path could not be found: {fullPath}");
        }

        [TestMethod]
        [DeploymentItem("TestData/Doubles/ArabianNights.sm", "TestData/Doubles")]
        public void SmFile_With_DisplayBpm_Tag_Shows_Correct_Bpm()
        {
            var smFile = new SmFile("TestData/Doubles/ArabianNights.sm");

            Assert.AreEqual("138", smFile.DisplayBpm);
        }

        [TestMethod]
        [DeploymentItem("TestData/Chris/gargoyle.sm", "TestData/Chris")]
        public void SmFile_With_No_DisplayBpm_Tag_Still_Shows_Correct_Bpm_Range()
        {
            var smFile = new SmFile("TestData/Chris/gargoyle.sm");

            Assert.AreEqual("75-150", smFile.DisplayBpm);
        }

        [TestMethod]
        [DeploymentItem("TestData/Misc/raffles.sm", "TestData/Misc")]
        public void SmFile_With_No_DisplayBpm_Tag_With_NewLines_In_Bpm_Tag_Shows_Correct_Bpm_Range()
        {
            var smFile = new SmFile(@"TestData/Misc/raffles.sm");

            Assert.AreEqual("105-210", smFile.DisplayBpm);
        }
    }
}
