﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ADC_CDC_CONTROLLER
{
    class SerialPortComm
    {
        private SerialPort serialPort;
        public bool IsOpen => serialPort.IsOpen;
        public int BytesToRead => serialPort.BytesToRead;

        public SerialPortComm()
        {
            serialPort = new SerialPort();
        }

        public SerialPortComm(string portName, int baudRate)
        {
            try
            {
                serialPort = new SerialPort(portName, baudRate);
                serialPort.NewLine = "\n";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void DataReceivedEvent(SerialDataReceivedEventHandler serialDataReceivedEventHandler)
        {
            serialPort.DataReceived += serialDataReceivedEventHandler;
        }

        public void SetBuffer(int rxBufSize, int rxTimeout, int txBufSize, int txTimeout)
        {
            serialPort.ReadBufferSize = rxBufSize;
            serialPort.ReadTimeout = rxTimeout;
            serialPort.WriteBufferSize = txBufSize;
            serialPort.WriteTimeout = txTimeout;
        }

        public void Open()
        {
            try
            {
                serialPort.Open();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Close()
        {
            try
            {
                serialPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // SOLUTION:https://stackoverflow.com/questions/16439897/serialport-readbyte-int32-int32-is-not-blocking-but-i-want-it-to-how-do
        // Stop Program and Keep UI alive.
        public int Read(byte[] responseBytes, int bytesExpected, int timeOut)
        {
            if (timeOut == 0)
            {
                if(serialPort.BytesToRead < bytesExpected)
                    bytesExpected = serialPort.BytesToRead;
                return Read(responseBytes, bytesExpected);
            }
            else
            {
                if (timeOut == SerialPort.InfiniteTimeout)
                    timeOut = Timeout.Infinite;

                int ActualBytesToRead = 0;
                Thread t = new Thread(o => Thread.Sleep(timeOut));
                t.Start(this);
                while (t.IsAlive)
                {
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

                    if (serialPort.BytesToRead >= bytesExpected)
                    {
                        ActualBytesToRead = bytesExpected;
                        t.Abort();
                    }
                    else
                    {
                        ActualBytesToRead = serialPort.BytesToRead;
                    }
                }

                return Read(responseBytes, ActualBytesToRead);
            }
        }

        public int Read(byte[] responseBytes, int bytesExpected)
        {
            int offset = 0, bytesRead;
            try
            {
                while (bytesExpected > 0 &&
                  (bytesRead = serialPort.Read(responseBytes, offset, bytesExpected)) > 0)
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

        public void Write(byte[] buffer, int bytesCount)
        {
            try
            {
                serialPort.Write(buffer, 0, bytesCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Write(string str)
        {
            try
            {
                serialPort.Write(str);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}