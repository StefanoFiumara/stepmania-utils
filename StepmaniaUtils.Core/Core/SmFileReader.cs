using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StepmaniaUtils.Enums;
using StepmaniaUtils.StepData;

namespace StepmaniaUtils.Core
{
    /// <inheritdoc />
    /// <summary>
    /// StreamReader wrapper designed to expose methods to properly parse a stepmania .sm file.
    /// </summary>
    public class SmFileReader : IDisposable
    {
        private readonly StreamReader _reader;
        private readonly FileStream _stream;

        private readonly StringBuilder _buffer;

        private SmFileAttribute _lastReadTag;

        /// <summary>
        /// True when the reader is positioned to read Note Data
        /// </summary>
        public bool IsParsingNoteData { get; private set; }

        /// <summary>
        /// Initializes the SmFileReader with the given smFilePath
        /// </summary>
        /// <param name="smFilePath"></param>
        public SmFileReader(string smFilePath)
        {
            if (string.IsNullOrEmpty(smFilePath))
            {
                throw new ArgumentNullException(nameof(smFilePath), "*.sm file path cannot be null or empty");
            }
            if (Path.GetExtension(smFilePath) != ".sm")
            {
                throw new ArgumentException("File path must point to a *.sm file", nameof(smFilePath));
            }
            _stream = new FileStream(smFilePath, FileMode.Open, FileAccess.Read);
            _reader = new StreamReader(_stream);
            _buffer = new StringBuilder();

            _lastReadTag = SmFileAttribute.UNDEFINED;
            IsParsingNoteData = false;
        }
        
        /// <summary>
        /// Reads the .sm file up until it encounters the next header tag
        /// </summary>
        /// <param name="tag">The header tag that was read</param>
        /// <returns>True if a valid header tag was found, false otherwise</returns>
        public bool ReadNextTag(out SmFileAttribute tag)
        {
            _buffer.Clear();
            
            while (!_reader.EndOfStream)
            {
                if (_reader.Peek() != ':')
                {
                    _buffer.Append((char)_reader.Read());
                }
                else
                {
                    tag = _buffer.SkipWhile(c => c != '#').ToString().Trim('#').ToAttribute();

                    //if the tag could not be read, skip this tag and read the next one
                    if (tag == SmFileAttribute.UNDEFINED)
                    {
                        _reader.Read(); //toss ':' token
                        _buffer.Clear();
                        continue;
                    }

                    _lastReadTag = tag;
                    IsParsingNoteData = false;
                    return true;
                }
            }

            tag = SmFileAttribute.UNDEFINED;
            IsParsingNoteData = false;
            return false;
        }


        /// <summary>
        /// Reads the value of the most recently parsed header tag.
        /// </summary>
        public string ReadTagValue()
        {
            if (_reader.Peek() != ':')
            {
                throw new InvalidOperationException("Reader is not positioned to read a tag value.");
            }

            if (_reader.Peek() == ':' && _lastReadTag == SmFileAttribute.NOTES)
            {
                throw new InvalidOperationException("Reader is positioned to read #NOTE data, use ReadStepchartMetadata method.");
            }

            _reader.Read(); //toss ':' token
            _buffer.Clear();
            while (_reader.Peek() != ';' && _reader.Peek() != '\n') //read until semicolon or newline char
            {
                _buffer.Append((char)_reader.Read());
            }

            IsParsingNoteData = false;
            return _buffer.ToString().Trim();
        }

        /// <summary>
        /// Reads stepchart metadata when the reader is positioned on a #NOTES tag.
        /// </summary>
        public StepMetadata ReadStepchartMetadata()
        {
            if (_lastReadTag != SmFileAttribute.NOTES || _reader.Peek() != ':')
            {
                throw new InvalidOperationException("Reader is not positioned to read chart metadata.");
            }

            _reader.Read(); //toss ':' token

            var stepData = new StepMetadata
            {
                PlayStyle = ReadNextNoteHeaderSection().ToStyleEnum(),
                ChartAuthor = ReadNextNoteHeaderSection(),
                Difficulty = ReadNextNoteHeaderSection().ToSongDifficultyEnum(),
                DifficultyRating = (int)double.Parse(ReadNextNoteHeaderSection())
            };

            //Skip groove radar values
            ReadNextNoteHeaderSection();

            IsParsingNoteData = true;

            return stepData;
        }

        /// <summary>
        /// Skip the stream reader ahead to the end of the current tag value
        /// </summary>
        public void SkipValue()
        {
            while (_reader.Peek() != ';') _reader.Read();
            IsParsingNoteData = false;
        }

        /// <summary>
        /// Reads and returns a measure of data for the chart currently being parsed.
        /// </summary>
        public IEnumerable<string> ReadMeasure()
        {
            if (!IsParsingNoteData)
            {
                throw new InvalidOperationException("Reader is not positioned to read measure data.");
            }

            _buffer.Clear();

            while (_reader.Peek() != ',' && _reader.Peek() != ';') _buffer.Append((char)_reader.Read());

            var measureLines = _buffer.ToString().Split(Environment.NewLine.ToCharArray())
                .Select(data => data.Trim())
                .Where(data => !data.Contains(@"//"))
                .Where(data => !string.IsNullOrWhiteSpace(data));

            if (_reader.Peek() == ';')
            {
                IsParsingNoteData = false;
            }
            else
            {
                _reader.Read();
            }

            return measureLines;

        }
        
        private string ReadNextNoteHeaderSection()
        {
            _buffer.Clear();
            while (_reader.Peek() != ':')
            {
                _buffer.Append((char)_reader.Read());
            }
            _reader.Read(); //toss ':' token

            return _buffer.SkipWhile(char.IsWhiteSpace).ToString();
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _stream?.Dispose();
        }
    }
}