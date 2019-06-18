using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinClient
{
    public partial class Form1 : Form
    {
        private int messageCount;

        public Form1()
        {
            InitializeComponent();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            var topics = new List<string>();
            var messageSender = new RabbitSender();

            topics.Add(GetComboItem(customerTypeComboBox));
            topics.Add(GetComboItem(orderSizeComboBox));
            topics.Add(GetComboItem(productComboBox));

            var message = string.Format("Message: {0}", messageCount);            

            var routingkey = messageSender.Send(message, topics);

            MessageBox.Show(string.Format("Sending Message - {0}, Routing Key - {1}", message, routingkey), "Message sent");

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
