using System;
using Topshelf;

namespace Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            var hf = HostFactory.Run(x => x.Service<ProcessorService>());

            var exitCode = (int)Convert.ChangeType(hf, hf.GetTypeCode());  //11
            Environment.ExitCode = exitCode;
        }
    }
}
