using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows;

// TODO:图表 https://github.com/Microsoft/InteractiveDataDisplay.WPF

namespace ADC_CDC_CONTROLLER
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort myPort = new SerialPort();
        bool isDataSaveFile = false;
        bool isBinData = false;

        const string CMD_OPEN_STR = "OPEN;";
        const string CMD_DATW_STR = "DATW;";
        const string CMD_DATR_STR = "DATR;";
        const string CMD_REGW_STR = "REGW;";
        const string CMD_REGR_STR = "REGR;";
        const string CMD_RUN1_STR = "RUN1;";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] portList = System.IO.Ports.SerialPort.GetPortNames();
            for (int i = 0; i < portList.Length; ++i)
            {
                string name = portList[i];
                serialPortComboBox1.Items.Add(name);
            }
            if (portList.Length > 0)
                serialPortComboBox1.SelectedIndex = 0;
        }

        private void SerialPortConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myPort.PortName = serialPortComboBox1.SelectedItem.ToString();
                myPort.BaudRate = 9600;
                myPort.DataBits = 8;
                myPort.NewLine = "\r\n";
                myPort.ReadBufferSize = Convert.ToInt32(rxBufSizeTextBox.Text);
                myPort.Open();
                serialPortStatusLabel.Content = myPort.IsOpen ? "Connected" : "Disconnected";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            myPort.DataReceived += MyPort_DataReceived;
        }

        private void SerialPortDisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (myPort.IsOpen)
            {
                myPort.Close();
                serialPortStatusLabel.Content = myPort.IsOpen ? "Connected" : "Disconnected";
            }
            else
                MessageBox.Show("COM has been closed!");
        }

        private void MyPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (isBinData)
                {
                    int _bytesToRead = myPort.BytesToRead;
                    if (_bytesToRead > 0)
                    {
                        byte[] recvData = new byte[_bytesToRead];

                        myPort.Read(recvData, 0, _bytesToRead);
                        string recvBinDataStr = ToHexStrFromByte(recvData);

                        SerialPortCommInfoTextBox_Update(false, recvBinDataStr);
                    }
                    isBinData = false;
                }
                else
                {
                    int _bytesToRead = myPort.BytesToRead;
                    if (_bytesToRead > 0)
                    {
                        byte[] recvData = new byte[_bytesToRead];

                        myPort.Read(recvData, 0, _bytesToRead);
                        string recvDataStr = System.Text.Encoding.ASCII.GetString(recvData);

                        SerialPortCommInfoTextBox_Update(false, recvDataStr);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CmdOPENButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_OPEN_STR;
            SerialPortStringSendFunc(TxString);
        }

        private void CmdDATWButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_DATW_STR + cmdDATWTextBox1.Text + ";";
            SerialPortStringSendFunc(TxString);
        }

        private void cmdDATRButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_DATR_STR;
            SerialPortStringSendFunc(TxString);
        }

        private void cmdREGWButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_REGW_STR + cmdREGWTextBox1.Text + ";" + cmdREGWTextBox2.Text + ";";
            SerialPortStringSendFunc(TxString);
        }

        private void cmdREGRButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_REGR_STR + cmdREGRTextBox1.Text + ";";
            SerialPortStringSendFunc(TxString);
        }
        private void cmdRUN1Button_Click(object sender, RoutedEventArgs e)
        {
            isBinData = true;
            string TxString = CMD_RUN1_STR + cmdRUN1TextBox1.Text + ";";
            SerialPortStringSendFunc(TxString);
        }
        private void cmdRUN1AndSaveButton_Click(object sender, RoutedEventArgs e)
        {
            isBinData = true;
            string TxString = CMD_RUN1_STR + cmdRUN1TextBox1.Text + ";";
            SerialPortStringSendFunc(TxString);
        }
        private void cmdLoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Open File Dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open Commands File...";
            openFileDialog.Filter = "Text File|*.txt";
            openFileDialog.FileName = string.Empty;
            if (openFileDialog.ShowDialog() == false)
                return;
            string txtFile = openFileDialog.FileName;
            cmdLoadFromFileTextBox.Text = openFileDialog.SafeFileName;

            // Open File
            List<string> txt = new List<string>();
            using (StreamReader sr = new StreamReader(txtFile, Encoding.Default))
            {
                int lineCount = 0;
                while (sr.Peek() > 0)
                {
                    lineCount++;
                    string temp = sr.ReadLine();
                    txt.Add(temp);
                }
            }
            string[] txtArray = txt.ToArray();

            // Commands
            for(int i=0;i<txtArray.Length;i++)
                SerialPortStringSendFunc(txtArray[i]);

        }
        private void SerialPortStringSendFunc(string str)
        {
            try
            {
                byte[] TxData = System.Text.Encoding.ASCII.GetBytes(str);
                myPort.Write(TxData, 0, TxData.Length);
                SerialPortCommInfoTextBox_Update(true, str);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void SerialPortCommInfoTextBox_Update(bool isTx, string str)
        {
            SerialPortCommInfoTextBox_Update(isTx, new string[] { str });
        }
        private void SerialPortCommInfoTextBox_Update(bool isTx, string[] str)
        {
            // Dispatcher.Invoke()
            // serialPortCommInfoTextBox.Text += str_buf;
            serialPortCommInfoTextBox.Dispatcher.Invoke(new System.Action(() =>
            {
                string str_buf = "";
                for (int i = 0; i < str.Length; ++i)
                {
                    str_buf += isTx ? "Tx: " : "Rx: ";
                    str_buf += str[i];
                    str_buf += "\r\n";
                }
                serialPortCommInfoTextBox.Text += str_buf;
                serialPortCommInfoTextBox.ScrollToEnd();
            }));
        }
        public static string ToHexStrFromByte(byte[] byteDatas)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteDatas.Length; i++)
            {
                builder.Append(string.Format("{0:X2} ", byteDatas[i]));
            }
            return builder.ToString().Trim();
        }


    }
}
