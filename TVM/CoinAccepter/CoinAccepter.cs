using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVM.CoinAccepter
{
    public class CoinAccepter
    {
        public CoinAccepter() 
        {
            InitializeComm();
        }

        private static bool InitializeComm()
        {
            //禁用硬币器
            //查询当前状态
            return true;
        }

        /// <summary>
        /// 发送串口数据
        /// </summary>
        /// <returns></returns>
        private bool CommDataSender() 
        {
            return true;
        }
        
        /// <summary>  
        /// 查询投币数量  开辟一个新的线程进行操作  using？？
        /// </summary>
        /// <param name="coinNumber">准备接收的硬币数量</param>
        /// <returns>0:投币完成 1:投币未完成 2:退币按钮被按下</returns>
        public int Checkin(int coinNumber)
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
