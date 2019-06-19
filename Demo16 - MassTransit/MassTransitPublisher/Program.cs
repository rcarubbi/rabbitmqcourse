using MassTransit;
using MassTransit.Messages;
using System;

namespace MassTransitPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });

            bus.Start(); 

            while (true)
            {
                var text = Console.ReadLine();
                if (text == "q")
                    break;

                bus.Publish<Message>(new { Text = text });
                Console.WriteLine($"Message {text} published");
               
            }

            bus.Stop();
        }
    }
}
