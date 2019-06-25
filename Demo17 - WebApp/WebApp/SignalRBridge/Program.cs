using System;
using Topshelf;

namespace SignalRBridge
{
    class Program
    {
        static void Main(string[] args)
        {
            var hf = HostFactory.Run(x =>
            {
                x.Service<BridgeService>();
            });

            var exitCode = (int)Convert.ChangeType(hf, hf.GetTypeCode());  //11
            Environment.ExitCode = exitCode;
        }
    }
}
