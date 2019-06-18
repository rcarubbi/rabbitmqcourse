using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ queue processor - Server 2");
            Console.WriteLine();
            Console.WriteLine();

            var queueProcessor = new RabbitConsumer() { Enabled = true };
            queueProcessor.Start();
            Console.ReadLine();
        }
    }
}
