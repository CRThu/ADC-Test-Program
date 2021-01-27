using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

// TODO:图表 https://github.com/Microsoft/InteractiveDataDisplay.WPF

namespace ADC_CDC_CONTROLLER
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort myPort = new SerialPort();
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
                try
                {
                    myPort.Close();
                    serialPortStatusLabel.Content = myPort.IsOpen ? "Connected" : "Disconnected";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
                MessageBox.Show("COM has been closed!");
        }

        private void MyPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int _bytesToRead = myPort.BytesToRead;
                if (_bytesToRead > 0)
                {
                    byte[] recvData = new byte[_bytesToRead];

                    myPort.Read(recvData, 0, _bytesToRead);
                    string recvDataStr = System.Text.Encoding.ASCII.GetString(recvData);

                    if (recvDataStr.IndexOf("<dat>") >= 0)
                    {
                        int hex_start = GetIndexOf(recvData, System.Text.Encoding.ASCII.GetBytes("<dat>")) + "<dat>".Length;
                        int hex_end = GetIndexOf(recvData, System.Text.Encoding.ASCII.GetBytes("</dat>")) - 1;

                        string recvDataStr_tmp = "";
                        recvDataStr_tmp += recvDataStr.Substring(0, hex_start);
                        // HEX
                        //recvDataStr_tmp += recvDataStr.Substring(hex_start, hex_end-hex_start+1);
                        byte[] hex_bytes = recvData.Skip(hex_start).Take(hex_end - hex_start + 1).ToArray();
                        recvDataStr_tmp += ToHexStrFromByte(hex_bytes);

                        recvDataStr_tmp += recvDataStr.Substring(hex_end + 1, recvDataStr.Length - hex_end - 1);
                        recvDataStr = recvDataStr_tmp;
                    }

                    SerialPortCommInfoTextBox_Update(false, recvDataStr);
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

        private void CmdDATRButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_DATR_STR;
            SerialPortStringSendFunc(TxString);
        }

        private void CmdREGWButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_REGW_STR + cmdREGWTextBox1.Text + ";" + cmdREGWTextBox2.Text + ";";
            SerialPortStringSendFunc(TxString);
        }

        private void CmdREGRButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_REGR_STR + cmdREGRTextBox1.Text + ";";
            SerialPortStringSendFunc(TxString);
        }
        private void CmdRUN1Button_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_RUN1_STR + cmdRUN1TextBox1.Text + ";";
            SerialPortStringSendFunc(TxString);
        }
        private void CmdRUN1AndSaveButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_RUN1_STR + cmdRUN1TextBox1.Text + ";";
            SerialPortStringSendFunc(TxString);
        }
        private void CmdLoadFromFileButton_Click(object sender, RoutedEventArgs e)
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
            for (int i = 0; i < txtArray.Length; i++)
            {
                SerialPortStringSendFunc(txtArray[i]);
                int sleepTime = Convert.ToInt32(cmdIntervalTextBox.Text);
                //Thread.Sleep(Convert.ToInt32(cmdIntervalTextBox.Text));
                // Stop Program and Keep UI alive.
                Thread t = new Thread(o => Thread.Sleep(sleepTime));
                t.Start(this);
                while (t.IsAlive)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
            }

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
                    // ASCII
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

        public int GetIndexOf(byte[] b, byte[] bb)
        {
            if (b == null || bb == null || b.Length == 0 || bb.Length == 0 || b.Length < bb.Length)
                return -1;
            int i, j;
            for (i = 0; i < b.Length - bb.Length + 1; i++)
            {
                if (b[i] == bb[0])
                {
                    for (j = 1; j < bb.Length; j++)
                    {
                        if (b[i + j] != bb[j])
                            break;
                    }
                    if (j == bb.Length)
                        return i;
                }
            }
            return -1;
        }

    }
}
