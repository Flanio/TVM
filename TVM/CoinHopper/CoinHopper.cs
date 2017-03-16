using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TVM
{
    public class CoinHopper
    {
        # region 属性
        CommHopper CoinHopperComm;
        byte[] strOrigin = { 0x05, 0x10, 0x03, 0x10, 0x01, 0x29 };

        # endregion

        #region 构造函数
        public CoinHopper(string COM)  //构造函数
        {
            //初始化投币器串口，是否打开？
            InitializeComm(COM);
        }
        /// <summary> 串口类初始化
        /// 初始化串口类
        /// </summary>
        /// <returns></returns>
        private bool InitializeComm(string COM)
        {
            CoinHopperComm = new CommHopper();
            //波特率
            CoinHopperComm.serialPort.PortName = COM;
            CoinHopperComm.serialPort.BaudRate = 9600;
            //数据位
            CoinHopperComm.serialPort.DataBits = 8;
            //两个停止位
            CoinHopperComm.serialPort.StopBits = System.IO.Ports.StopBits.One;
            //无奇偶校验位
            CoinHopperComm.serialPort.Parity = System.IO.Ports.Parity.Even;
            CoinHopperComm.serialPort.ReadTimeout = 100;
            CoinHopperComm.serialPort.WriteTimeout = -1; 
            if (CoinHopperComm.Open()) {
                //禁用硬币器
                if (CommCmdSender("RETURN",0)) {
                    Console.WriteLine("初始化退币命令发送成功");
                    //MessageBox.Show("初始化禁止命令发送成功");
                    return true;
                }
                else {
                    MessageBox.Show("初始化退币命令发送失败");
                    return false;
                }
            }
            else {
                MessageBox.Show("退币端口打开失败");
                return false;
            }
        }
        #endregion

        /*----------------------------------------------------------------*/
        #region 命令发送
        /// <summary>  退币器状态查询、设置命令
        /// 发送串口数据
        /// </summary>
        /// <param name="Cmd">串口命令</param>
        /// Cmd = RETURN/DISABLE/CHECK
        /// <returns>发送成功返回值</returns>
        private bool CommCmdSender(string Cmd,int CoinNum)
        {
            //byte[] str = {0x01,0x02};

            //strOrigin[4] = 
            //Console.WriteLine( (CoinNum.ToString("x2")));
            //Console.WriteLine( Encoding.ASCII.GetBytes(CoinNum.ToString("x2")).ToString());
            strOrigin[4] = Convert.ToByte(CoinNum.ToString());
            strOrigin[5] = CheckSum(strOrigin);//计算校验位
            switch (Cmd)
            {
                case "RETURN":
                    return CoinHopperComm.WritePort(strOrigin, 0, 6);//5改为6
                default:
                    return false;
            }
        }
        /// <summary>
        /// 退币方法
        /// </summary>
        /// <param name="CoinNum">退币数量</param>
        public void returnCoin(int CoinNum)  
        {
            CommCmdSender("RETURN",CoinNum);
        }

        #endregion

        private byte CheckSum( byte [] str)
        {
            byte checksum = 0;
            for (int i = 0; i < 5; i++)
            {
                checksum += strOrigin[i];
            }
                return checksum ;
        }
        /*----------------------------------------------------------------*/

        /*----------------------------------------------------------------*/

        /*----------------------------------------------------------------*/
    }
}
