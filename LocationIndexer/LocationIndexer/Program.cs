using LocationIndexer.LocationBuilders;
using System;

namespace LocationIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            var runtimeParameters = new RuntimeParameters(args);
            var environmentContext = new EnvironmentContext(runtimeParameters);
            var globalContext = new GlobalContext(environmentContext);
            var processor = new LocationProcessor(globalContext);
            processor.Process();

            Console.ReadLine();
        }
    }
}
