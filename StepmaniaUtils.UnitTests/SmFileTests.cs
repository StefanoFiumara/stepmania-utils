using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StepmaniaUtils.Core;
using StepmaniaUtils.Enums;

namespace StepmaniaUtils.UnitTests
{
    [TestClass]
    public class SmFileTests
    {
        private const string TEST_DATA_ROOT = "../../TestData";

        private SmFile SmFile { get; set; }

        [TestInitialize]
        public void Setup()
        {
            this.SmFile = new SmFile(new FileInfo($"{TEST_DATA_ROOT}/DDR1stMix/BUTTERFLY.sm"));
        }

        [TestCleanup]
        public void CleanUp()
        {
            
            var backupFiles = Directory.GetFiles(TEST_DATA_ROOT, "*",SearchOption.AllDirectories).Where(f => f.EndsWith(".backup")).Select(f => new FileInfo(f));

            foreach (var backupFile in backupFiles)
            {
                var correspondingSmFile = new FileInfo(backupFile.FullName.Replace(".backup", string.Empty));

                if (correspondingSmFile.Exists) File.Delete(correspondingSmFile.FullName);

                File.Copy(backupFile.FullName, correspondingSmFile.FullName, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NonExistentFile()
        {
            this.SmFile = new SmFile(new FileInfo("TestData/wrongFile.sm"));
        }

        [TestMethod]
        public void GetTitleAttribute()
        {
            string value = this.SmFile.GetAttribute(SmFileAttribute.TITLE);

            Assert.IsTrue(string.IsNullOrEmpty(value) == false);
        }

        [TestMethod]
        public void GetNonExistentAttribute()
        {
            string value = this.SmFile.GetAttribute(SmFileAttribute.TIMESIGNATURES);

            Assert.IsTrue(string.IsNullOrEmpty(value));
        }

        [TestMethod]
        public void GetCommonAttributes()
        {
            Assert.AreEqual("BUTTERFLY", this.SmFile.SongName);
            Assert.AreEqual("TestData", this.SmFile.Group); //Group name is the name of the parent folder
            Assert.AreEqual("BUTTERFLY.png", this.SmFile.BannerPath);
            Assert.AreEqual("SMILE.dk", this.SmFile.Artist);
        }
        [TestMethod]
        public void GetHighestDifficulty()
        {
            using (var data = this.SmFile.ExtractChartData())
            {
                var songDifficulty = data.GetHighestChartedDifficulty(PlayStyle.Single);

                Assert.IsTrue(songDifficulty == SongDifficulty.Challenge);
            }
        }

        [TestMethod]
        public void GetChartData()
        {
            using (var data = this.SmFile.ExtractChartData())
            {
                var stepData = data.GetSteps(PlayStyle.Single, SongDifficulty.Challenge);
                Assert.IsTrue(stepData.Measures.All(m => m.Notes.Count % 4 == 0));
            }
        }

        [TestMethod]
        public void GenerateLightsChart()
        {
            using (var data = this.SmFile.ExtractChartData())
            {
                var reference = data.GetSteps(PlayStyle.Single, SongDifficulty.Challenge);
                var lightsData = StepChartBuilder.GenerateLightsChart(reference);

                Assert.IsTrue(lightsData != null);
            }
        }

        [TestMethod]
        public void GenerateLightsChart_Doubles()
        {
            using (var data = this.SmFile.ExtractChartData())
            {
                var reference = data.GetSteps(PlayStyle.Double, SongDifficulty.Challenge);
                var lightsData = StepChartBuilder.GenerateLightsChart(reference);

                Assert.IsTrue(lightsData != null);
            }
        }
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GenerateLightsChart_NoReference()
        {
            using (var data = this.SmFile.ExtractChartData())
            {
                var reference = data.GetSteps(PlayStyle.Undefined, SongDifficulty.Challenge);
                StepChartBuilder.GenerateLightsChart(reference);
            }
        }

        [TestMethod]
        public void SaveChartData()
        {
            bool hasLightsDataBeforeSave;
            bool hasLightsDataAfterSave;

            using (var data = this.SmFile.ExtractChartData())
            {
                hasLightsDataBeforeSave = data.GetSteps(PlayStyle.Lights, SongDifficulty.Easy) != null;

                var reference = data.GetSteps(PlayStyle.Single, SongDifficulty.Hard);
                var lightsData = StepChartBuilder.GenerateLightsChart(reference);

                data.AddNewStepchart(lightsData);
            }

            var newSmFileReference = new SmFile(new FileInfo($"{TEST_DATA_ROOT}/DDR1stMix/BUTTERFLY.sm"));

            using (var data = newSmFileReference.ExtractChartData())
            {
                hasLightsDataAfterSave = data.GetSteps(PlayStyle.Lights, SongDifficulty.Easy) != null;
            }
            Assert.IsFalse(hasLightsDataBeforeSave);
            Assert.IsTrue(hasLightsDataAfterSave);
        }
    }
}
