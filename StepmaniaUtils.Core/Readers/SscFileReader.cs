using System;
using StepmaniaUtils.Enums;
using StepmaniaUtils.Extensions;
using StepmaniaUtils.StepData;

namespace StepmaniaUtils.Readers
{
    public class SscFileReader : SmFileReader
    {
        /// <summary>
        /// An extension of the SmFileReader that supports reading .ssc files
        /// </summary>
        /// <param name="sscFilePath"></param>
        internal SscFileReader(string sscFilePath) : base(sscFilePath)
        {

        }

        public override bool ReadNextTag(out SmFileAttribute tag)
        {
            var success = base.ReadNextTag(out tag);

            if (success)
            {
                State = tag == SmFileAttribute.NOTEDATA
                    ? ReaderState.ReadingChartMetadata
                    : ReaderState.ReadingTagValue;
            }
            else
            {
                State = ReaderState.Default;
            }

            return success;
        }

        public override StepMetadata ReadStepchartMetadata()
        {
            if (State != ReaderState.ReadingChartMetadata)
            {
                throw new InvalidOperationException("Reader is not positioned to read Chart metadata");
            }

            var stepData = new StepMetadata();
            do
            {
                ReadNextTag(out SmFileAttribute tag);

                switch (tag)
                {
                    case SmFileAttribute.CHARTNAME:
                        stepData.ChartName = ReadTagValue();
                        break;
                    case SmFileAttribute.STEPSTYPE:
                        stepData.PlayStyle = ReadTagValue().AsPlayStyle();
                        break;
                    //case SmFileAttribute.DESCRIPTION:
                    //    break;
                    //case SmFileAttribute.CHARTSTYLE:
                    //    break;
                    case SmFileAttribute.DIFFICULTY:
                        stepData.Difficulty = ReadTagValue().AsSongDifficulty();
                        break;
                    case SmFileAttribute.METER:
                        stepData.DifficultyRating = (int)double.Parse(ReadTagValue());
                        break;
                    //case SmFileAttribute.CREDIT:
                    //    break;
                    //case SmFileAttribute.DISPLAYBPM:
                    //    break;
                    case SmFileAttribute.NOTES:
                        break;
                    default:
                        SkipTagValue();
                        break;
                }

            } while (_lastReadTag != SmFileAttribute.NOTES);

            _reader.Read(); //Toss ':' token

            State = ReaderState.ReadingNoteData;
            return stepData;
        }
    }
}
