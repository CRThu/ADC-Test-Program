﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static ADC_CDC_CONTROLLER.Util;

namespace ADC_CDC_CONTROLLER
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //SerialPort myPort = new SerialPort();

        bool isDataReceivedRefused = false;
        string recvDataStr;
        bool isWaitingSignal = false;
        string ReceivedSignalStr = "";
        int bytesPerCode;

        string adcCurrentSampleSettingInfoStr = "";
        Dictionary<string, string> adcCurrentSampleSettingInfo = new Dictionary<string, string>();
        AdcDataStorage adcDataStorage = new AdcDataStorage();

        const string CMD_OPEN_STR = "OPEN;";
        const string CMD_RESET_STR = "RESET;";
        const string CMD_DATW_STR = "DATW;";
        const string CMD_DATR_STR = "DATR;";
        const string CMD_REGW_STR = "REGW;";
        const string CMD_REGR_STR = "REGR;";
        const string CMD_REGM_STR = "REGM;";
        const string CMD_REGQ_STR = "REGQ;";
        const string CMD_TASK1RUN_STR = "TASK1.RUN;";
        const string CMD_TASK1COMM_STR = "TASK1.COMM;";
        const string CMD_TASK1PACK_STR = "TASK1.PACK;";

        SerialPortComm serialPortComm;
        Log log1 = new Log();

        private void SerialPortConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string serialPortName = serialPortComboBox1.SelectedItem.ToString().Split('|').First().Trim();
            int serialPortBaudRate = 9600;
            int serialPortRxBufSize = Convert.ToInt32(debugTabRxBufSizeTextBox.Text);
            int serialPortRxTimeout = Convert.ToInt32(debugTabRxTimeOutTextBox.Text);
            int serialPortTxBufSize = 2048;
            int serialPortTxTimeout = SerialPort.InfiniteTimeout;

            serialPortComm = new SerialPortComm(serialPortName, serialPortBaudRate);
            serialPortComm.SetBuffer(serialPortRxBufSize, serialPortRxTimeout, serialPortTxBufSize, serialPortTxTimeout);
            serialPortComm.Open();
            debugTabSerialPortStatusLabel.Content = serialPortComm.IsOpen ? "Connected" : "Disconnected";

            serialPortComm.DataReceivedEvent(MyPort_DataReceived);
        }

        private void SerialPortDisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (serialPortComm.IsOpen)
            {
                serialPortComm.Close();
                debugTabSerialPortStatusLabel.Content = serialPortComm.IsOpen ? "Connected" : "Disconnected";
            }
            else
                MessageBox.Show("Serial port has been closed!");
        }

        private void MyPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Refuse DataReceived Process
            if (!isDataReceivedRefused)
            {
                int _bytesToRead = serialPortComm.BytesToRead;
                if (_bytesToRead > 0)
                {
                    byte[] recvData = new byte[_bytesToRead];

                    serialPortComm.Read(recvData, _bytesToRead);
                    //serialPortComm.serialPort.Read(recvData, 0, _bytesToRead);

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
                        adcDataStorage.WriteTmpAdcSamples(adcCurrentSampleSettingInfoStr, hex_bytes.ToList(), bytesPerCode);
                        recvDataStr_tmp += recvDataStr.Substring(hex_end + 1, recvDataStr.Length - hex_end - 1);
                        recvDataStr = recvDataStr_tmp;
                    }

                    SerialPortLoggerTextBox_Update(false, recvDataStr);
                }
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
        private void CmdREGQButton_Click(object sender, RoutedEventArgs e)
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
            string TxString = CMD_REGQ_STR
                + cmdREGMTextBox1.Text + ";"
                 + regBitsLSB + ";" + regBitsLen + ";";
            SerialPortStringSendFunc(TxString);
        }

        private void CmdTASK1RUNButton_Click(object sender, RoutedEventArgs e)
        {
            string TxString = CMD_TASK1RUN_STR + cmdTASK1RUNTextBox1.Text + ";";
            SerialPortStringSendFunc(TxString);
        }

        private void CmdTASK1COMMButton_Click(object sender, RoutedEventArgs e)
        {
            bytesPerCode = Convert.ToInt32(bytesPerCodeTextBox.Text);
            int AdcDataSize = Convert.ToInt32(cmdTASK1RUNTextBox1.Text);
            string TxString = CMD_TASK1COMM_STR + AdcDataSize + ";";
            SerialPortStringSendFunc(TxString);
        }

        private void CmdTASK1PACKButton_Click(object sender, RoutedEventArgs e)
        {
            int AdcDataSize = Convert.ToInt32(cmdTASK1RUNTextBox1.Text);
            int BytesPerCode = Convert.ToInt32(bytesPerCodeTextBox.Text);
            int timeout = Convert.ToInt32(debugTabRxTimeOutTextBox.Text);
            Task1PACKReceive(AdcDataSize, BytesPerCode, timeout);
        }
        private void Task1PACKReceive(int adcDataSize, int bytesPerCode, int timeout)
        {
            int recvDataPackageSize = adcDataSize * bytesPerCode;

            isDataReceivedRefused = true;
            string TxString = CMD_TASK1PACK_STR + adcDataSize + ";";
            SerialPortStringSendFunc(TxString);

            string str = "";
            byte[] recvDataPackage = new byte[recvDataPackageSize];

            int data_len = serialPortComm.Read(recvDataPackage, recvDataPackageSize, timeout);

            //ADC_Data_Stroage_Raw = new List<byte>(recvDataPackage);
            adcDataStorage.WriteTmpAdcSamples(adcCurrentSampleSettingInfoStr, recvDataPackage.ToList(), bytesPerCode);
            str += ("Read " + data_len + "/" + recvDataPackageSize + " Bytes in Packet.\n");
            str += ToHexStrFromByte(recvDataPackage);
            SerialPortLoggerTextBox_Update(true, str);

            isDataReceivedRefused = false;

            if (serialPortComm.BytesToRead > 0)
                MyPort_DataReceived(null, null);
        }
        
        private void CmdLoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Open File Dialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Commands File...",
                Filter = "Text File|*.txt",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
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
            DebugTabAutoRunCmds(txtArray, Path.GetDirectoryName(openFileDialog.FileName));
        }
        private void WriteCmdsFromTextBoxButton_Click(object sender, RoutedEventArgs e)
        {
            // AutoRun
            AutoRunCmds(writeCmdsTextBox.Text.Split(new char[] { '\r', '\n' }));
        }

        private void SaveCmdsToFileButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            // Save File Dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save Commands File...",
                Filter = "Text File|*.txt",
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            if (saveFileDialog.ShowDialog() == false)
                return;
            FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create);

            fs.Write(System.Text.Encoding.Default.GetBytes(writeCmdsTextBox.Text), 0, writeCmdsTextBox.Text.Length);
            fs.Flush();
            fs.Close();
        }

        private void SerialPortStringSendFunc(string str)
        {
            serialPortComm.Write(str);
            SerialPortLoggerTextBox_Update(true, str);
        }

        private void SerialPortLoggerTextBox_Update(bool isTx, string str)
        {
            SerialPortLoggerTextBox_Update(isTx, str, serialPortCommInfoTextBox);
        }
        
        private void SerialPortLoggerTextBox_Update(bool isTx, string str, TextBox textBox)
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
                //textBox.Text += str_buf;

                log1.LogText += str_buf;
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

        private void AutoRunCmds(string[] txtArray)
        {
            DebugTabAutoRunCmds(txtArray, "./");
        }

        private void DebugTabAutoRunCmds(string[] txtArray, string taskFileDir)
        {
            AutoRunCmds(txtArray, taskFileDir);
        }

        private void AutoRunCmds(string[] txtArray, string taskFileDir)
        {
            try
            {
                txtArray = CmdFileDeleteComments(txtArray);

                // AutoRunCommands
                int sleepTime = Convert.ToInt32(cmdIntervalTextBox.Text);
                for (int i = 0; i < txtArray.Length; i++)
                {
                    AutoRunPraseCmd(txtArray[i], taskFileDir, sleepTime);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        string[] CmdFileDeleteComments(string[] txtArray)
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
            return txtArray;
        }

        void AutoRunPraseCmd(string command, string taskFileDir, int intervalTime)
        {
            if (command.Contains("<delay>") && command.Contains("</delay>"))
            {
                // GUI Delay
                intervalTime = Convert.ToInt32(MidStrEx(command, "<delay>", "</delay>"));
                SerialPortLoggerTextBox_Update(true, "Delay " + intervalTime + " ms...");
            }
            else if (command.Contains("<msgBox>") && command.Contains("</msgBox>"))
            {
                // MessageBox
                string msgBoxStr = MidStrEx(command, "<msgBox>", "</msgBox>");
                MessageBox.Show(msgBoxStr);
            }
            else if (command.Contains("<log>") && command.Contains("</log>"))
            {
                // Log Print
                string logStr = MidStrEx(command, "<log>", "</log>");
                SerialPortLoggerTextBox_Update(true, logStr);
            }
            else if (command.Contains("<waitCmd/>"))
            {
                // Wait For [COMMAND]: xxx
                isWaitingSignal = true;
                SerialPortLoggerTextBox_Update(true, "Waiting For Signal");
                while (isWaitingSignal)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
                // Receive Signal
                string str = ReceivedSignalStr.Replace(" ", "").Split(new char[] { ':' })[1];
                SerialPortLoggerTextBox_Update(true, "Get Signal:" + str);
            }
            else if (command.Contains("TASK1.PACK"))
            {
                string[] param = command.Split(new char[] { ';' });
                int AdcDataSize = Convert.ToInt32(param[1]);
                int BytesPerCode = Convert.ToInt32(bytesPerCodeTextBox.Text);
                int timeout = Convert.ToInt32(debugTabRxTimeOutTextBox.Text);
                // TASK1.PACK;256;
                if (param.Length > 3)
                {
                    // TASK1.PACK;256;4;1000;
                    BytesPerCode = Convert.ToInt32(param[2]);
                    timeout = Convert.ToInt32(param[3]);
                }
                Task1PACKReceive(AdcDataSize, BytesPerCode, timeout);
            }
            else if (command.Contains("<storeDic>") && command.Contains("</storeDic>"))
            {
                string dicKeyStr = MidStrEx(command, "<storeDic>", "</storeDic>");
                int size = adcDataStorage.WriteTmpAdcSamplesToDataStorage(dicKeyStr);
                SerialPortLoggerTextBox_Update(true, "Stored data to dictionary:<Key.Name=" + dicKeyStr + ", Value.Size=" + size + ">");
            }
            else if (command.Contains("<storeFile>") && command.Contains("</storeFile>"))
            {
                string fileNameStr = MidStrEx(command, "<storeFile>", "</storeFile>");
                // WriteFile
                string fullPathStr = Path.GetFullPath(taskFileDir + @"\" + fileNameStr);
                if (!Directory.Exists(Path.GetDirectoryName(fullPathStr)))
                {
                    var di = Directory.CreateDirectory(Path.GetDirectoryName(fullPathStr));
                    SerialPortLoggerTextBox_Update(true, "Create Directory:" + Path.GetDirectoryName(fullPathStr));
                }
                adcDataStorage.StoretmpAdcSamplesToFile(fullPathStr, DataStroageExtension.Csv);
                SerialPortLoggerTextBox_Update(true, "Stored data to path:" + fullPathStr);
            }
            else if(command.Contains("<setAnalysisParams>") && command.Contains("</setAnalysisParams>"))
            {
                adcCurrentSampleSettingInfo.Clear();
                string dataParams = MidStrEx(command, "<setAnalysisParams>", "</setAnalysisParams>");
                string[] dataParamsArray = dataParams.Split(new char[] { ';' });
                // <setAnalysisParams>a;1;b;2;c;3;d;4;</setAnalysisParams>
                string log = "Set Analysis Params: {";
                for (int i = 0; i < dataParamsArray.Length - 1; i += 2)
                {
                    adcCurrentSampleSettingInfo.Add(dataParamsArray[i], dataParamsArray[i + 1]);
                    log += " [" + dataParamsArray[i] + "," + dataParamsArray[i + 1] + "]";
                }
                log += " }";
                SerialPortLoggerTextBox_Update(true, log);
                adcCurrentSampleSettingInfoStr = dataParams;
            }
            else
            {
                // Tramsit Command
                SerialPortStringSendFunc(command);
            }
            if (intervalTime > 0)
            {
                // Stop Program and Keep UI alive.
                Thread t = new Thread(o => Thread.Sleep(intervalTime));
                t.Start(this);
                while (t.IsAlive)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
            }
        }

        private void SaveLogsToFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Save File Dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save Logs File...",
                Filter = "Text File|*.txt",
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            if (saveFileDialog.ShowDialog() == false)
                return;
            FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create);

            fs.Write(System.Text.Encoding.Default.GetBytes(log1.LogText), 0, log1.LogText.Length);
            fs.Flush();
            fs.Close();
        }

        private void ClearLogsButton_Click(object sender, RoutedEventArgs e)
        {
            log1.LogText = "";
        }
    }
}