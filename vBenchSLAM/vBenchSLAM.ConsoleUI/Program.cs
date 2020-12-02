using System;
using System.Threading.Tasks;
using vBenchSLAM.Core;
using vBenchSLAM.Core.DockerCore;

namespace vBenchSLAM.ConsoleUI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Settings.Initialize();
                Console.WriteLine("Hello World from Docker!");
                IDockerManager manager = new DockerManager();
                var containers = await manager.ListContainersAsync();
                foreach (var container in containers)
                {
                    Console.WriteLine(container.Image);
                }
                var run = await manager.StartContainerAsync(containers[0].ID);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadLine();
        }
    }
}
