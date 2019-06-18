using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Program
    {
        private const string RetryHeader = "RETRY-COUNT";
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "Demo13";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ queue processor");
            Console.WriteLine();
            Console.WriteLine();
            DisplaySettings();

            var connectionFactory = new ConnectionFactory
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password
            };

            var connection = connectionFactory.CreateConnection();
            var model = connection.CreateModel();
            model.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(model);
            model.BasicConsume(QueueName, false, consumer);

            consumer.Received += (m, deliveryArgs) =>
            {
                var message = Encoding.Default.GetString(deliveryArgs.Body);

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Message Recieved - {0}", message);

                if (message == "1")
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Acknowledging successful processing of message");
                    Console.ForegroundColor = ConsoleColor.White;
                    model.BasicAck(deliveryArgs.DeliveryTag, false);
                }
                else if (message == "2")
                {
                    var attempts = GetRetryAttempts(deliveryArgs.BasicProperties);
                    if (attempts < 3)
                    {
                        Console.WriteLine("Message is 2 so rejecting and requeuing message");
                        Console.WriteLine("Attempts made: {0}", attempts);

                        //Create retry message
                        attempts++;
                        var properties = model.CreateBasicProperties();
                        properties.Headers = CopyMessageHeaders(deliveryArgs.BasicProperties.Headers);
                        SetRetryAttempts(properties, attempts);

                        //Publish new updated message for retry
                        model.BasicPublish(deliveryArgs.Exchange, deliveryArgs.RoutingKey, properties, deliveryArgs.Body);

                        //Ack original message
                        model.BasicAck(deliveryArgs.DeliveryTag, false);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Message rejected for retry");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Message is 2 but has has already made {0} attempts so rejecting the message as retries exhausted", attempts);
                        Console.ForegroundColor = ConsoleColor.White;

                        //Reject message all retries used
                        model.BasicReject(deliveryArgs.DeliveryTag, false);
                    }
                }
                else if (message == "3")
                {
                    var attempts = GetRetryAttempts(deliveryArgs.BasicProperties);
                    if (attempts < 2)
                    {
                        Console.WriteLine("Message is 3 so rejecting and requeuing message");
                        Console.WriteLine("Attempts made: {0}", attempts);

                        attempts++;

                        //Create retry message
                        var properties = model.CreateBasicProperties();
                        properties.Headers = CopyMessageHeaders(deliveryArgs.BasicProperties.Headers);
                        SetRetryAttempts(properties, attempts);

                        //Publish retry message
                        model.BasicPublish(deliveryArgs.Exchange, deliveryArgs.RoutingKey, properties, deliveryArgs.Body);

                        //Ack original message
                        model.BasicAck(deliveryArgs.DeliveryTag, false);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Message rejected for retry");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("Message is 3 and {0} attempts have been made so this one will work successfully, and the message is acknowledged", attempts);
                        Console.ForegroundColor = ConsoleColor.White;

                        //Message Processed successfully so ack
                        model.BasicAck(deliveryArgs.DeliveryTag, false);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Message is >3 so rejecting and not requeuing message");
                    Console.ForegroundColor = ConsoleColor.White;

                    //Message scenario always rekected
                    model.BasicReject(deliveryArgs.DeliveryTag, false);
                }

            };


        }

        private static Dictionary<string, object> CopyMessageHeaders(IDictionary<string, object> existingProperties)
        {
            var newProperties = new Dictionary<string, object>();
            if (existingProperties != null)
            {
                var enumerator = existingProperties.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    newProperties.Add(enumerator.Current.Key, enumerator.Current.Value);
                }
            }
            return newProperties;
        }
        private static void SetRetryAttempts(IBasicProperties properties, int newAttempts)
        {
            if (properties.Headers.Keys.Contains(RetryHeader))
                properties.Headers[RetryHeader] = newAttempts;
            else
                properties.Headers.Add(RetryHeader, newAttempts);
        }
        private static int GetRetryAttempts(IBasicProperties properties)
        {
            if (properties.Headers == null || properties.Headers.Keys.Contains(RetryHeader) == false)
                return 0;

            var val = properties.Headers[RetryHeader];
            if (val == null)
                return 0;

            return Convert.ToInt32(val);
        }
        /// <summary>
        /// Displays the rabbit settings
        /// </summary>
        private static void DisplaySettings()
        {
            Console.WriteLine("Host: {0}", HostName);
            Console.WriteLine("Username: {0}", UserName);
            Console.WriteLine("Password: {0}", Password);
            Console.WriteLine("QueueName: {0}", QueueName);
        }
    }
}
