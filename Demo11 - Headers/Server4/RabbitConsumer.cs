using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using System.Collections.Generic;

namespace Server4
{
    /// <summary>
    /// Class to encapsulate recieving messages from RabbitMQ
    /// </summary>
    public class RabbitConsumer : IDisposable
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "Demo11.Queue4";
        private const bool IsDurable = true;
        //The two below settings are just to illustrate how they can be used but we are not using them in
        //this sample as we will use the defaults
        private const string VirtualHost = "";
        private int Port = 0;

        public delegate void OnReceiveMessage(string message);

        public bool Enabled { get; set; }
    
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _model;
        private Subscription _subscription;
        

        /// <summary>
        /// Ctor with a key to lookup the configuration
        /// </summary>
        public RabbitConsumer()
        {
            DisplaySettings();
            _connectionFactory = new ConnectionFactory
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password
            };

            if (string.IsNullOrEmpty(VirtualHost) == false)
                _connectionFactory.VirtualHost = VirtualHost;
            if (Port > 0)
                _connectionFactory.Port = Port;

            _connection = _connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            _model.BasicQos(0, 1, false);
        }
        /// <summary>
        /// Displays the rabbit settings
        /// </summary>
        private void DisplaySettings()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Host: {0}", HostName);
            Console.WriteLine("Username: {0}", UserName);
            Console.WriteLine("Password: {0}", Password);
            Console.WriteLine("QueueName: {0}", QueueName);
            Console.WriteLine("VirtualHost: {0}", VirtualHost);
            Console.WriteLine("Port: {0}", Port);
            Console.WriteLine("Is Durable: {0}", IsDurable);
            Console.WriteLine();
            Console.WriteLine();
        }
        /// <summary>
        /// Starts receiving a message from a queue
        /// </summary>
        public void Start()
        {
            _subscription = new Subscription(_model, QueueName, false);

            var consumer = new ConsumeDelegate(Poll);
            consumer.Invoke();
        }
        private delegate void ConsumeDelegate();

        private void Poll()
        {
            while (Enabled)
            {
                //Get next message
                var deliveryArgs = _subscription.Next();
                //Deserialize message
                var message = Encoding.Default.GetString(deliveryArgs.Body);

                //Handle Message
                Console.WriteLine("Message Recieved - {0}", message);
                foreach (var headerKey in deliveryArgs.BasicProperties.Headers.Keys)
                {
                    var headerValue = deliveryArgs.BasicProperties.Headers[headerKey];
                    var val = Encoding.Default.GetString((byte[])headerValue);
                    Console.WriteLine("Header - Key: {0}, Value: {1}", headerKey, val);
                }
                Console.WriteLine();

                //Acknowledge message is processed
                _subscription.Ack(deliveryArgs);
            }
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_model != null)
                _model.Dispose();
            if (_connection != null)
                _connection.Dispose();

            _connectionFactory = null;

            GC.SuppressFinalize(this);
        }
    }
}
