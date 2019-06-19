using RabbitMQ.Client;
using System;
using System.Text;
using System.Windows.Forms;

namespace WinClient
{
    public partial class Form1 : Form
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string ExchangeName = "Demo15.Exchange";
        private int messageCount;

        public Form1()
        {
            InitializeComponent();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            #region Connect to RabbitMQ
            var connectionFactory = new ConnectionFactory
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password
            };

            var connection = connectionFactory.CreateConnection();
            var model = connection.CreateModel();
            #endregion
            
            var routingKey = GetComboItem(productComboBox);

            //Setup properties
            var properties = model.CreateBasicProperties();
            properties.Persistent = true;

            //Serialize
            byte[] messageBuffer = Encoding.Default.GetBytes(routingKey);            

            //Send message
            model.BasicPublish(ExchangeName, routingKey, properties, messageBuffer);

            MessageBox.Show(string.Format("Sending Message - Routing Key - {0}", routingKey), "Message sent");

            messageCount++;
        }

        private static string GetComboItem(ComboBox comboBox)
        {
            if (string.IsNullOrEmpty(comboBox.Text))
                return string.Empty;
            return comboBox.Text;
        }
    }
}
