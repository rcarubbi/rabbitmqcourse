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
            var headers = new Dictionary<string, string>();
            var messageSender = new RabbitSender();

            headers.Add("material", GetComboItem(materialsComboBox));
            headers.Add("customertype", GetComboItem(customerTypeComboBox));

            var message = string.Format("Message: {0}", messageCount);

            messageSender.Send(message, headers);

            MessageBox.Show(string.Format("Sending Message - {0}", message), "Message sent");

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
