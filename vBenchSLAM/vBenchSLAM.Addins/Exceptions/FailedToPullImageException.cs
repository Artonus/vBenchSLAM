using System;

namespace vBenchSLAM.Addins.Exceptions
{
    [Serializable]
    public class FailedToPullImageException : Exception
    {
        public int ExitCode { get; }
        public string ImageName { get; }

        public FailedToPullImageException(string message, int exitCode) : base(message)
        {
            ExitCode = exitCode;
        }
        public FailedToPullImageException(string message, int exitCode, string imageName) : base(message)
        {
            ExitCode = exitCode;
            ImageName = imageName;
        }
    }
}