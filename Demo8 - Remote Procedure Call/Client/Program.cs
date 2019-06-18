using System;

namespace Client
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ Message Sender");
            Console.WriteLine();
            Console.WriteLine();

            var messageCount = 0;
            var sender = new RabbitSender();

            Console.WriteLine("Press enter key to send a message");
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q)
                    break;

                if (key.Key != ConsoleKey.Enter) continue;
                var message = string.Format("Message: {0}", messageCount);
                Console.WriteLine("Sending - {0}", message);
                    
                var response = sender.Send(message, new TimeSpan(0, 0, 3, 0));

                Console.WriteLine("Response - {0}", response);
                messageCount++;
            }
            
            Console.ReadLine();
        }
    }
}
