using MassTransit;
using Microsoft.Owin.Hosting;
using System;
using Topshelf;

namespace SignalRBridge
{
    class BridgeService : ServiceControl
    {
        private readonly IBusControl _bus;

        public BridgeService()
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
                    ep.Consumer<ResponseMessageConsumer>();
                });
            });
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                _bus.Start();
                string url = "http://localhost:8080";
                WebApp.Start(url);
                Console.WriteLine("Server running on {0}", url);
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
