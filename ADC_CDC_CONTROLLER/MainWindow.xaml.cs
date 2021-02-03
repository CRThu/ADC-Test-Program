﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using static ADC_CDC_CONTROLLER.Util;

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
        bool isDataReceivedRefused = false;
        string recvDataStr;
        bool isWaitingSignal = false;
        string ReceivedSignalStr = "";
        List<byte> ADC_Data_Stroage_Raw = new List<byte>();
        Dictionary<string, List<int>> AdcDataStorageDictionary = new Dictionary<string, List<int>>();

        const string CMD_OPEN_STR = "OPEN;";
        const string CMD_RESET_STR = "RESET;";
        const string CMD_DATW_STR = "DATW;";
        const string CMD_DATR_STR = "DATR;";
        const string CMD_REGW_STR = "REGW;";
        const string CMD_REGR_STR = "REGR;";
        const string CMD_REGM_STR = "REGM;";
        const string CMD_TASK1RUN_STR = "TASK1.RUN;";
        const string CMD_TASK1COMM_STR = "TASK1.COMM;";
        const string CMD_TASK1COMMNEW_STR = "TASK1.COMMNEW;";
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
                myPort.NewLine = "\n";
                myPort.ReadBufferSize = Convert.ToInt32(rxBufSizeTextBox.Text);
                myPort.ReadTimeout = Convert.ToInt32(rxTimeOutTextBox.Text);
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
                // Refuse DataReceived Process
                if (!isDataReceivedRefused)
                {
                    int _bytesToRead = myPort.BytesToRead;
                    if (_bytesToRead > 0)
                    {
                        byte[] recvData = new byte[_bytesToRead];

                        myPort.Read(recvData, 0, _bytesToRead);
                        // TODO 重构
                        recvDataStr = System.Text.Encoding.ASCII.GetString(recvData);

                        if (isWaitingSignal)
                        {
                            if (recvDataStr.IndexOf("[COMMAND]") >= 0)
                            {
                                ReceivedSignalStr = recvDataStr;
                                isWaitingSignal = false;
                            }
                        }
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

        private void cmdRESETButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_RESET_STR;
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
            int AdcDataSize = Convert.ToInt32(cmdTASK1RUNTextBox1.Text);
            string TxString = CMD_TASK1COMM_STR + AdcDataSize + ";";
            SerialPortStringSendFunc(TxString);
        }

        private void CmdTASK1PACKButton_Click(object sender, RoutedEventArgs e)
        {
            int AdcDataSize = Convert.ToInt32(cmdTASK1RUNTextBox1.Text);
            int BytesPerCode = Convert.ToInt32(bytesPerCodeTextBox.Text);
            int timeout = Convert.ToInt32(rxTimeOutTextBox.Text);
            Task1PACKReceive(AdcDataSize, BytesPerCode, timeout);
        }
        private void Task1PACKReceive(int adcDataSize, int bytesPerCode, int timeout)
        {
            try
            {
                int recvDataPackageSize = adcDataSize * bytesPerCode;

                isDataReceivedRefused = true;
                string TxString = CMD_TASK1PACK_STR + adcDataSize + ";" + bytesPerCode + ";" + timeout + ";";
                SerialPortStringSendFunc(TxString);

                string str = "";
                byte[] recvDataPackage = new byte[recvDataPackageSize];

                // NEW SOLUTION:https://stackoverflow.com/questions/16439897/serialport-readbyte-int32-int32-is-not-blocking-but-i-want-it-to-how-do
                // Stop Program and Keep UI alive.
                /*
                Thread t = new Thread(o => Thread.Sleep(timeout));
                t.Start(this);
                while (t.IsAlive)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                    if (myPort.BytesToRead >= recvDataPackageSize)
                        t.Abort();
                }
                */
                int data_len = ReadDataBlock(recvDataPackage, recvDataPackageSize, timeout);

                ADC_Data_Stroage_Raw = new List<byte>(recvDataPackage);
                str += ("[WPF]: Read " + data_len + "/" + recvDataPackageSize + " Bytes in Packet.\n");
                str += ToHexStrFromByte(recvDataPackage);
                str += "\n";
                SerialPortCommInfoTextBox_Update(false, str);

                isDataReceivedRefused = false;

                if (myPort.BytesToRead > 0)
                    MyPort_DataReceived(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public int ReadDataBlock(byte[] responseBytes, int bytesExpected, int timeOut)
        {
            int offset = 0, bytesRead;
            myPort.ReadTimeout = timeOut;
            try
            {
                while (bytesExpected > 0 &&
                  (bytesRead = myPort.Read(responseBytes, offset, bytesExpected)) > 0)
                {
                    offset += bytesRead;
                    bytesExpected -= bytesRead;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return offset;
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
            //AutoRunCmds(txtArray);
            AutoRunCmds(txtArray, Path.GetDirectoryName(openFileDialog.FileName));
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

        private void tmpUpdate_Click(object sender, RoutedEventArgs e)
        {
            List<int> ADC_Data_Stroage_Code = new List<int>();

            for (int i = 0; i < ADC_Data_Stroage_Raw.Count / 4; i++)
                ADC_Data_Stroage_Code.Add(System.BitConverter.ToInt32(ADC_Data_Stroage_Raw.ToArray(), i * 4));

            string data_str = "";
            for (int i = 0; i < ADC_Data_Stroage_Code.Count; i++)
            {
                data_str += ADC_Data_Stroage_Code[i].ToString();
                data_str += "\n";
            }
            tmpCode.Text = data_str;
        }
        private void tmpUpdateList_Click(object sender, RoutedEventArgs e)
        {
            tmpCodeDicListBox.Items.Clear();
            foreach (var kv in AdcDataStorageDictionary)
                tmpCodeDicListBox.Items.Add(kv.Key);
        }
        private void tmpCodeDicListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (tmpCodeDicListBox.SelectedItem == null)
                return;

            string data_str = "";
            for (int i = 0; i < AdcDataStorageDictionary[(string)tmpCodeDicListBox.SelectedItem].Count; i++)
            {
                data_str += AdcDataStorageDictionary[(string)tmpCodeDicListBox.SelectedItem][i].ToString();
                data_str += "\n";
            }
            tmpCode.Text = data_str;
        }

        private void AutoRunCmds(string[] txtArray)
        {
            AutoRunCmds(txtArray, "./");
        }
        private void AutoRunCmds(string[] txtArray, string taskFileDir)
        {
            try
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
                        SerialPortCommInfoTextBox_Update(true, "Delay " + sleepTime + " ms...");
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
                    else if (txtArray[i].Contains("<waitCmd/>"))
                    {
                        // Wait For [COMMAND]: xxx
                        isWaitingSignal = true;
                        SerialPortCommInfoTextBox_Update(true, "Waiting For Signal");
                        while (isWaitingSignal)
                        {
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                        }
                        // Receive Signal
                        string str = ReceivedSignalStr.Replace(" ", "").Split(new char[] { ':' })[1];
                        SerialPortCommInfoTextBox_Update(true, "Get Signal:" + str);
                    }
                    else if (txtArray[i].Contains("TASK1.PACK"))
                    {
                        string[] param = txtArray[i].Split(new char[] { ';' });
                        int AdcDataSize = Convert.ToInt32(param[1]);
                        int BytesPerCode = Convert.ToInt32(param[2]);
                        int timeout = Convert.ToInt32(param[3]);
                        Task1PACKReceive(AdcDataSize, BytesPerCode, timeout);
                    }
                    else if (txtArray[i].Contains("<storeDic>") && txtArray[i].Contains("</storeDic>"))
                    {
                        List<int> AdcDataStroageCode = new List<int>();

                        for (int j = 0; j < ADC_Data_Stroage_Raw.Count / 4; j++)
                            AdcDataStroageCode.Add(System.BitConverter.ToInt32(ADC_Data_Stroage_Raw.ToArray(), j * 4));

                        string dicKeyStr = MidStrEx(txtArray[i], "<storeDic>", "</storeDic>");
                        if (!AdcDataStorageDictionary.ContainsKey(dicKeyStr))
                            AdcDataStorageDictionary.Add(dicKeyStr, AdcDataStroageCode);
                        else
                            AdcDataStorageDictionary[dicKeyStr] = AdcDataStroageCode;
                        SerialPortCommInfoTextBox_Update(true, "Stored data to dictionary:<Key.Name=" + dicKeyStr + ", Value.Size=" + AdcDataStroageCode.Count + ">");
                    }
                    else if (txtArray[i].Contains("<storeFile>") && txtArray[i].Contains("</storeFile>"))
                    {
                        List<int> AdcDataStroageCode = new List<int>();

                        for (int j = 0; j < ADC_Data_Stroage_Raw.Count / 4; j++)
                            AdcDataStroageCode.Add(System.BitConverter.ToInt32(ADC_Data_Stroage_Raw.ToArray(), j * 4));

                        string fileNameStr = MidStrEx(txtArray[i], "<storeFile>", "</storeFile>");
                        // WriteFile
                        FileStream fs = new FileStream(taskFileDir +@"\"+ fileNameStr, FileMode.Create);
                        foreach (int code in AdcDataStroageCode)
                        {
                            byte[] writeFileBytes = System.Text.Encoding.Default.GetBytes(code.ToString() + Environment.NewLine);
                            fs.Write(writeFileBytes, 0, writeFileBytes.Length);
                        }
                        fs.Flush();
                        fs.Close();

                        SerialPortCommInfoTextBox_Update(true, "Stored data to path:" + taskFileDir + @"\" + fileNameStr);
                    }
                    else
                    {
                        // Tramsit Command
                        SerialPortStringSendFunc(txtArray[i]);
                        //Thread.Sleep(sleepTime);
                    }
                    if (sleepTime > 0)
                    {
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}