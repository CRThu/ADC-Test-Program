using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ADC_CDC_CONTROLLER
{
    class LoggerControl : Logger
    {
        Logger logger = new Logger();
        private TextBox textBox;
        public LoggerControl(TextBox textBox) : base()
        {
            this.textBox = textBox;
            //this.textBox.DataContext = this;
        }

        public void SerialPortLoggerTextBox_Update(bool isTx, string str)
        {
            // Dispatcher.Invoke()
            // serialPortCommInfoTextBox.Text += str_buf;

            textBox.Dispatcher.Invoke(new System.Action(() =>
            {
                string str_buf = "";
                // ASCII
                //str_buf += isTx ? "Tx: " : "Rx: ";
                str_buf += isTx ? "[WPF]: " : "";
                str_buf += str;
                str_buf += isTx ? "\n" : "";

                this.LogText += str_buf;

                // Too long
                if (textBox.Text.Length > 150000)
                {
                    textBox.Text = ".....\n";
                }

                textBox.AppendText(str_buf);
                textBox.SelectionStart = textBox.Text.Length;
                textBox.ScrollToEnd();
            }));
        }
    }
}
