using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace Client
{
    public class RabbitSender : IDisposable
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "Demo8";
        private const bool IsDurable = false;
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
            Console.WriteLine("QueueName: {0}", QueueName);
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

            //Create dynamic response queue
            _responseQueue = _model.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(_model);

            _consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties != null && ea.BasicProperties.CorrelationId == _correlationId)
                {
                    var body = ea.Body;
                    var response = Encoding.UTF8.GetString(body);

                    if (!_responses.TryAdd(response, 60000))
                    {
                        throw new TimeoutException();
                    }
                }
            };

        }

        public string Send(string message, TimeSpan timeout)
        {
            _responses = new BlockingCollection<string>();

            _correlationId = Guid.NewGuid().ToString();

            props = _model.CreateBasicProperties();
            props.CorrelationId = _correlationId;
            props.ReplyTo = _responseQueue;

            var messageBytes = Encoding.UTF8.GetBytes(message);

            _model.BasicPublish(
                exchange: "",
                routingKey: QueueName,
                basicProperties: props,
                body: messageBytes);

            _model.BasicConsume(
                consumer: _consumer,
                queue: _responseQueue,
                autoAck: true);

            if (_responses.TryTake(out string responseMessage, 60000)) {
                return responseMessage;
            } else {
                throw new TimeoutException();
            }
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
