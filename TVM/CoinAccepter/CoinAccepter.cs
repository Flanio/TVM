﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TVM
{
    public class CoinAccepter
    {
        # region 属性
        CommAccepter CoinAccepterComm;
        byte[] strEnable  =    { 0x90, 0x05, 0x01, 0x03, 0x99 };
        byte[] strDisable =    { 0x90, 0x05, 0x02, 0x03, 0x9A };
        byte[] strCheck   =    { 0x90, 0x05, 0x11, 0x03, 0xA9 };
        byte[] strAck     =    { 0x90, 0x05, 0x50, 0x03, 0xE8 };
        byte[] strNak     =    { 0x90, 0x05, 0x4B, 0x03, 0xE3 };
        byte[] strCoin    =    { 0x90, 0x06, 0x12, 0x03, 0x03, 0xAE };
        byte[] strDown    =    { 0x90, 0x05, 0x14, 0x03, 0xAC };
        byte[] strIdling  =    { 0x90, 0x05, 0x11, 0x03, 0xA9 };

        private static int _currentCoinsNum = 0;

        public bool _KeepCheck = false;
        public bool _PrintTicket = false;
        public int _TicketNum;
        # endregion

        #region 构造函数
        public CoinAccepter(string COM)  //构造函数
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
            CoinAccepterComm = new CommAccepter();
            //波特率
            CoinAccepterComm.serialPort.PortName = COM;
            CoinAccepterComm.serialPort.BaudRate = 9600;
            //数据位
            CoinAccepterComm.serialPort.DataBits = 8;
            //两个停止位
            CoinAccepterComm.serialPort.StopBits = System.IO.Ports.StopBits.One;
            //无奇偶校验位
            CoinAccepterComm.serialPort.Parity = System.IO.Ports.Parity.None;
            CoinAccepterComm.serialPort.ReadTimeout = 100;
            CoinAccepterComm.serialPort.WriteTimeout = -1; 
            if (CoinAccepterComm.Open()) {
                //禁用硬币器
                if (CommCmdSender("DISABLE")) {
                    Console.WriteLine("初始化禁止命令发送成功");
                    //MessageBox.Show("初始化禁止命令发送成功");
                    return true;
                }
                else {
                    MessageBox.Show("初始化禁止命令发送失败");
                    return false;
                }
            }
            else {
                MessageBox.Show("端口打开失败");
                return false;
            }
        }
        #endregion

        /*----------------------------------------------------------------*/
        #region 命令发送
        /// <summary>  投币机状态查询、设置命令
        /// 发送串口数据
        /// </summary>
        /// <param name="Cmd">串口命令</param>
        /// Cmd = ENABLE/DISABLE/CHECK
        /// <returns>发送成功返回值</returns>
        private bool CommCmdSender(string Cmd)
        {
            //byte[] str = {0x01,0x02};
            switch (Cmd)
            {
                case "ENABLE":
                    return CoinAccepterComm.WritePort(strEnable, 0, 5);
                case "DISABLE":
                    return CoinAccepterComm.WritePort(strDisable, 0, 5);
                case "CHECK":
                    {
                        return CoinAccepterComm.WritePort(strCheck, 0, 5);
                    }
                default:
                    return false;
            }
        }
        public void setReading()  //设置接受币状态
        {
            CommCmdSender("ENABLE");
        }
        public void setReject()  //设置禁用投币状态
        {
            CommCmdSender("DISABLE");
        }
        #endregion
        /*----------------------------------------------------------------*/

        /*----------------------------------------------------------------*/
        #region  硬币数量相关方法
        /// <summary>等待接收硬币
        /// 等待接收硬币
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool CheckAllIn(int NumNeeded)
        {
            //CommCmdSender("ENABLE");
            _KeepCheck = true;
            //MessageBox.Show("keepcheck");
            if(_KeepCheck)
            {
                if (GetCurrentCoinsNum() == NumNeeded)
                {
                    //MessageBox.Show("im in!!");
                    CommCmdSender("DISABLE"); //禁用收币器
                    //ClearCurrentCoinsNum();//设置硬币数量
                    //_KeepCheck = false;
                    return true;
                }
            }
            return false;
        }

        /// <summary> 查询投币数量 
        /// 查询投币数量  开辟一个新的线程进行操作  using？？
        /// </summary>
        /// <param name="coinNumber">准备接收的硬币数量,从UI获取</param>
        /// <returns>0:投币完成 1:投币未完成 2:退币按钮被按下</returns>
        public void CheckIn(int coinNumber)
        {
                
            if (_currentCoinsNum == coinNumber)
                {
                    _currentCoinsNum = 0;
                }
                else {
                }
        }

        /// <summary>获取当前硬币数量
        /// 获取当前硬币数量
        /// </summary>
        /// <returns></returns>
        public int GetCurrentCoinsNum() 
        {
            return _currentCoinsNum = CoinAccepterComm._coin;
        }
        /// <summary>硬币数量清零
        /// 硬币数量清零
        /// </summary>
        public void ClearCurrentCoinsNum()
        {
            CoinAccepterComm._coin = 0;
            _currentCoinsNum = 0;
        }
        #endregion
        /*----------------------------------------------------------------*/
    }
}
