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

// MSCHART 已经引用
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
        List<byte> ADC_Data_Stroage_Raw = new List<byte>();
        List<int> ADC_Data_Stroage_Code = new List<int>();
        int BytesPerCode;
        int AdcDataSize;

        const string CMD_OPEN_STR = "OPEN;";
        const string CMD_DATW_STR = "DATW;";
        const string CMD_DATR_STR = "DATR;";
        const string CMD_REGW_STR = "REGW;";
        const string CMD_REGR_STR = "REGR;";
        const string CMD_REGM_STR = "REGM;";
        const string CMD_TASK1RUN_STR = "TASK1.RUN;";
        const string CMD_TASK1COMM_STR = "TASK1.COMM;";
        const string CMD_TASK1PACK_STR = "TASK1.PACK;";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitAdcSettings();
            AdcPrimarySettingsListBox.ItemsSource = AdcSettings.Select(t => t.ConfigName).ToList();
            AdcPrimarySettingsListBox.SelectedIndex = 0;
            UpdateAdcSecondarySettingsListBox();

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

        // myPort.ReceivedBytesThreshold
        private void MyPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (!isBinData)
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
                            ADC_Data_Stroage_Raw = new List<byte>(hex_bytes);
                            recvDataStr_tmp += recvDataStr.Substring(hex_end + 1, recvDataStr.Length - hex_end - 1);
                            recvDataStr = recvDataStr_tmp;
                        }

                        SerialPortCommInfoTextBox_Update(false, recvDataStr);
                    }
                }
                else
                {
                    /*
                    string str = "";
                    byte[] recvDataPackage = new byte[packet_size];
                    myPort.ReadTimeout=500;
                    int data_len = myPort.Read(recvDataPackage, 0, packet_size);
                    ADC_Data_Stroage_Raw = new List<byte>(recvDataPackage);
                    str += ("[WPF]: Read " + data_len + "/"+ packet_size + " Bytes in Packet.\n");
                    str += ToHexStrFromByte(recvDataPackage);
                    str += "\n";
                    SerialPortCommInfoTextBox_Update(false, str);
                    isBinData = false;
                    */
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

        private void CmdREGMButton_Click(object sender, RoutedEventArgs e)
        {
            string[] regBitsModifyPosStr = cmdREGMTextBox2.Text.Split(new char[] { ':' });
            int[] regBitsModifyPos = new int[] {
                Convert.ToInt32(regBitsModifyPosStr[0] ),
                Convert.ToInt32(regBitsModifyPosStr[1])};
            int regBitsMSB = Math.Max(regBitsModifyPos[0], regBitsModifyPos[1]);
            int regBitsLSB = Math.Min(regBitsModifyPos[0], regBitsModifyPos[1]);
            int regBitsLen = regBitsMSB - regBitsLSB + 1;
            // Modify REG21[10:0]=0x180
            //  REGM;21;0;11;180;
            string TxString = CMD_REGM_STR
                + cmdREGMTextBox1.Text + ";"
                 + regBitsLSB + ";" + regBitsLen + ";"
                 + cmdREGMTextBox3.Text + ";";
            SerialPortStringSendFunc(TxString);
        }

        private void CmdTASK1RUNButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_TASK1RUN_STR + cmdTASK1RUNTextBox1.Text + ";";
            SerialPortStringSendFunc(TxString);
        }
        private void CmdTASK1COMMButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_TASK1COMM_STR;
            SerialPortStringSendFunc(TxString);
        }
        private void CmdTASK1PACKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: Change Value Name of isBinData
                isBinData = true;
                AdcDataSize = Convert.ToInt32(cmdTASK1RUNTextBox1.Text);
                BytesPerCode = Convert.ToInt32(bytesPerCodeTextBox.Text);

                string TxString = CMD_TASK1PACK_STR;
                SerialPortStringSendFunc(TxString);

                string str = "";
                int recvDataPackageSize = AdcDataSize * BytesPerCode;
                byte[] recvDataPackage = new byte[recvDataPackageSize];

                // TIMEOUT when no bytes
                myPort.ReadTimeout = 500;
                // TIMEOUT when transmission
                // Stop Program and Keep UI alive.
                Thread t = new Thread(o => Thread.Sleep(500));
                t.Start(this);
                while (t.IsAlive)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                    if (myPort.BytesToRead < recvDataPackageSize)
                        ;//TODO
                    else
                        t.Abort();
                }
                int data_len = myPort.Read(recvDataPackage, 0, recvDataPackageSize);
                ADC_Data_Stroage_Raw = new List<byte>(recvDataPackage);
                str += ("[WPF]: Read " + data_len + "/" + recvDataPackageSize + " Bytes in Packet.\n");
                str += ToHexStrFromByte(recvDataPackage);
                str += "\n";
                SerialPortCommInfoTextBox_Update(false, str);

                isBinData = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void CmdLoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Open File Dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open Commands File...";
            openFileDialog.Filter = "Text File|*.txt";
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

            // AutoRun
            AutoRunCmds(txtArray);
        }
        private void WriteCmdsFromTextBoxButton_Click(object sender, RoutedEventArgs e)
        {
            // AutoRun
            AutoRunCmds(writeCmdsTextBox.Text.Split(new char[] { '\r', '\n' }));
        }

        private void SaveCmdsToFileButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            // Save File Dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Commands File...";
            saveFileDialog.Filter = "Text File|*.txt";
            if (saveFileDialog.ShowDialog() == false)
                return;
            FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create);

            fs.Write(System.Text.Encoding.Default.GetBytes(writeCmdsTextBox.Text), 0, writeCmdsTextBox.Text.Length);

            fs.Flush();
            fs.Close();
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
                    //str_buf += isTx ? "Tx: " : "Rx: ";
                    str_buf += isTx ? "[WPF]: " : "";
                    str_buf += str[i];
                    str_buf += isTx ? "\n" : "";
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

        public static string MidStrEx(string sourse, string startstr, string endstr)
        {
            string result = string.Empty;
            int startindex, endindex;
            try
            {
                startindex = sourse.IndexOf(startstr);
                if (startindex == -1)
                    return result;
                string tmpstr = sourse.Substring(startindex + startstr.Length);
                endindex = tmpstr.IndexOf(endstr);
                if (endindex == -1)
                    return result;
                result = tmpstr.Remove(endindex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return result;
        }

        private void tmpUpdate_Click(object sender, RoutedEventArgs e)
        {
            ADC_Data_Stroage_Code.Clear();
            for (int i = 0; i < ADC_Data_Stroage_Raw.Count / 4; i++)
                ADC_Data_Stroage_Code.Add(System.BitConverter.ToInt32(ADC_Data_Stroage_Raw.ToArray(), i * 4));
            tmpCode.Text = "";
            for (int i = 0; i < ADC_Data_Stroage_Code.Count; i++)
            {
                tmpCode.Text += ADC_Data_Stroage_Code[i];
                tmpCode.Text += "\n";
            }
        }

        private void AutoRunCmds(string[] txtArray)
        {
            // Comments
            for (int i = 0; i < txtArray.Length; i++)
            {
                int comment_symbol_index = txtArray[i].IndexOf("#");
                if (comment_symbol_index < 0)
                    continue;
                txtArray[i] = txtArray[i].Substring(0, comment_symbol_index);
            }
            // Delete empty elememts
            txtArray = txtArray.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            // AutoRunCommands
            for (int i = 0; i < txtArray.Length; i++)
            {
                int sleepTime = Convert.ToInt32(cmdIntervalTextBox.Text);
                if (txtArray[i].Contains("<delay>") && txtArray[i].Contains("</delay>"))
                {
                    // GUI Delay
                    sleepTime = Convert.ToInt32(MidStrEx(txtArray[i], "<delay>", "</delay>"));
                }
                else if (txtArray[i].Contains("<msgBox>") && txtArray[i].Contains("</msgBox>"))
                {
                    // MessageBox
                    string msgBoxStr = MidStrEx(txtArray[i], "<msgBox>", "</msgBox>");
                    MessageBox.Show(msgBoxStr);
                }
                else if (txtArray[i].Contains("<log>") && txtArray[i].Contains("</log>"))
                {
                    // Log Print
                    string logStr = MidStrEx(txtArray[i], "<log>", "</log>");
                    SerialPortCommInfoTextBox_Update(true, logStr);
                }
                else
                {
                    // Tramsit Command
                    SerialPortStringSendFunc(txtArray[i]);
                    //Thread.Sleep(sleepTime);
                }
                // Stop Program and Keep UI alive.
                Thread t = new Thread(o => Thread.Sleep(sleepTime));
                t.Start(this);
                while (t.IsAlive)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
            }
        }

    }
}