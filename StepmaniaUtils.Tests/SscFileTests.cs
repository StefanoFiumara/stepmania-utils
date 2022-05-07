using System.IO;
using System.Linq;
using StepmaniaUtils.Enums;
using StepmaniaUtils.StepGenerator;
using StepmaniaUtils.Tests.TestConstants;
using Xunit;

namespace StepmaniaUtils.Tests
{
    public class SscFileTests
    {
        [Theory]
        [InlineData(Ssc.HELLO, "xRGTMx")]
        public void SscFile_Parses_Chart_Name(string sscFilePath, string expectedChartName)
        {
            var sscFile = new SmFile(sscFilePath);

            var actualChartName = sscFile.ChartMetadata.StepCharts.Single().ChartName;

            Assert.Equal(expectedChartName, actualChartName);
        }

        [Theory]
        [InlineData(Ssc.HELLO, PlayStyle.Single)]
        public void SscFile_Parses_Steps_Type(string sscFilePath, PlayStyle expectedPlayStyle)
        {
            var sscFile = new SmFile(sscFilePath);

            var actualPlayStyle = sscFile.ChartMetadata.StepCharts.Single().PlayStyle;

            Assert.Equal(expectedPlayStyle, actualPlayStyle);
        }

        [Theory]
        [InlineData(Ssc.HELLO, SongDifficulty.Challenge)]
        public void SscFile_Parses_Difficulty(string sscFilePath, SongDifficulty expecteDifficulty)
        {
            var sscFile = new SmFile(sscFilePath);

            var actualDifficulty = sscFile.ChartMetadata.StepCharts.Single().Difficulty;

            Assert.Equal(expecteDifficulty, actualDifficulty);
        }

        [Theory]
        [InlineData(Ssc.HELLO, 10)]
        public void SscFile_Parses_Difficulty_Meter(string sscFilePath, int expectedMeter)
        {
            var sscFile = new SmFile(sscFilePath);

            var actualMeter = sscFile.ChartMetadata.StepCharts.Single().DifficultyRating;

            Assert.Equal(expectedMeter, actualMeter);
        }

        [Theory]
        [MemberData(nameof(Ssc.Data), MemberType = typeof(Ssc))]
        public void SscFile_LightsChart_Saves_Data_To_File(string sscFilePath)
        {
            // We create a copy of each .ssc file since this test writes data to the files under test
            // and we do not want this data to conflict with other tests
            var sscFileCopy = $"{sscFilePath}.test.ssc";
            string backupFilePath = $"{sscFileCopy}.backup";

            File.Copy(sscFilePath, sscFileCopy, true);
            var smFile = new SmFile(sscFileCopy);

            bool hasLightsBeforeSave = smFile.ChartMetadata.GetSteps(PlayStyle.Lights, SongDifficulty.Easy) != null;

            var chart = StepChartBuilder.GenerateLightsChart(smFile);

            smFile.WriteLightsChart(chart);

            bool hasLightsAfterSave = smFile.ChartMetadata.GetSteps(PlayStyle.Lights, SongDifficulty.Easy) != null;

            Assert.False(hasLightsBeforeSave, $".ssc file under test already has a lights chart defined.\n{sscFileCopy}");
            Assert.True(hasLightsAfterSave, $".ssc file did not have lights chart after save.\n{sscFileCopy}");

            try
            {
                if(File.Exists(sscFileCopy)) File.Delete(sscFileCopy);
                if(File.Exists(backupFilePath)) File.Delete(backupFilePath);
            }

            catch { /* intentionally left empty */ }
        }
        //TODO: Test files with multiple charts
        //TODO: Test files with multiple charts for the same difficulty/game mode (PIU support)
        //TODO: Test light chart generation with correct header
    }
}
