using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StepmaniaUtils.Core;
using StepmaniaUtils.Enums;

namespace StepmaniaUtils.StepChart
{
    [DebuggerDisplay("{PlayStyle} - {Difficulty} - {DifficultyRating}")]
    public class StepData : IDisposable
    {
        public PlayStyle PlayStyle { get; set; }
        public SongDifficulty Difficulty { get; set; }
        public int DifficultyRating { get; set; }
        public string ChartAuthor { get; set; }

        public List<MeasureData> Measures { get; set; }

        public StepData(PlayStyle playStyle, SongDifficulty difficulty, int difficultyRating, string chartAuthor)
        {
            PlayStyle = playStyle;
            Difficulty = difficulty;
            DifficultyRating = difficultyRating;
            ChartAuthor = chartAuthor;

            Measures = new List<MeasureData>();
        }

        public StepData(PlayStyle playStyle, SongDifficulty difficulty, int difficultyRating, string chartAuthor, List<string> rawChartData)
            : this(playStyle, difficulty, difficultyRating, chartAuthor)
        {
            Measures = ParseRawChartData(rawChartData);
        }

        private List<MeasureData> ParseRawChartData(List<string> rawChartData)
        {
            var rawMeasures = string.Join("\n", rawChartData).Split(',');

            return rawMeasures.Select(data => new MeasureData(data)).ToList();
        }

        public List<string> GetRawChartData()
        {
            var rawData = new List<string>
            {
                $"//---------------{PlayStyle.ToStyleName()}-----------------",
                $"#NOTES:",
                $"    {PlayStyle.ToStyleName()}:",
                $"    {ChartAuthor}:",
                $"    {Difficulty}:",
                $"    {DifficultyRating}:",
                $"    0.000,0.000,0.000,0.000,0.000:"
            };

            foreach (var measureData in Measures)
            {
                foreach (var noteData in measureData.Notes)
                {
                    rawData.Add(noteData.Columns);
                }
                rawData.Add(",");
            }

            rawData.RemoveAt(rawData.Count - 1); //remove the last comma, replace with semicolon
            rawData.Add(";");

            return rawData;
        }

        public void Dispose()
        {
            foreach (var measureData in Measures)
            {
                measureData.Dispose();
            }

            Measures = null;
        }
    }
}