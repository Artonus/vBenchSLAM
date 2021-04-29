using System;
using System.Collections.Generic;
using System.IO;

namespace vBenchSLAM.Core.Model
{
    public class DatasetCheckResult
    {
        public bool IsValid { get; }
        public Exception Exception { get; }
        public FileInfo VocabFile { get; set; }
        public FileInfo ConfigFile { get; set; }
        public FileInfo VideoFile { get; set; }
        public DirectoryInfo SequenceDirectory { get; set; }

        public DatasetCheckResult(bool isValid, Exception exception)
        {
            IsValid = isValid;
            if (isValid == false && exception is null)
            {
                throw new ArgumentNullException(nameof(exception),
                    $"Parameter {nameof(exception)} cannot be null if the {nameof(isValid)} parameter is false");
            }
            Exception = exception;
        }

        public IEnumerable<FileInfo> GetAllFiles()
        {
            return new[] {VocabFile, ConfigFile, VideoFile};
        }
    }
}
