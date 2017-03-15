using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//include new headers
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace TVM
{
    //http://www.cnblogs.com/binfire/archive/2011/10/08/2201973.html
    public class CommHopper
    {
        public delegate void EventHandle(byte[] readBuffer);
        public event EventHandle DataReceived;

        public SerialPort serialPort;
        Thread threadReading;
        volatile bool _keepReading;

        byte[] strAck= { 0x05, 0x10, 0x03, 0x10, 0x01, 0x29 };//debug

        public CommHopper()  //构造函数
        {
            serialPort = new SerialPort();
            threadReading = null;
            _keepReading = false;
            DataReceived += new CommHopper.EventHandle(commDataReceived);
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
                threadReading = new Thread(new ThreadStart(ReadPort));
                threadReading.Start();
                Console.WriteLine("start reading serial Hopper port");
                //MessageBox.Show("START READING");
            }
        }

        private void StopReading()
        {
            if (_keepReading)
            {
                _keepReading = false;
                threadReading.Join(); //thread执行完后再执行主线程
                threadReading = null;
            }
        }

        private void ReadPort()
        {
            while (_keepReading)
            {
                Thread.Sleep(100);  //延时，否则数据不完整
                if (serialPort.IsOpen)
                {
                    int count = serialPort.BytesToRead;
                    if (count > 0)
                    {
                        //MessageBox.Show("num:  " + count.ToString());
                        byte[] readBuffer = new byte[count];
                        try
                        {
                            //Application.DoEvents(); //winform 使用
                            serialPort.Read(readBuffer, 0, count);
                            if (DataReceived != null)
                            DataReceived(readBuffer);
                            //Thread.Sleep(100);
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
            //MessageBox.Show("ready");
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
            serialPort.Close(); //点击close以后，没有办法再打开
            //Thread.Sleep(1000);
        }

        /// <summary> 发送命令
        /// 发送命令
        /// </summary>
        /// <param name="send"></param>
        /// <param name="offSet"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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

        //串口数据获取事件
        private void commDataReceived(byte[] readBuffer)
        {
            //MessageBox.Show(readBuffer.Length.ToString());
            if (readBuffer.Length>0)
            {
                //string receive = ByteToStr(readBuffer);
                //MessageBox.Show(Encoding.UTF8.GetString(readBuffer));
                string receivedData = byteToHexStr(readBuffer);
                Console.Write("收到来自投币器的应答字节: "+ receivedData +"。应答内容为：" );
                //Console.WriteLine(byteToHexStr(strCoin));
                //strCoin为字符串常量
                if (string.Equals(byteToHexStr(strAck), receivedData))
                {
                    //退币器答复指令已收到
                    //_coin++;//收入一枚硬币
                    //Console.WriteLine("收入一枚硬币: " + _coin.ToString()); 
                }
                else 
                {
                    //此处需要建立日志文件，以便日后查看错误
                    Console.WriteLine("日志记录： "+ System.DateTime.Now.ToString() + "-----something wrong:-----" + receivedData);
                }
                //Console.WriteLine("yahoo " + StringToHexString(receive,Encoding.UTF8));// ByteToStr(readBuffer));
                # region debug
                //string str = "06";
                //if (string.Equals(receive.Trim(), str, StringComparison.CurrentCultureIgnoreCase))
                //{
                //    try
                //    {
                //        //if (is_read_card)
                //        //{
                //        //    byte[] send = new byte[1];
                //        //    send[0] = 0x05;
                //        //    comm.WritePort(send, 0, send.Length);
                //        //    Thread.Sleep(500);
                //        //    comm.DataReceived -= new Comm.EventHandle(comm_DataReceived);
                //        //    InitReadComm();
                //        //}
                //        //if (sendCardToOut)
                //        //{
                //        //    byte[] send = new byte[1];
                //        //    send[0] = 0x05;
                //        //    comm.WritePort(send, 0, send.Length);


                //        //    readComm.DataReceived -= new Comm.EventHandle(readComm_DataReceived);
                //        //    readComm.Close();

                //        //    log.Info("发卡完成！");
                //        //    lblMsg.Text = "发卡成功！";
                //        //    lblSendCardMsg.Text = "发卡完成，请收好卡！";
                //        //    timer1.Tick -= new EventHandler(timer1_Tick);
                //        //    PlaySound();
                //        //    this.btnOK.Enabled = true;


                //        //}
                //    }
                //    catch (Exception ex)
                //    {
                //        //log.Info(ex.ToString());
                //    }
                //}
                # endregion
            }
        }

        //数组转换字符串
        public static String ByteToStr(Byte[] bt)
        {
            //return encoding.GetString(bt);
            string str;
            return str = System.Text.Encoding.UTF8.GetString(bt);
        }
        /// <summary>字节数组转16进制字符串
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name=”bytes”></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
       {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                     returnStr += bytes[i].ToString("X2");
                 }
             }
            return returnStr;
         }

    }
}
