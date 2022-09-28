using Microsoft.Win32;
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

        const string CMD_OPEN_STR = "OPEN";
        const string CMD_RESET_STR = "RESET";
        const string CMD_DATW_STR = "DATW";
        const string CMD_DATR_STR = "DATR";
        const string CMD_REGW_STR = "REGW";
        const string CMD_REGR_STR = "REGR";
        const string CMD_REGM_STR = "REGM";
        const string CMD_REGQ_STR = "REGQ";
        const string CMD_TASK1RUN_STR = "TASK1.RUN";
        const string CMD_TASK1REALTIME_STR = "TASK1.REALTIME";
        const string CMD_TASK1PACK_STR = "TASK1.PACK";
        const string CMD_VCPPERFTEST_STR = "VCP.PERFTEST";

        SerialPortProtocol serialPort = new SerialPortProtocol();
        //Logger log1 = new Logger();
        LoggerControl loggerControl;

        private void SerialPortConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                string serialPortName = serialPortComboBox1.SelectedItem.ToString().Split('|').First().Trim();
                int serialPortBaudRate = 9600;
                int serialPortRxBufSize = Convert.ToInt32(debugTabRxBufSizeTextBox.Text);
                int serialPortRxTimeout = Convert.ToInt32(debugTabRxTimeOutTextBox.Text);
                int serialPortTxBufSize = 2048;
                int serialPortTxTimeout = SerialPort.InfiniteTimeout;

                serialPort = new SerialPortProtocol(serialPortName, serialPortBaudRate);
                serialPort.SetBuffer(serialPortRxBufSize, serialPortRxTimeout, serialPortTxBufSize, serialPortTxTimeout);
                serialPort.Open();
                debugTabSerialPortStatusLabel.Content = serialPort.IsOpen ? "Connected" : "Disconnected";

                serialPort.DataReceivedEvent(MyPort_DataReceived);
            }
            else
                MessageBox.Show("Serial port has been opened!");
        }

        private void SerialPortDisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                debugTabSerialPortStatusLabel.Content = serialPort.IsOpen ? "Connected" : "Disconnected";
            }
            else
                MessageBox.Show("Serial port has been closed!");
        }

        private void MyPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort.Closing) return;
            serialPort.Listening = true;

            // Refuse DataReceived Process
            if (!isDataReceivedRefused)
            {
                int _bytesToRead = serialPort.BytesToRead;
                if (_bytesToRead > 0)
                {
                    byte[] recvData = new byte[_bytesToRead];

                    serialPort.Read(recvData, _bytesToRead);

                    recvDataStr = System.Text.Encoding.ASCII.GetString(recvData);

                    if (isWaitingSignal)
                    {
                        if (recvDataStr.IndexOf("[COMMAND]") >= 0)
                        {
                            ReceivedSignalStr = recvDataStr;
                            isWaitingSignal = false;
                        }
                    }

                    loggerControl.UpdateLoggerTextBox(false, recvDataStr);
                }
            }
            serialPort.Listening = false;
        }

        private void CmdOPENButton_Click(object sender, RoutedEventArgs e)
        {
            string command = serialPort.SerialPortCommandParamsWrite(CMD_OPEN_STR);
            loggerControl.UpdateLoggerTextBox(true, command);
        }

        private void CmdRESETButton_Click(object sender, RoutedEventArgs e)
        {
            string command = serialPort.SerialPortCommandParamsWrite(CMD_RESET_STR);
            loggerControl.UpdateLoggerTextBox(true, command);
        }
        private void CmdDATWButton_Click(object sender, RoutedEventArgs e)
        {
            string command = serialPort.SerialPortCommandParamsWrite(CMD_DATW_STR, cmdDATWTextBox1.Text);
            loggerControl.UpdateLoggerTextBox(true, command);
        }

        private void CmdDATRButton_Click(object sender, RoutedEventArgs e)
        {
            string command = serialPort.SerialPortCommandParamsWrite(CMD_DATR_STR);
            loggerControl.UpdateLoggerTextBox(true, command);
        }

        private void CmdREGWButton_Click(object sender, RoutedEventArgs e)
        {
            string command = serialPort.SerialPortCommandParamsWrite(CMD_REGW_STR, cmdREGWTextBox1.Text, cmdREGWTextBox2.Text);
            loggerControl.UpdateLoggerTextBox(true, command);
        }

        private void CmdREGRButton_Click(object sender, RoutedEventArgs e)
        {
            string command = serialPort.SerialPortCommandParamsWrite(CMD_REGR_STR, cmdREGRTextBox1.Text);
            loggerControl.UpdateLoggerTextBox(true, command);
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
            string command = serialPort.SerialPortCommandParamsWrite(CMD_REGM_STR, cmdREGMTextBox1.Text, regBitsLSB.ToString(), regBitsLen.ToString(), cmdREGMTextBox3.Text);
            loggerControl.UpdateLoggerTextBox(true, command);
        }
        private void CmdREGQButton_Click(object sender, RoutedEventArgs e)
        {
            string[] regBitsModifyPosStr = cmdREGQTextBox2.Text.Split(new char[] { ':' });
            int[] regBitsModifyPos = new int[] {
                Convert.ToInt32(regBitsModifyPosStr[0] ),
                Convert.ToInt32(regBitsModifyPosStr[1])};
            int regBitsMSB = Math.Max(regBitsModifyPos[0], regBitsModifyPos[1]);
            int regBitsLSB = Math.Min(regBitsModifyPos[0], regBitsModifyPos[1]);
            int regBitsLen = regBitsMSB - regBitsLSB + 1;
            // Modify REG21[10:0]=0x180
            //  REGM;21;0;11;
            string command = serialPort.SerialPortCommandParamsWrite(CMD_REGQ_STR, cmdREGMTextBox1.Text, regBitsLSB.ToString(), regBitsLen.ToString());
            loggerControl.UpdateLoggerTextBox(true, command);
        }

        private void CmdTASK1RUNButton_Click(object sender, RoutedEventArgs e)
        {
            string command = serialPort.SerialPortCommandParamsWrite(CMD_TASK1RUN_STR, cmdTASK1RUNTextBox1.Text);
            loggerControl.UpdateLoggerTextBox(true, command);
        }

        private void CmdTASK1RealTimeButton_Click(object sender, RoutedEventArgs e)
        {
            bytesPerCode = Convert.ToInt32(bytesPerCodeTextBox.Text);
            int AdcDataSize = Convert.ToInt32(cmdTASK1RUNTextBox1.Text);
            int BytesPerCode = Convert.ToInt32(bytesPerCodeTextBox.Text);
            int timeout = Convert.ToInt32(debugTabRxTimeOutTextBox.Text);

            // TODO : Create TRANSFER FUNCTION
            int recvDataPackageSize = AdcDataSize * BytesPerCode;
            isDataReceivedRefused = true;
            string command = serialPort.SerialPortCommandParamsWrite(CMD_TASK1REALTIME_STR, AdcDataSize.ToString());
            loggerControl.UpdateLoggerTextBox(true, command);

            string str = "";
            byte[] recvDataPackage = new byte[recvDataPackageSize];
            // TODO REALTIME
            int data_len = serialPort.ReadContinuously(recvDataPackage, recvDataPackageSize, timeout);

            //ADC_Data_Stroage_Raw = new List<byte>(recvDataPackage);
            adcDataStorage.WriteTmpAdcSamples(adcCurrentSampleSettingInfoStr, recvDataPackage.ToList(), bytesPerCode);
            str += ("Read " + data_len + "/" + recvDataPackageSize + " Bytes in Packet.\n");
            str += ToHexStrFromByte(recvDataPackage);
            if (data_len != recvDataPackageSize)
                str += "\nWarning:Packets hasn't been received successfully!";
            loggerControl.UpdateLoggerTextBox(true, str);

            isDataReceivedRefused = false;

            if (serialPort.BytesToRead > 0)
                MyPort_DataReceived(null, null);
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
            string command = serialPort.SerialPortCommandParamsWrite(CMD_TASK1PACK_STR, adcDataSize.ToString());
            loggerControl.UpdateLoggerTextBox(true, command);

            string str = "";
            byte[] recvDataPackage = new byte[recvDataPackageSize];

            int data_len = serialPort.Read(recvDataPackage, recvDataPackageSize, timeout);

            //ADC_Data_Stroage_Raw = new List<byte>(recvDataPackage);
            adcDataStorage.WriteTmpAdcSamples(adcCurrentSampleSettingInfoStr, recvDataPackage.ToList(), bytesPerCode);
            str += ("Read " + data_len + "/" + recvDataPackageSize + " Bytes in Packet.\n");
            str += ToHexStrFromByte(recvDataPackage);
            if (data_len != recvDataPackageSize)
                str += "\nWarning:Packets hasn't been received successfully!";
            loggerControl.UpdateLoggerTextBox(true, str);

            isDataReceivedRefused = false;

            if (serialPort.BytesToRead > 0)
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

        //private void CmdVCPTESTButton_Click(object sender, RoutedEventArgs e)
        //{
        //    isDataReceivedRefused = true;
        //    int packetsCount = Convert.ToInt32(cmdVCPTESTPacketsTextBox.Text);
        //    string command = serialPort.SerialPortCommandParamsWrite(CMD_VCPPERFTEST_STR, packetsCount.ToString());
        //    loggerControl.UpdateLoggerTextBox(true, command);

        //    long ticksBegin = DateTime.Now.Ticks;
        //    Thread.Sleep(1000);
        //    long ticksEnd = DateTime.Now.Ticks;
        //    loggerControl.UpdateLoggerTextBox(true, "~" + (ticksEnd - ticksBegin) + " Ticks / Second.");

        //    byte[] recvBytes = new byte[1048576];
        //    int recvByteLength = 0;

        //    while (serialPort.BytesToRead == 0)
        //        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

        //    recvByteLength = serialPort.Read(recvBytes, serialPort.BytesToRead);
        //    ticksBegin = DateTime.Now.Ticks;
        //    loggerControl.UpdateLoggerTextBox(true, "Receive Packets Len = " + recvByteLength);

        //    for (int i = 0; i < packetsCount; i++)
        //    {
        //        while (serialPort.BytesToRead == 0)
        //            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

        //        recvByteLength = serialPort.Read(recvBytes, serialPort.BytesToRead);
        //        ticksEnd = DateTime.Now.Ticks;
        //        long elapsedTick = (ticksEnd - ticksBegin);
        //        double elapsedTime = (double)elapsedTick / 10000.0; // ms
        //        loggerControl.UpdateLoggerTextBox(true, recvByteLength + " Bytes,\t" + elapsedTick + " Ticks,\t" + elapsedTime + " ms,\t" + ((double)recvByteLength / elapsedTime).ToString("G4") + " KB/s.");
        //        ticksBegin = ticksEnd;
        //    }
        //    isDataReceivedRefused = false;
        //    if (serialPort.BytesToRead > 0)
        //        MyPort_DataReceived(null, null);
        //    loggerControl.UpdateLoggerTextBox(true, "VCP.TEST DONE.");
        //}

        private void AutoRunCmds(string[] txtArray)
        {
            DebugTabAutoRunCmds(txtArray, "./");
        }

        private void DebugTabAutoRunCmds(string[] txtArray, string taskFileDir)
        {
            AutoRunCmds(txtArray, taskFileDir);
        }

        int programCursor = 0;
        bool isResume = false;

        private void writeCmdsTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            programCursor = 0;
        }

        private void AutoRunCmds(string[] txtArray, string taskFileDir)
        {
            try
            {
                isResume = isAutoRunResumeFromStopCheckBox.IsChecked ?? false;

                txtArray = CmdFileDeleteComments(txtArray);

                // AutoRunCommands
                int sleepTime = Convert.ToInt32(cmdIntervalTextBox.Text);
                for (int i = isResume ? programCursor : 0; i < txtArray.Length; i++)
                {
                    int ret = AutoRunPraseCmd(txtArray[i], taskFileDir, sleepTime);
                    if (ret == -1)
                    {
                        programCursor = i + 1;
                        break;
                    }
                    if (i == txtArray.Length - 1)
                    {
                        programCursor = 0;
                    }
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

        int AutoRunPraseCmd(string command, string taskFileDir, int intervalTime)
        {
            if (command.Contains("<delay>") && command.Contains("</delay>"))
            {
                // GUI Delay
                intervalTime = Convert.ToInt32(MidStrEx(command, "<delay>", "</delay>"));
                loggerControl.UpdateLoggerTextBox(true, "Delay " + intervalTime + " ms...");
            }
            else if (command.Contains("<stop/>"))
            {
                loggerControl.UpdateLoggerTextBox(true, "STOP");
                return -1;
            }
            else if (command.Contains("<deldata>") && command.Contains("</deldata>"))
            {
                adcDataStorage = new AdcDataStorage();
                DataTabUpdateListBoxButton_Click(null, null);
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
                loggerControl.UpdateLoggerTextBox(true, logStr);
            }
            else if (command.Contains("<waitCmd/>"))
            {
                // Wait For [COMMAND]: xxx
                isWaitingSignal = true;
                loggerControl.UpdateLoggerTextBox(true, "Waiting For Signal");
                while (isWaitingSignal)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }
                // Receive Signal
                string str = ReceivedSignalStr.Replace(" ", "").Split(new char[] { ':' })[1];
                loggerControl.UpdateLoggerTextBox(true, "Get Signal:" + str);
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
                loggerControl.UpdateLoggerTextBox(true, "Stored data to dictionary:<Key.Name=" + dicKeyStr + ", Value.Size=" + size + ">");
            }
            else if (command.Contains("<storeFile>") && command.Contains("</storeFile>"))
            {
                string fileNameStr = MidStrEx(command, "<storeFile>", "</storeFile>");

                // WriteFile
                string fullPathStr = Path.GetFullPath(taskFileDir + @"\" + fileNameStr.Replace('/', '-').Replace('\\', '-'));
                if (!Directory.Exists(Path.GetDirectoryName(fullPathStr)))
                {
                    var di = Directory.CreateDirectory(Path.GetDirectoryName(fullPathStr));
                    loggerControl.UpdateLoggerTextBox(true, "Create Directory:" + Path.GetDirectoryName(fullPathStr));
                }
                adcDataStorage.StoretmpAdcSamplesToFile(fullPathStr, DataStroageExtension.Csv);
                loggerControl.UpdateLoggerTextBox(true, "Stored data to path:" + fullPathStr);
            }
            else if (command.Contains("<setAnalysisParams>") && command.Contains("</setAnalysisParams>"))
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
                loggerControl.UpdateLoggerTextBox(true, log);
                adcCurrentSampleSettingInfoStr = dataParams;
            }
            else
            {
                // Tramsit Command
                string txCmd = serialPort.SerialPortCommandLineWrite(command);
                loggerControl.UpdateLoggerTextBox(true, txCmd);
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
            return 0;
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

            fs.Write(Encoding.Default.GetBytes(loggerControl.LogText), 0, loggerControl.LogText.Length);
            fs.Flush();
            fs.Close();
        }

        private void ClearLogsButton_Click(object sender, RoutedEventArgs e)
        {
            loggerControl.LogText = "";
            serialPortCommInfoTextBox.Text = "";
        }
    }
}
