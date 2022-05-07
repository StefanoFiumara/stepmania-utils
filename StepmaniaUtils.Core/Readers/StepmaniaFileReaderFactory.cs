using System;
using System.IO;

namespace StepmaniaUtils.Readers
{
    public static class StepmaniaFileReaderFactory
    {
        public static IStepmaniaFileReader CreateReader(string smOrSscFilePath)
        {
            switch (Path.GetExtension(smOrSscFilePath))
            {
                case ".sm": return new SmFileReader(smOrSscFilePath);
                case ".ssc": return new SscFileReader(smOrSscFilePath);

                default: throw new ArgumentException("The given file type is not supported.");
            }
        }
    }
}