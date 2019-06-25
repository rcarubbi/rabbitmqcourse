using System.Threading.Tasks;
using MassTransit;
using MessageContracts;

namespace Processor
{
    public class RequestMessageConsumer : IConsumer<RequestMessage>
    {
        public async Task Consume(ConsumeContext<RequestMessage> context)
        {
            System.Console.WriteLine($"Message received: {context.Message.Text}");
            await context.Publish<ResponseMessage>(new { To = context.Message.To, Answer = $"Answer to {context.Message.Text}" });
        }
    }
}
