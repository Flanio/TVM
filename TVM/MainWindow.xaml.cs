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

using System.ComponentModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace TVM
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        # region 属性

        bool mbr;

        CoinAccepter coinAccepter;  //投币器类
        int CoinNum = 5;
        int TicketNum = 0;

        //退币标志位
        bool COINHOPPER = false;

        //打印机属性
        string path = "/dev/usb/lp0";
        string dt;  //datetime string  //打印机属性

        Thread threadCoinAccepter = null;//投币器进程
        Thread threadCoinHopper = null;//退币器进程

        /// <summary>
        /// Current status text to display
        /// 当前状态显示字符串
        /// </summary>
        private string statusText = "1";

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
                    Launch360();
                    break;
                case "VR":
                    LaunchVR();
                    break;
                case "FullSound":
                    IMAGE_360.Visibility = Visibility.Visible;
                    IMAGE_VR.Visibility = Visibility.Hidden;
                    MBWarning();
                    if (mbr)
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
            if (threadCoinAccepter == null)
            {
                threadCoinAccepter = new Thread(new ParameterizedThreadStart(WaitForCoins));
                threadCoinAccepter.Start(CoinNum);
            }
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
            if (coinAccepter.GetCurrentCoinsNum() != 0)
            { MessageBox.Show("error!" + coinAccepter.GetCurrentCoinsNum().ToString() + "当前硬币数量不为0"); }
            while (!COINHOPPER) //退币选项未被按下
            {
                INSTANTNUMBER.Dispatcher.Invoke(new Action(() => INSTANTNUMBER.Text = coinAccepter.GetCurrentCoinsNum().ToString())); 
                //INSTANTNUMBER.Text = "5";//coinAccepter.GetCurrentCoinsNum().ToString();
                if (coinAccepter.CheckAllIn(int.Parse(CoinNum.ToString())))  //硬币数量达到要求数量 （5个）
                {
                    //coinAccepter.ClearCurrentCoinsNum();
                    //PrintTicket("360自行车");
                    MessageBox.Show("360 is PRINTING");
                    INSTANTNUMBER.Dispatcher.Invoke(new Action(() => INSTANTNUMBER.Text = "0"));
                    TICKETNUMBER.Dispatcher.Invoke(new Action(() => TICKETNUMBER.Text = "0"));
                    //coinAccepter.setReject();//禁用投币器
                    break;
                }
            }
            MessageBox.Show("threadabort");
            threadCoinAccepter.Abort();
        }

        # endregion

        # region 数据库信息操作
        /// <summary>
        /// 数据库操作
        /// </summary>
        /// <param name="time"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private List<string> Get_ticket_info_from_Database(string time, string item) //自定义一个List类型 包括 票的时间和数量
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
            if (SOLDOUT == true)
            {

            }
            return num;//返回剩余票数
        }
        # endregion

        private void Button_CoinHopper(object sender, RoutedEventArgs e) //我要退币
        {
            coinAccepter.setReject();//禁止收币功能
            //退币
            COINHOPPER = true;//退币状态置位1
            //调用退币模块
            CoinNum = coinAccepter.GetCurrentCoinsNum(); //获取当前收币模块中的硬币数量
            MessageBox.Show("退币数量为 " + CoinNum.ToString() + "\n点击确认后开始退币");
            COINHOPPER = false;
            //CoinHopper(int CoinNum); //退币 数量为CoinNum
        }

        private void Button_start_insert(object sender, RoutedEventArgs e)
        {
            AcceptCoin(CoinNum);  //4 等待硬币投入 开辟一个新线程
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

        #region 数值更新
        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Gets or sets the current status text to display
        /// 获取和设置当前状态并显示
        /// </summary>
        /// <summary>
        /// Gets or sets the current status text to display
        /// 获取和设置当前状态并显示
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                //如果传入的数值不等于原来的值，则进入下一步，否则不更新
                {
                    this.statusText = value;
                    //this.showText = "wowo";

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }


        private void plus(object sender, RoutedEventArgs e)
        {
            TicketNum++;
            CoinNum = TicketNum * 5;
            TICKETNUMBER.Text = TicketNum.ToString();
        }

        private void minus(object sender, RoutedEventArgs e)
        {
            if (TicketNum>0)
            {
                CoinNum = TicketNum * 5;
                TicketNum--;
            }
            TICKETNUMBER.Text = TicketNum.ToString();
        }
        #endregion

        /// <summary>
        /// 启动弹出窗口，所有控件在xaml文件中设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Launch360()
        {
            //1 提示信息
            MBWarning();
        }

        private void LaunchVR()
        {
            IMAGE_360.Visibility = Visibility.Hidden;
            IMAGE_VR.Visibility = Visibility.Visible;
            MBWarning();
            if (mbr)
            {
                this.ToggleFlyout(1);
                PrintTicket("VR体验");
                MessageBox.Show("VR is PRINTING");
            }
        }

        private void LaunchFullSound() { }

        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }

        private void ShowSettingsRight(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)this.Flyouts.Items[1];
            flyout.Position = Position.Right;
        }

        private void ShowSettingsLeft(object sender, RoutedEventArgs e)
        {
            var flyout = (Flyout)this.Flyouts.Items[1];
            flyout.Position = Position.Left;
        }

        private async void MBWarning()
        {
            // This demo runs on .Net 4.0, but we're using the Microsoft.Bcl.Async package so we have async/await support
            // The package is only used by the demo and not a dependency of the library!
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "YES",
                NegativeButtonText = "NOP",
                FirstAuxiliaryButtonText = "CANCEL",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            MessageDialogResult result = await this.ShowMessageAsync("警告", "注意，本项目具有一定危险性",
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

            if (result == MessageDialogResult.Affirmative)
                //确认注意选项后，才进入下一步，否则返回主菜单。
                    this.ToggleFlyout(0);
                    //2 获取数据库票务数据并显示
                    // return item_list <--GetTicketInfoFromDatabase(string time,string item)
                    // Display(item_list)
                    //3 等待用户选择  数量  获取硬币数量 int coinNum
                    // if button_OK  clicked 确认支付后，开始进入投币接收状态
            else
                {
                    //退出返回主窗口
                }
        }
    }
}

