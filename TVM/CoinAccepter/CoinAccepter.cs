using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TVM.CoinAccepter
{
    public class CoinAccepter
    {
        # region //初始化
        Comm CoinComm;
        byte[] strEnable  =    { 0x90, 0x05, 0x01, 0x03, 0x99 };
        byte[] strDisable =    { 0x90, 0x05, 0x02, 0x03, 0x9A };
        byte[] strCheck   =    { 0x90, 0x05, 0x11, 0x03, 0xA9 };
        byte[] strAck     =    { 0x90, 0x05, 0x50, 0x03, 0xE8 };
        byte[] strNak     =    { 0x90, 0x05, 0x4B, 0x03, 0xE3 };
        byte[] strCoin    =    { 0x90, 0x06, 0x12, 0x03, 0x03, 0xAE };
        byte[] strDown    =    { 0x90, 0x05, 0x14, 0x03, 0xAC };
        byte[] strIdling  =    { 0x90, 0x05, 0x11, 0x03, 0xA9 };
        # endregion

        public CoinAccepter() 
        {
            //初始化投币器串口，是否打开？
            InitializeComm();
        }

        /// <summary> 串口类初始化
        /// 初始化串口类
        /// </summary>
        /// <returns></returns>
        private bool InitializeComm()
        {
            CoinComm = new Comm();
            if (CoinComm.Open())
            {
                //禁用硬币器
                if (CommCmdSender("DISABLE"))
                {
                    MessageBox.Show("禁止命令发送成功");
                    return true;
                }
                else
                {
                    MessageBox.Show("禁止命令发送失败");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("端口打开失败");
                return false;
            }
        }

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
                    return CoinComm.WritePort(strEnable, 0, 5);                    
                case "DISABLE":
                     return CoinComm.WritePort(strDisable, 0, 5);
                case "CHECK":
                    return CoinComm.WritePort(strCheck, 0, 5);
                default:
                    return false;
            }
        }
        
        /// <summary>  
        /// 查询投币数量  开辟一个新的线程进行操作  using？？
        /// </summary>
        /// <param name="coinNumber">准备接收的硬币数量</param>
        /// <returns>0:投币完成 1:投币未完成 2:退币按钮被按下</returns>
        public int CheckIn(int coinNumber)
        {
            if (true)
            { return 0; }
            else if (true)
            { return 1; }
            else
            { return 2; }
        }
        
            
    }
}
