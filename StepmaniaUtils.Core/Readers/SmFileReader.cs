using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StepmaniaUtils.Enums;
using StepmaniaUtils.Extensions;
using StepmaniaUtils.StepData;

namespace StepmaniaUtils.Readers
{
    public class SmFileReader : IStepmaniaFileReader
    {
        protected readonly StreamReader _reader;
        protected readonly FileStream _stream;

        protected readonly StringBuilder _buffer;

        protected SmFileAttribute _lastReadTag;
        
        public string FilePath { get; }

        public ReaderState State { get; protected set; }

        public bool IsParsingNoteData => State == ReaderState.ReadingNoteData;

        internal SmFileReader(string smFilePath)
        {
            if (string.IsNullOrEmpty(smFilePath))
            {
                throw new ArgumentNullException(nameof(smFilePath), "*.sm file path cannot be null or empty");
            }
            
            _stream = new FileStream(smFilePath, FileMode.Open, FileAccess.Read);
            _reader = new StreamReader(_stream);
            _buffer = new StringBuilder();

            _lastReadTag = SmFileAttribute.UNDEFINED;

            FilePath = smFilePath;
            State = ReaderState.Default;
        }

        public virtual bool ReadNextTag(out SmFileAttribute tag)
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
                    tag = _buffer.SkipWhile(c => c != '#').ToString().Trim('#').AsSmFileAttribute();

                    //if the tag could not be read, skip this tag and read the next one
                    if (tag == SmFileAttribute.UNDEFINED)
                    {
                        _reader.Read(); //toss ':' token
                        _buffer.Clear();
                        continue;
                    }

                    _lastReadTag = tag;
                    State = tag == SmFileAttribute.NOTES ? ReaderState.ReadingChartMetadata : ReaderState.ReadingTagValue;
                    return true;
                }
            }

            tag = SmFileAttribute.UNDEFINED;
            State = ReaderState.Default;
            return false;
        }

        public string ReadTagValue()
        {
            if (State != ReaderState.ReadingTagValue)
            {
                throw new InvalidOperationException("Reader is not positioned to read a tag value.");
            }

            _reader.Read(); //toss ':' token
            _buffer.Clear();
            while (_reader.Peek() != ';' && _reader.Peek() != '#') //read until semicolon or next tag
            {
                _buffer.Append((char)_reader.Read());
            }

            State = ReaderState.Default;
            return _buffer.ToString().Trim();
        }

        public void SkipTagValue()
        {
            while (_reader.Peek() != ';') _reader.Read();
            State = ReaderState.Default;
        }

        public virtual StepMetadata ReadStepchartMetadata()
        {
            if(State != ReaderState.ReadingChartMetadata)
            {
                throw new InvalidOperationException("Reader is not positioned to read chart metadata.");
            }

            _reader.Read(); //toss ':' token

            var stepData = new StepMetadata
            {
                PlayStyle = ReadNoteHeaderSection().AsPlayStyle(),
                ChartAuthor = ReadNoteHeaderSection(),
                Difficulty = ReadNoteHeaderSection().AsSongDifficulty(),
                DifficultyRating = (int)double.Parse(ReadNoteHeaderSection())
            };

            //Skip groove radar values - no one cares
            ReadNoteHeaderSection();

            State = ReaderState.ReadingNoteData;

            return stepData;
        }

        public IEnumerable<string> ReadMeasure()
        {
            if (State != ReaderState.ReadingNoteData)
            {
                throw new InvalidOperationException("Reader is not positioned to read measure data.");
            }

            _buffer.Clear();

            while (_reader.Peek() != ',' && _reader.Peek() != ';') _buffer.Append((char)_reader.Read());

            var measureLines = _buffer.ToString().Split(Environment.NewLine.ToCharArray())
                .Select(data => data.Trim())
                .Where(data => !data.Contains("//"))
                .Where(data => !string.IsNullOrWhiteSpace(data));

            if (_reader.Peek() == ';')
            {
                State = ReaderState.Default;
            }
            else
            {
                _reader.Read();
            }

            return measureLines;

        }
        
        private string ReadNoteHeaderSection()
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