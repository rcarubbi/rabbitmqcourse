using RabbitMQ.Client;
using static System.Console;

namespace QueueCreator
{
    class Program
    {
        private static readonly string Password = "guest";
        private static readonly string UserName = "guest";
        private static readonly string HostName = "localhost";

        public static void Main(string[] args)
        {
            WriteLine("Starting RabbitMq Queue Creator\n\n");
            var connectionFactory = new ConnectionFactory()
            {
                Password = Password,
                UserName = UserName,
                HostName = HostName
            };

            var connection = connectionFactory.CreateConnection();
            var model = connection.CreateModel();

            model.QueueDeclare("MyQueue", true, false, false, null);
            WriteLine("Queue created");

            model.ExchangeDeclare("MyExchange", ExchangeType.Topic);
            WriteLine("Exchange created");

            model.QueueBind("MyQueue", "MyExchange", "cars");
            WriteLine("Exchange and queue bound");

            ReadKey();
        }
    }
}
