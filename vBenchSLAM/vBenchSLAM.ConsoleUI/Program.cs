using System;
using System.Threading.Tasks;
using vBenchSLAM.Core;
using vBenchSLAM.Core.DockerCore;
using vBenchSLAM.Core.Enums;

namespace vBenchSLAM.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Settings.Initialize();

                IDockerManager manager = new DockerManager();

                var runner = new Runner(MapperTypeEnum.OpenVslam, manager);
                runner.Run();
                //var containers = await manager.ListContainersAsync();
                //foreach (var container in containers)
                //{
                //    manager.StopContainerAsync(container.ID);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();
        }
    }
}
