using System;
using System.IO;
using StepmaniaUtils.Core;
using StepmaniaUtils.Enums;
using StepmaniaUtils.StepGenerator;
using Xunit;

namespace StepmaniaUtils.Tests
{
    public class SmFileTests
    {
        public const string TEST_DATA_BUTTERFLY = "TestData/DDR1stMix/BUTTERFLY.sm";
        public const string TEST_DATA_GARGOYLE = "TestData/Chris/gargoyle.sm";
        public const string TEST_DATA_ARABIAN_NIGHTS = "TestData/Doubles/ArabianNights.sm";
        public const string TEST_DATA_RAFFLES = "TestData/Misc/raffles.sm";

        [Fact]
        public void SmFile_Throws_Exception_When_File_Does_Not_Exist()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var smFile = new SmFile("TestData/wrongFile.sm");
            });
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY, "BUTTERFLY")]
        [InlineData(TEST_DATA_GARGOYLE, "Gargoyle")]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS, "1001 Arabian Nights")]
        [InlineData(TEST_DATA_RAFFLES, "Magic Raffles")]
        public void SmFile_Title_Attribute_Is_Populated(string smFileName, string expectedTitle)
        {
            var smFile = new SmFile(smFileName);

            string actualTitle = smFile.SongTitle;

            Assert.Equal(expectedTitle, actualTitle);
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY)]
        [InlineData(TEST_DATA_GARGOYLE)]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS)]
        public void SmFile_Returns_Empty_String_For_NonExistent_Attribute(string smFilePath)
        {
            var smFile = new SmFile(smFilePath);

            string value = smFile[SmFileAttribute.TIMESIGNATURES];

            Assert.Equal(string.Empty, value);
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY, "TestData")]
        [InlineData(TEST_DATA_GARGOYLE, "TestData")]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS, "TestData")]
        [InlineData(TEST_DATA_RAFFLES, "TestData")]
        public void SmFile_Group_Attribute_Is_Populated(string smFilePath, string expectedGroup)
        {
            var smFile = new SmFile(smFilePath);

            string actualGroup = smFile.Group;

            Assert.Equal(expectedGroup, actualGroup);
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY, "BUTTERFLY.png")]
        [InlineData(TEST_DATA_GARGOYLE, "../bn.png")]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS, "../throughtheages-bn.png")]
        [InlineData(TEST_DATA_RAFFLES, "raffles-bn.png")]
        public void SmFile_Banner_Attribute_Is_Populated(string smFilePath, string expectedBanner)
        {
            var smFile = new SmFile(smFilePath);

            string actualBanner = smFile.BannerPath;

            Assert.Equal(expectedBanner, actualBanner);
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY, "SMILE.dk")]
        [InlineData(TEST_DATA_GARGOYLE, "Sanxion7")]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS, "Ch!pz")]
        [InlineData(TEST_DATA_RAFFLES, "t+pazolite")]
        public void SmFile_Artist_Attribute_Is_Populated(string smFilePath, string expectedArtist)
        {
            var smFile = new SmFile(smFilePath);

            string actualArtist = smFile.Artist;

            Assert.Equal(expectedArtist, actualArtist);
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY, SongDifficulty.Challenge)]
        [InlineData(TEST_DATA_GARGOYLE, SongDifficulty.Challenge)]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS, SongDifficulty.None)]
        [InlineData(TEST_DATA_RAFFLES, SongDifficulty.Challenge)]
        public void SmFile_GetHighestChartedDifficulty_Returns_The_Highest_Difficulty(string smFilePath, SongDifficulty expectedDifficulty)
        {
            var smFile = new SmFile(smFilePath);

            var actualDifficulty = smFile.ChartMetadata.GetHighestChartedDifficulty(PlayStyle.Single);

            Assert.Equal(expectedDifficulty, actualDifficulty);
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY, 16)]
        [InlineData(TEST_DATA_GARGOYLE, 1)]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS, 1)]
        [InlineData(TEST_DATA_RAFFLES, 2)]
        public void SmFile_ChartMetadata_Populates_All_Stepcharts(string smFilePath, int expectedChartCount)
        {
            var smFile = new SmFile(smFilePath);

            int actualChartCount = smFile.ChartMetadata.StepCharts.Count;

            Assert.Equal(expectedChartCount, actualChartCount);
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY)]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS)] //Doubles Only chart
        public void SmFile_GenerateLightsChart_Generates_Lights_Data_Content(string smFilePath)
        {
            var smFile = new SmFile(smFilePath);

            var lightsData = StepChartBuilder.GenerateLightsChart(smFile);

            Assert.True(!string.IsNullOrEmpty(lightsData.Content));
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY, PlayStyle.Single, SongDifficulty.Hard)]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS, PlayStyle.Double, SongDifficulty.Challenge)] //Doubles Only chart
        public void SmFile_GenerateLightsChart_Maintains_Reference_Chart_Data(string smFilePath,
            PlayStyle referenceChartStyle, SongDifficulty referenceChartDifficulty)
        {
            var smFile = new SmFile(smFilePath);

            var lightsData = StepChartBuilder.GenerateLightsChart(smFile);

            Assert.Equal(referenceChartStyle, lightsData.ReferenceChart.PlayStyle);
            Assert.Equal(referenceChartDifficulty, lightsData.ReferenceChart.Difficulty);
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY)]
        [InlineData(TEST_DATA_GARGOYLE)] //Test case for missing semicolon in attribute tag
        public void SmFile_BannerPath_Points_To_Valid_File(string smFilePath)
        {
            var smFile = new SmFile(smFilePath);
            var fullBannerPath = Path.Combine(smFile.Directory, smFile.BannerPath);

            var bannerExists = File.Exists(fullBannerPath);

            Assert.True(bannerExists, $"Banner path could not be found: {fullBannerPath}");
        }


        [Theory]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS, "138")]
        [InlineData(TEST_DATA_BUTTERFLY, "135")]
        public void SmFile_With_DisplayBpm_Tag_Shows_Correct_Bpm(string smFilePath, string expectedDisplayBpm)
        {
            var smFile = new SmFile(smFilePath);

            Assert.Equal(expectedDisplayBpm, smFile.DisplayBpm);
        }

        [Theory]
        [InlineData(TEST_DATA_GARGOYLE, "75-150")]
        [InlineData(TEST_DATA_RAFFLES, "105-210")]
        public void SmFile_With_No_DisplayBpm_Tag_Still_Shows_Correct_Bpm_Range(string smFilePath, string expectedBpm)
        {
            var smFile = new SmFile(smFilePath);

            var bpm = smFile.DisplayBpm;

            Assert.Equal(expectedBpm, bpm);
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY)]
        [InlineData(TEST_DATA_GARGOYLE)]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS)]
        [InlineData(TEST_DATA_RAFFLES)]
        public void SmFile_LightsChart_Saves_Data_To_File(string smFilePath)
        {
            // We create a copy of each .sm file since this test writes data to the files under test
            // and we do not want this data to conflict with other tests
            var smFileCopy = $"{smFilePath}.test.sm";
            string backupFilePath = $"{smFileCopy}.backup";

            File.Copy(smFilePath, smFileCopy, true);
            var smFile = new SmFile(smFileCopy);

            bool hasLightsBeforeSave = smFile.ChartMetadata.GetSteps(PlayStyle.Lights, SongDifficulty.Easy) != null;

            var chart = StepChartBuilder.GenerateLightsChart(smFile);

            smFile.WriteLightsChart(chart);
            smFile.Refresh();

            bool hasLightsAfterSave = smFile.ChartMetadata.GetSteps(PlayStyle.Lights, SongDifficulty.Easy) != null;

            Assert.False(hasLightsBeforeSave, $".sm file under test already has a lights chart defined.\n{smFileCopy}");
            Assert.True(hasLightsAfterSave, $".sm file did not have lights chart after save.\n{smFileCopy}");

            try
            {
                File.Delete(smFileCopy);
                File.Delete(backupFilePath);
            }

            catch { /* intentionally left empty */ }
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY)]
        [InlineData(TEST_DATA_GARGOYLE)]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS)]
        [InlineData(TEST_DATA_RAFFLES)]
        public void SmFile_LightsChart_Creates_Backup_File(string smFilePath)
        {
            // We create a copy of each .sm file since this test writes data to the files under test
            // and we do not want this data to conflict with other tests
            var smFileCopy = $"{smFilePath}.test.sm";
            string backupFilePath = $"{smFileCopy}.backup";

            File.Copy(smFilePath, smFileCopy, true);
            var smFile = new SmFile(smFileCopy);

            var chart = StepChartBuilder.GenerateLightsChart(smFile);
            smFile.WriteLightsChart(chart);

            var backupFileExists = File.Exists(backupFilePath);

            Assert.True(backupFileExists, $".sm backup file was not created.\n{smFileCopy}");

            try
            {
                File.Delete(smFileCopy);
                File.Delete(backupFilePath);
            }

            catch { /* intentionally left empty */ }
        }

        [Theory]
        [InlineData(TEST_DATA_BUTTERFLY)]
        [InlineData(TEST_DATA_GARGOYLE)]
        [InlineData(TEST_DATA_ARABIAN_NIGHTS)]
        [InlineData(TEST_DATA_RAFFLES)]
        public void SMFile_LightsChart_Does_Not_Leave_Unended_Holds(string smFilePath)
        {
            var smFileCopy = $"{smFilePath}.test.sm";
            string backupFilePath = $"{smFileCopy}.backup";

            File.Copy(smFilePath, smFileCopy, true);
            var smFile = new SmFile(smFileCopy);

            var chart = StepChartBuilder.GenerateLightsChart(smFile);
            smFile.WriteLightsChart(chart);

            var endHoldState = LightsChartHelper.VerifyLightChartHolds(smFileCopy);

            Assert.Equal(LightsChartHelper.LightChartHoldState.HoldingNone, endHoldState);

            try
            {
                File.Delete(smFileCopy);
                File.Delete(backupFilePath);
            }

            catch { /* intentionally left empty */ }
        }
    }
}
