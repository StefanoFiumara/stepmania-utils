using System;
using System.Collections.Generic;
using System.IO;
using StepmaniaUtils.Enums;
using StepmaniaUtils.Readers;

namespace StepmaniaUtils.Tests
{
    internal static class LightsChartHelper
    {
        [Flags]
        public enum LightChartHoldState
        {
            HoldingNone         = 0,
            HoldingMarqueeLeft  = 1 << 0,
            HoldingMarqueeDown  = 1 << 1,
            HoldingMarqueeUp    = 1 << 2,
            HoldingMarqueeRight = 1 << 3,
            HoldingBassLeft     = 1 << 4,
            HoldingBassRight    = 1 << 5
        }

        private const int MARQUEE_LEFT  = 0;
        private const int MARQUEE_DOWN  = 1;
        private const int MARQUEE_UP    = 2;
        private const int MARQUEE_RIGHT = 3;
        private const int BASS_LEFT     = 4;
        private const int BASS_RIGHT    = 5;

        private const char HOLD_BEGIN = '2';
        private const char ROLL_BEGIN = '4';
        private const char HOLD_END = '3';

        private static readonly Dictionary<int, LightChartHoldState> HoldStateMap = new Dictionary<int, LightChartHoldState>
        {
            {MARQUEE_LEFT, LightChartHoldState.HoldingMarqueeLeft},
            {MARQUEE_RIGHT, LightChartHoldState.HoldingMarqueeRight},
            {MARQUEE_DOWN, LightChartHoldState.HoldingMarqueeDown},
            {MARQUEE_UP, LightChartHoldState.HoldingMarqueeUp},
            {BASS_LEFT, LightChartHoldState.HoldingBassLeft},
            {BASS_RIGHT, LightChartHoldState.HoldingBassRight},
        };

        public static LightChartHoldState VerifyLightChartHolds(string smFilePath)
        {
            var holdState = LightChartHoldState.HoldingNone;

            void VerifyHoldState(string line, int column, int measure)
            {
                if (line[column] == HOLD_BEGIN || line[column] == ROLL_BEGIN)
                {
                    if (!holdState.HasFlag(HoldStateMap[column]))
                    {
                        holdState |= HoldStateMap[column];
                    }
                    else
                    {
                        throw new Exception($"Parser encountered HOLD_BEGIN on column {column} (note: {line[column]}, measure: {measure}) when hold state was already holding.\n{Path.GetFullPath(smFilePath)}");
                    }
                }
                else if (line[column] == HOLD_END)
                {
                    if (holdState.HasFlag(HoldStateMap[column]))
                    {
                        holdState &= ~HoldStateMap[column];
                    }
                    else
                    {
                        throw new Exception($"Parser encountered HOLD_END on column {column} (note: {line[column]}, measure: {measure}) when hold state was not holding.\n{Path.GetFullPath(smFilePath)}");
                    }
                }
            }

            using (var reader = StepmaniaFileReaderFactory.CreateReader(smFilePath))
            {
                while (reader.ReadNextTag(out SmFileAttribute tag))
                {
                    if (tag != SmFileAttribute.NOTES) continue;

                    var stepData = reader.ReadStepchartMetadata();

                    if (stepData.PlayStyle == PlayStyle.Lights && stepData.Difficulty == SongDifficulty.Easy)
                    {
                        int measureNumber = 1;
                        while (reader.IsParsingNoteData)
                        {
                            var measure = reader.ReadMeasure();

                            foreach (string line in measure)
                            {
                                VerifyHoldState(line, MARQUEE_LEFT, measureNumber);
                                VerifyHoldState(line, MARQUEE_UP, measureNumber);
                                VerifyHoldState(line, MARQUEE_DOWN, measureNumber);
                                VerifyHoldState(line, MARQUEE_RIGHT, measureNumber);
                                VerifyHoldState(line, BASS_LEFT, measureNumber);
                                VerifyHoldState(line, BASS_RIGHT, measureNumber);
                            }

                            measureNumber++;
                        }

                        break;
                    }
                }
            }

            return holdState;
        }
    }
}