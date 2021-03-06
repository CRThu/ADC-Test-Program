using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ADC_CDC_CONTROLLER
{
    class SerialPortProtocol : SerialPortComm
    {
        public SerialPortProtocol() : base()
        {

        }

        public SerialPortProtocol(string portName, int baudRate) : base(portName, baudRate)
        {

        }

        public string SerialPortCommandParamsWrite(params string[] @params)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string param in @params)
            {
                stringBuilder.Append(param);
                stringBuilder.Append(";");
            }
            string command = stringBuilder.ToString();

            SerialPortCommandLineWrite(command);

            return command;
        }
        public string SerialPortCommandLineWrite(string command)
        {
            try
            {
                serialPort.Write(command);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return command;
        }
    }
}
