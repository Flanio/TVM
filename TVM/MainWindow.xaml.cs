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

        CoinAccepter coinAccepter;  //投币器类
        int CoinNum = 5;
        int TicketNum = 1;
        int PreCoinNum = 0;

        //退币标志位
        bool COINHOPPER = false;

        //打印机属性
        //string path = "/dev/usb/lp0";
        string dt;  //datetime string  //打印机属性

        Thread threadCoinAccepter = null;//投币器进程
        Thread threadCoinHopper = null;//退币器进程

        /// <summary>
        /// Current status text to display
        /// 当前状态显示字符串
        /// </summary>
        private string statusText = "1";

        # endregion

        #region 构造函数
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
        #endregion

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
                    LaunchFullSound();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 启动弹出窗口，所有控件在xaml文件中设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Launch360()
        {
            //1 提示信息
            MBWarning("360");
        }
        private void LaunchVR()
        {
            IMAGE_360.Visibility = Visibility.Hidden;
            IMAGE_VR.Visibility = Visibility.Visible;
            MBWarning("VR");
        }
        private void LaunchFullSound() {
            IMAGE_VR.Visibility = Visibility.Hidden;
            IMAGE_360.Visibility = Visibility.Visible;
            MBWarning("FullSound");
        }
        private async void MBWarning(string item)  //提示窗口
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
            {
                //确认注意选项后，才进入下一步，否则返回主菜单。
                //2 获取数据库票务数据并显示
                // return item_list <--GetTicketInfoFromDatabase(string time,string item)
                // Display(item_list)
                //3 等待用户选择  数量  获取硬币数量 int coinNum
                // if button_OK  clicked 确认支付后，开始进入投币接收状态
                switch (item)
                {
                    case "360":
                        FlyoutTool.Header = "360自行车";
                        this.ToggleFlyout(0);
                        break;
                    case "VR":
                        FlyoutTool.Header = "VR";
                        this.ToggleFlyout(0);
                        break;
                    case "FullSound":
                        FlyoutTool.Header = "全息音效";
                        this.ToggleFlyout(0);
                        break;
                }
            }


            else
            {
                //退出返回主窗口
            }
        }
        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }

        private void Flyout_close(object sender, RoutedEventArgs e)
        {
            //退币
            //if(coinAccepter.GetCurrentCoinsNum != 0)
            //{ } 此处有bug 投币过程中不能退出
            //threadCoinAccepter.Abort();
            //Button_CoinHopper(this, System.Windows.RoutedEvent);
            TicketNum = 1;
            TICKETNUMBER.Text = "1";
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

        #endregion

        #region UI交互接口
        private void Button_CoinHopper(object sender, RoutedEventArgs e) //我要退币
        {
            ButtonInsert.Visibility = System.Windows.Visibility.Visible;
            Buttonhopper.Visibility = System.Windows.Visibility.Collapsed;
            coinAccepter.setReject();//禁止收币功能
            COINHOPPER = true;//退币状态置位1
            //投币线程强制终止
            threadCoinAccepter.Abort();
            //退币
            //调用退币模块
            CoinNum = coinAccepter.GetCurrentCoinsNum(); //获取当前收币模块中的硬币数量
            MessageBox.Show("退币数量为 " + CoinNum.ToString() + "\n点击确认后开始退币");
            COINHOPPER = false;

            TicketNum = 1;
            CoinNum = 5;
            //CoinHopper(int CoinNum); //退币 数量为CoinNum
        }

        private void Button_start_insert(object sender, RoutedEventArgs e)
        {
            Buttonhopper.Visibility = System.Windows.Visibility.Visible;
            ButtonInsert.Visibility = System.Windows.Visibility.Collapsed;
            AcceptCoin(CoinNum);  //4 等待硬币投入 开辟一个新线程
        }

        private void plus(object sender, RoutedEventArgs e)
        {
            TicketNum++;
            CoinNum = TicketNum * 5;
            TICKETNUMBER.Text = TicketNum.ToString();
        }

        private void minus(object sender, RoutedEventArgs e)
        {
            if (TicketNum > 1)
            {
                TicketNum--;
                CoinNum = TicketNum * 5;                
            }
            TICKETNUMBER.Text = TicketNum.ToString();
        }
        #endregion

        #region 硬币接收
        /// <summary>接收硬币线程
        /// 接收硬币函数
        /// </summary>
        /// <param name="CoinNum"></param>
        public void AcceptCoin(int CoinNum)
        {
            //if (threadCoinAccepter == null)
            {
                //MessageBox.Show("new coin thread");
                threadCoinAccepter = new Thread(new ParameterizedThreadStart(WaitForCoins));
                MessageBox.Show(CoinNum.ToString());
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
            coinAccepter.setReading();//test    
            if (coinAccepter.GetCurrentCoinsNum() != 0)
            { MessageBox.Show("error!" + coinAccepter.GetCurrentCoinsNum().ToString() + "当前硬币数量不为0"); }
            while (!COINHOPPER) //退币选项未被按下
            {
                //INSTANTNUMBER.Text = "5";//coinAccepter.GetCurrentCoinsNum().ToString();
                if (coinAccepter.CheckAllIn(int.Parse(CoinNum.ToString())))  //硬币数量达到要求数量 （5个）
                {
                    INSTANTNUMBER.Dispatcher.Invoke(new Action(() => INSTANTNUMBER.Text = coinAccepter.GetCurrentCoinsNum().ToString()));
                    //coinAccepter.ClearCurrentCoinsNum();
                    //PrintTicket("360自行车");
                    ButtonInsert.Dispatcher.Invoke(new Action(() => ButtonInsert.Visibility = System.Windows.Visibility.Visible));
                    ButtonInsert.Dispatcher.Invoke(new Action(() => Buttonhopper.Visibility = System.Windows.Visibility.Collapsed));

                    MessageBox.Show("360 is PRINTING");
                    //重置无效？？
                    TicketNum = 1;
                    CoinNum = 5 ;
                    MessageBox.Show("pre"+CoinNum.ToString()+ "  pre"+TicketNum.ToString());
                    INSTANTNUMBER.Dispatcher.Invoke(new Action(() => INSTANTNUMBER.Text = "0"));
                    TICKETNUMBER.Dispatcher.Invoke(new Action(() => TICKETNUMBER.Text = "1"));
                    //coinAccepter.setReject();//禁用投币器
                    break;
                }
                else
                {
                    if (PreCoinNum != coinAccepter.GetCurrentCoinsNum())
                    {
                        PreCoinNum = coinAccepter.GetCurrentCoinsNum();
                        INSTANTNUMBER.Dispatcher.Invoke(new Action(() => INSTANTNUMBER.Text = coinAccepter.GetCurrentCoinsNum().ToString()));
                    }
                }
            }
            //MessageBox.Show("threadabort");
            threadCoinAccepter.Abort();
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



        #endregion


    }
}

