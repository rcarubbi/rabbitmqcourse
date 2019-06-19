using MassTransit;
using MassTransit.Messages;
using System;
using System.Threading.Tasks;

namespace MassTransitConsumer
{
    class MessageConsumer : IConsumer<Message>
    {
        public Task Consume(ConsumeContext<Message> context)
        {
            Console.WriteLine($"Message received by consumer {context.Message.Text}");
            return Task.CompletedTask;
        }
    }
}
