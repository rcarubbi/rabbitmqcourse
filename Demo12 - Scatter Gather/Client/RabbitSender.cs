using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Client
{
    public class RabbitSender : IDisposable
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const bool IsDurable = false;
        private const string ExchangeName = "Demo12.Exchange";
        //The two below settings are just to illustrate how they can be used but we are not using them in
        //this sample as we will use the defaults
        private const string VirtualHost = "";
        private int Port = 0;

        private IConnection _connection;
        private IModel _model;
        private string _responseQueue;
        private EventingBasicConsumer _consumer;
        private BlockingCollection<string> _responses;
        private IBasicProperties props;
        private string _correlationId;

        /// <summary>
        /// Ctor
        /// </summary>
        public RabbitSender()
        {
            DisplaySettings();
            SetupRabbitMq();
        }

        private void DisplaySettings()
        {
            Console.WriteLine("Host: {0}", HostName);
            Console.WriteLine("Username: {0}", UserName);
            Console.WriteLine("Password: {0}", Password);
            Console.WriteLine("VirtualHost: {0}", VirtualHost);
            Console.WriteLine("Port: {0}", Port);
            Console.WriteLine("Is Durable: {0}", IsDurable);
        }
        /// <summary>
        /// Sets up the connections for rabbitMQ
        /// </summary>
        private void SetupRabbitMq()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password
            };

            if (string.IsNullOrEmpty(VirtualHost) == false)
                connectionFactory.VirtualHost = VirtualHost;
            if (Port > 0)
                connectionFactory.Port = Port;

            _connection = connectionFactory.CreateConnection();
            _model = _connection.CreateModel();

            _responseQueue = _model.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(_model);

            _consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties != null && ea.BasicProperties.CorrelationId == _correlationId)
                {
                    var body = ea.Body;
                    var response = Encoding.UTF8.GetString(body);

                    Console.WriteLine("Sender got response: {0}", response);

                    if (!_responses.TryAdd(response, 60000))
                    {
                        throw new TimeoutException();
                    }
                }
            };
        }

        public List<string> Send(string message, string routingKey, TimeSpan timeout, int minResponses)
        {
            _responses = new BlockingCollection<string>();

            _correlationId = Guid.NewGuid().ToString();

            props = _model.CreateBasicProperties();
            props.CorrelationId = _correlationId;
            props.ReplyTo = _responseQueue;

            var messageBytes = Encoding.UTF8.GetBytes(message);

            _model.BasicPublish(
                exchange: ExchangeName,
                routingKey: routingKey,
                basicProperties: props,
                body: messageBytes);

            _model.BasicConsume(
                consumer: _consumer,
                queue: _responseQueue,
                autoAck: true);

            var timeoutAt = DateTime.Now + timeout;

            //Wait for response
            while (DateTime.Now <= timeoutAt)
            {
                //No more messages on queue at present so if we have already got the minimum expected responses then
                //lets just return those
                if (_responses.Count >= minResponses)
                    return _responses.ToList();

                Console.WriteLine("Waiting for responses");
                Thread.Sleep(new TimeSpan(0, 0, 0, 0, 200));
            }

            return _responses.ToList();
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_connection != null)
                _connection.Close();

            if (_model != null && _model.IsOpen)
                _model.Abort();

            GC.SuppressFinalize(this);
        }
    }
}
