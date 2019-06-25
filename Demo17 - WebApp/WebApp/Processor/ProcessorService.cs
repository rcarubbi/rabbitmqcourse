using MassTransit;
using System;
using Topshelf;

namespace Processor
{
    class ProcessorService : ServiceControl
    {
        private readonly IBusControl _bus;

        public ProcessorService()
        {
            _bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
             {
                 var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                 {
                     h.Username("guest");
                     h.Password("guest");
                 });

                 sbc.ReceiveEndpoint(host, "ProcessorService", ep =>
                 {
                     ep.Consumer<RequestMessageConsumer>();
                 });
             });
            
        }


        public bool Start(HostControl hostControl)
        {
            try
            {
                _bus.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            try
            {
                _bus.Stop();
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
