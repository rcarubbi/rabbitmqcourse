using System;
using System.Globalization;

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
            string routingKey;
            var sender = new RabbitSender();

            Console.WriteLine("Press enter key to send a message");
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q)
                    break;

                if (key.Key == ConsoleKey.Enter)
                {
                    //Used to generate a random routing key so server 1,2 or no one will get the message
                    routingKey = new Random().Next(0, 4).ToString(CultureInfo.InvariantCulture);

                    var message = string.Format("Message: {0} - Routing Key: {1}", messageCount, routingKey);
                    Console.WriteLine("Sending - {0}", message);
                    
                    sender.Send(message, routingKey);
                    messageCount++;
                }
            }
            
            Console.ReadLine();
        }
    }
}
