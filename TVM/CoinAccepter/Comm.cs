using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//include new headers
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace TVM.CoinAccepter
{
    //http://www.cnblogs.com/binfire/archive/2011/10/08/2201973.html
    public class Comm
    {
        public delegate void EventHandle(byte[] readBuffer);
        public event EventHandle DataReceived;

        public SerialPort serialPort;
        Thread thread;
        volatile bool _keepReading;

        public Comm()
        {
            serialPort = new SerialPort();
            thread = null;
            _keepReading = false;
        }

        public bool IsOpen
        {
            get
            {
                return serialPort.IsOpen;
            }
        }

        private void StartReading()
        {
            if (!_keepReading)
            {
                _keepReading = true;
                thread = new Thread(new ThreadStart(ReadPort));
                thread.Start();
            }
        }

        private void StopReading()
        {
            if (_keepReading)
            {
                _keepReading = false;
                thread.Join(); //
                thread = null;
            }
        }

        private void ReadPort()
        {
            while (_keepReading)
            {
                if (serialPort.IsOpen)
                {
                    int count = serialPort.BytesToRead;
                    if (count > 0)
                    {
                        byte[] readBuffer = new byte[count];
                        try
                        {
                            //Application.DoEvents();
                            serialPort.Read(readBuffer, 0, count);
                            if (DataReceived != null)
                                DataReceived(readBuffer);
                            Thread.Sleep(100);
                        }
                        catch (TimeoutException)
                        {
                        }
                    }
                }
            }
        }

        public bool Open()
        {
            Close();
            serialPort.Open();
            if (serialPort.IsOpen)
            {
                StartReading();
                return true;
            }
            else
            {
                MessageBox.Show("串口打开失败！");
                return false;
            }
        }

        public void Close()
        {
            StopReading();
            serialPort.Close();
        }

        public bool WritePort(byte[] send, int offSet, int count)
        {
            if (IsOpen)
            {
                try
                {
                    serialPort.Write(send, offSet, count);
                    return true;
                }
                catch
                {
                    MessageBox.Show("串口数据发送失败");
                    return false;
                }
            }
            else
                return false;
        }
    }
}
