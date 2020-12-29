using System;

namespace vBenchSLAM.Addins.Exceptions
{
    [Serializable]
    public class FailedToBuildImageException : Exception
    {
        public int ExitCode { get; }
        public string ImageName { get; }

        public FailedToBuildImageException(string message, int exitCode) : base(message)
        {
            ExitCode = exitCode;
        }
        public FailedToBuildImageException(string message, int exitCode, string imageName) : base(message)
        {
            ExitCode = exitCode;
            ImageName = imageName;
        }
    }
}