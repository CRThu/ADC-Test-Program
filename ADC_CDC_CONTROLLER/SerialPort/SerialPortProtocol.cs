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

        public string SerialPortCommandWrite(params string[] @params)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string param in @params)
            {
                stringBuilder.Append(param);
                stringBuilder.Append(";");
            }
            string command = stringBuilder.ToString();
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
        public string SerialPortTaskCommandWrite(string commandString)
        {
            try
            {
                serialPort.Write(commandString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return commandString;
        }
    }
}
