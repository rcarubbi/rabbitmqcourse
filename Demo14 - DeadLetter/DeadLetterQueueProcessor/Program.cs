﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DeadLetterQueueProcessor
{
    class Program
    {        
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string DeadLetterQueueName = "Demo14.DeadLetter";


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

            consumer.Received += (m, deliveryArgs) => {
                //Serialize message
                var message = Encoding.Default.GetString(deliveryArgs.Body);

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Message Recieved - {0}", message);

                //Change message
                Console.WriteLine("Changing message to be 1");
                message = "1";

                //Resend message                
                var properties = model.CreateBasicProperties();
                properties.Persistent = true;
                byte[] messageBuffer = Encoding.Default.GetBytes(message);

                var resubmitQueue = GetQueue(deliveryArgs);
                model.BasicPublish("", resubmitQueue, properties, messageBuffer);

                //Ack message from Dead Letter Queue
                model.BasicAck(deliveryArgs.DeliveryTag, false);
            };
          
            model.BasicConsume(DeadLetterQueueName, false, consumer);

        }

        private static string GetQueue(BasicDeliverEventArgs deliveryArgs)
        {
            if (deliveryArgs.BasicProperties.Headers == null)
                return string.Empty;

            var header = deliveryArgs.BasicProperties.Headers["x-death"];
            if (header == null)
                return string.Empty;

            var xDeathHeader = header as List<object>;
            if (xDeathHeader == null || xDeathHeader.Count < 1)
                return string.Empty;

            var properties = xDeathHeader[0] as Dictionary<string, object>;
            if (properties == null || properties.Count < 1)
                return string.Empty;

            if (properties.ContainsKey("queue"))
            {
                var queueBytes = properties["queue"] as byte[];
                return Encoding.Default.GetString(queueBytes);               
            }

            return string.Empty;
        }
        /// <summary>
        /// Displays the rabbit settings
        /// </summary>
        private static void DisplaySettings()
        {
            Console.WriteLine("Host: {0}", HostName);
            Console.WriteLine("Username: {0}", UserName);
            Console.WriteLine("Password: {0}", Password);
            Console.WriteLine("Dead Letter QueueName: {0}", DeadLetterQueueName);            
        }
    }
}
