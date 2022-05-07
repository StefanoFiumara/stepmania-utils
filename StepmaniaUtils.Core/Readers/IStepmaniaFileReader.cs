using System;
using System.Collections.Generic;
using StepmaniaUtils.Enums;
using StepmaniaUtils.StepData;

namespace StepmaniaUtils.Readers
{
    public interface IStepmaniaFileReader : IDisposable
    {
        /// <summary>
        /// Path to the file being read.
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// The current state of the reader, allows the user to decipher where the reader is positioned in the file
        /// </summary>
        ReaderState State { get; }
        
        /// <summary>
        /// Helper property to maintain backward compatibility
        /// </summary>
        bool IsParsingNoteData { get; }

        /// <summary>
        /// Reads the file up until it encounters the next header tag
        /// </summary>
        /// <param name="tag">The header tag that was read</param>
        /// <returns>True if a valid header tag was found, false otherwise</returns> 
        bool ReadNextTag(out SmFileAttribute tag);

        /// <summary>
        /// Reads the value of the most recently parsed header tag.
        /// </summary>
        string ReadTagValue();

        /// <summary>
        /// Skip the reader ahead to the end of the current tag value
        /// </summary>
        void SkipTagValue();

        /// <summary>
        /// Parses the metadata of the chart where the reader is currently positioned.
        /// </summary>
        StepMetadata ReadStepchartMetadata();

        /// <summary>
        /// Reads and returns a measure of data for the chart currently being parsed.
        /// </summary>
        IEnumerable<string> ReadMeasure();
    }
}