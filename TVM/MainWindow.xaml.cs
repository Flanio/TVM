using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Threading;

namespace TVM
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        # region 属性

        MessageBoxResult mbr;

        CoinAccepter coinAccepter;  //投币器类
        int CoinNum;

        //退币标志位
        bool COINHOPPER = false;

        //打印机属性
        string path = "/dev/usb/lp0";  
        string dt;  //datetime string  //打印机属性

        Thread threadCoinHopper = null;//退币器进程
        # endregion

        public MainWindow()
        {
            InitializeComponent();
            //初始化所有硬件
            //1.初始化打印机  通讯是否正常？ 是否有纸？
            //TRY CATCH
            // PRINTER_OK = true
            //2.初始化投币器   采用串口通讯   开启后，串口一直处于打开状态，通过控制投币器使能来进行控制
            try { coinAccepter = new CoinAccepter(); }
            catch { MessageBox.Show("投币器初始化失败"); }
            //TRY CATCH
            // COIN_RECEIVER_OK = true
            //3.初始化退币器
            //TRY CATCH
            // COIN_RETURNER_OK = true
            //判断是否初始化完成？
            // if ( PRINTER_OK AND COIN_RECEIVER_OK AND COIN_RETURNER_OK )-->系统提示 完成
            // else () 提示错误
             
        }

        # region  购票按钮触发
        /// <summary>
        /// 购票事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_360(object sender, RoutedEventArgs e)
        {
            ///显示购票界面
            GetTicketInfo("360");
            //PrintTicket("360自行车");
            ///选择票型
            ///
        }

        private void Button_FullSound(object sender, RoutedEventArgs e)
        {
            ///显示购票界面
            GetTicketInfo("VR");
            //PrintTicket("VR体验");
            ///选择票型
        }

        private void Button_VR(object sender, RoutedEventArgs e)
        {
            ///显示购票界面
            GetTicketInfo("FullSound");
            //PrintTicket("全息音效");
            ///选择票型
        }
        # endregion

        # region 票面信息显示
        /// <summary>
        /// 票面信息显示
        /// </summary>
        /// <param name="item"></param>
        private void GetTicketInfo(string item)
        {
            switch (item)
            { 
                case "360":
                    //1 提示信息
                    mbr = MessageBox.Show("注意，本项目具有一定危险性","WARNING",MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    //确认注意选项后，才进入下一步，否则返回主菜单。
                    if (mbr == MessageBoxResult.OK)  //mbr 是否需要清零？？
                    {
                        //2 获取数据库票务数据并显示
                        // return item_list <--GetTicketInfoFromDatabase(string time,string item)
                        // Display(item_list)
                        //3 等待用户选择  数量  获取硬币数量 int coinNum
                        // if button_OK  clicked 确认支付后，开始进入投币接收状态
                        AcceptCoin(CoinNum);  //4 等待硬币投入 开辟一个新线程
                    }
                    else
                    { 
                        //退出返回主窗口
                    }
                   
                    break;
                case "VR":
                    mbr = MessageBox.Show("注意，本项目具有一定危险性", "WARNING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (mbr == MessageBoxResult.OK)
                    {
                        PrintTicket("VR体验");
                        MessageBox.Show("VR is PRINTING");
                    }
                    break;
                case "FullSound":
                    mbr = MessageBox.Show("注意，本项目具有一定危险性", "WARNING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (mbr == MessageBoxResult.OK)
                    {
                        PrintTicket("全息音效");
                        MessageBox.Show("FullSound is PRINTING");
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>接收硬币线程
        /// 接收硬币函数
        /// </summary>
        /// <param name="CoinNum"></param>
        public void AcceptCoin(int CoinNum)
        {
            threadCoinHopper = new Thread(new ParameterizedThreadStart(WaitForCoins));
            threadCoinHopper.Start(CoinNum);
            //WaitForCoins();//debug
            //MessageBox.Show("收取硬币线程开启");
        }

        public void WaitForCoins(object CoinNum)
        {
            //4.1if 投币完成
            //      出票
            //  if 出票完成  返回主界面
            //4.2else  退钱按钮被按下
            //  退款
            //  4.2.1if 退款成功 返回主界面
            //  4.2.2else MessageBox 请联系工作人员
            while (!COINHOPPER) //退币选项未被按下
            {
                {
                    if (coinAccepter.WaitForCoins( int.Parse( CoinNum.ToString() ) ) )  //硬币数量
                    {
                        coinAccepter.ClearCurrentCoinsNum();
                        //PrintTicket("360自行车");
                        MessageBox.Show("360 is PRINTING");
                    }
                }
            }
            if(COINHOPPER == true)
            {                
                COINHOPPER = false;
                MessageBox.Show("进入退币模块");
            //获取当前已投入硬币数量 int num = CoinAccepter.getCurrentCoinsNums()
            //退币
            }
        }

        # endregion

        # region 数据库信息操作
        /// <summary>
        /// 数据库操作
        /// </summary>
        /// <param name="time"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private List<string> Get_ticket_info_from_Database(string time,string item) //自定义一个List类型 包括 票的时间和数量
        {
            //根据当前时间和项目名称获取数据
            List<string> list = new List<string>();
            list.Add("a");
            list.Add("b");
            return list;
        }

        private int Change_ticket_info_from_Database(string item, string article, bool sellout, int num)
        { 
            //根据项目名称，出票数量修改数据库票数信息
            bool SOLDOUT = sellout;
            if (SOLDOUT== true)
            {

            }
            return num;//返回剩余票数
        }
        # endregion

        private void Button_CoinHopper(object sender, RoutedEventArgs e) //我要退币
        {
            //退币
            COINHOPPER = true;//退币状态置位1
            //调用退币模块
            CoinNum = coinAccepter.GetCurrentCoinsNum(); //获取当前收币模块中的硬币数量
            MessageBox.Show(CoinNum.ToString());
            //CoinHopper(int CoinNum); //退币 数量为CoinNum

        }

        private void Button_start_comm(object sender, RoutedEventArgs e)
        {
            coinAccepter.setReading();//test      
        }

        # region 门票打印模块

        # region DLL import
        [DllImport("MsprintsdkRM.dll")]
        public static extern int SetDevname(int iDevtype, string cDevname, int iBaudrate);
        [DllImport("MsprintsdkRM.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetPrintport(string strPort, int iBaudrate);
        [DllImport("MsprintsdkRM.dll")]
        public static extern int SetInit();
        [DllImport("MsprintsdkRM.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PrintFeedline(int iLine);
        [DllImport("MsprintsdkRM.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PrintString(string strData, int iImme);
        [DllImport("MsprintsdkRM.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetClean();
        [DllImport("MsprintsdkRM.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PrintCutpaper(int iMode);
        [DllImport("MsprintsdkRM.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PrintDiskbmpfile(string strPath);
        # endregion
        /// <summary>  门票打印
        /// 门票打印
        /// </summary>
        /// <param name="ticketName">门票项目名称</param>
        private void PrintTicket(string ticketName)
        {
            (SetPrintport("USB001", 38400)).ToString();
            SetInit().ToString();
            //SetClean();
            PrintFeedline(1);
            //SetInit().ToString();
            PrintDiskbmpfile("logo2.bmp");
            PrintString("----------------------", 0);
            dt = System.DateTime.Now.ToLongDateString().ToString();
            PrintString(ticketName + "\n票价 5元\n" + "有效期 " + dt + "\npowered by SXKJG\n限制定场次有效", 0);
            //SetInit().ToString();
            PrintString("----------------------", 0);
            PrintFeedline(10);
            PrintCutpaper(1);
        }
        # endregion
    }
}
