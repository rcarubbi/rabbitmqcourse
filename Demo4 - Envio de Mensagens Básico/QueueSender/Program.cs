using RabbitMQ.Client;
using System.Text;
using static System.Console;

namespace QueueSender
{
    class Program
    {
        private static readonly string Password = "guest";
        private static readonly string UserName = "guest";
        private static readonly string HostName = "localhost";
        private static readonly string QueueName = "Demo4";
        private static readonly string ExchangeName = "";

        public static void Main(string[] args)
        {
            WriteLine("Starting RabbitMq Message Sender\n\n");
            var connectionFactory = new ConnectionFactory()
            {
                Password = Password,
                UserName = UserName,
                HostName = HostName
            };

            var connection = connectionFactory.CreateConnection();
            var model = connection.CreateModel();

            var properties = model.CreateBasicProperties();
            properties.Persistent = false;

            // Serialize
            byte[] messageBuffer = Encoding.Default.GetBytes("this is my message");

            // Send message
            model.BasicPublish(ExchangeName, QueueName, properties, messageBuffer);

            WriteLine("Message Sent");
            ReadLine();
        }
    }
}