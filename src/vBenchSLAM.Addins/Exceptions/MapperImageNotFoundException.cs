using System;

namespace vBenchSLAM.Addins.Exceptions
{
    [Serializable]
    public class MapperImageNotFoundException : Exception
    {
        public string ContainerName { get; }

        public MapperImageNotFoundException(string containerName, string message) : base(message)
        {
            ContainerName = containerName;
        }

        public override string ToString()
        {
            return $"{Message}: {ContainerName} {Environment.NewLine} {base.ToString()}";
        }
    }
}