using System;
using System.Collections.Generic;
using System.IO;

namespace vBenchSLAM.Core.Model
{
    /// <summary>
    /// Result of the dataset completeness check
    /// </summary>
    internal class DatasetCheckResult
    {
        /// <summary>
        /// Is valid dataset
        /// </summary>
        public bool IsValid { get; }
        /// <summary>
        /// Generated exception, null if dataset is correct
        /// </summary>
        public Exception Exception { get; }
        /// <summary>
        /// Localized vocabulary file
        /// </summary>
        public FileInfo VocabFile { get; set; }
        /// <summary>
        /// Localized configuration file
        /// </summary>
        public FileInfo ConfigFile { get; set; }
        /// <summary>
        /// Localized sequence directory
        /// </summary>
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
        /// <summary>
        /// Get the list of all identified files in the dataset. Does not include files in the sequence fol;der
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FileInfo> GetAllFiles()
        {
            return new[] {VocabFile, ConfigFile};
        }
    }
}
