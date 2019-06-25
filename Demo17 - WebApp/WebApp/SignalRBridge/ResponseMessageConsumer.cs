using MassTransit;
using MessageContracts;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace SignalRBridge
{
    internal class ResponseMessageConsumer : IConsumer<ResponseMessage>
    {
        private readonly IHubContext _hub;

        public ResponseMessageConsumer()
        {
            _hub = GlobalHost.ConnectionManager.GetHubContext<ResponseMessageHub>();
        }
        public Task Consume(ConsumeContext<ResponseMessage> context)
        {
            System.Console.WriteLine($"Message received: {context.Message.Answer}");
            _hub.Clients.Client(context.Message.To).SendAnswer(context.Message.Answer);
            return Task.CompletedTask;
        }
    }
}