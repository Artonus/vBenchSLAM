using System;

namespace vBenchSLAM.Core.Model
{
    public class DatasetCheckResult
    {
        public bool IsValid { get; }
        public Exception Exception { get; }

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

    }
}
