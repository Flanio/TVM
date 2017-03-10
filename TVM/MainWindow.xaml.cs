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
        CoinHopper coinHopper;
        int CoinNum = 5;
        int TicketNum = 1;
        int PreCoinNum = 0;
        string TicketName = "";
        int Factor =5;

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

        MessageDialogResult result;

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
            try { coinAccepter = new CoinAccepter("COM7"); }  //初始化选择COM口
            catch { MessageBox.Show("投币器初始化失败"); Application.Current.Shutdown(); }
            try { coinHopper = new CoinHopper("COM5"); }
            catch { MessageBox.Show("退币器初始化失败"); Application.Current.Shutdown(); }
            //TRY CATCH
            // COIN_RECEIVER_OK = true
            //3.初始化退币器
            //TRY CATCH
            // COIN_RETURNER_OK = true
            //判断是否初始化完成？
            // if ( PRINTER_OK AND COIN_RECEIVER_OK AND COIN_RETURNER_OK )-->系统提示 完成
            // else () 提示错误

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBox.Show("");
            //e.Cancel = true;
            Environment.Exit(-1);//暴力退出
            //Application.Current.Shutdown();
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
            GetTicketInfo("FullSound");
            //PrintTicket("VR体验");
            ///选择票型
        }

        private void Button_VR(object sender, RoutedEventArgs e)
        {
            ///显示购票界面
            GetTicketInfo("VR");
            //PrintTicket("全息音效");
            ///选择票型
        }
        # endregion

        # region 票面信息显示 flyout 窗口
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
            TicketName = "360自行车";
            Factor = 5;
            IMAGE_FULLSOUND.Visibility = Visibility.Hidden;
            IMAGE_360.Visibility = Visibility.Visible;
            IMAGE_VR.Visibility = Visibility.Hidden;
            //1 提示信息
            MBWarning("360");
        }
        private void LaunchVR()
        {
            TicketName = "VR";
            Factor = 10;
            IMAGE_FULLSOUND.Visibility = Visibility.Hidden;
            IMAGE_360.Visibility = Visibility.Hidden;
            IMAGE_VR.Visibility = Visibility.Visible;
            MBWarning("VR");
        }
        private void LaunchFullSound() {
            TicketName = "全息音效";
            Factor = 5;
            IMAGE_360.Visibility = Visibility.Hidden;
            IMAGE_VR.Visibility = Visibility.Hidden;
            IMAGE_FULLSOUND.Visibility = Visibility.Visible;
            MBWarning("FullSound");
        }
        private async void MBWarning(string item)  //提示窗口
        {
            // This demo runs on .Net 4.0, but we're using the Microsoft.Bcl.Async package so we have async/await support
            // The package is only used by the demo and not a dependency of the library!
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "确定",
                NegativeButtonText = "取消",
                DialogMessageFontSize = 30.0,
                //FirstAuxiliaryButtonText = "CANCEL",
                ColorScheme = MetroDialogOptions.ColorScheme
            };


            switch(item)
            {
                case "360":
                    result = await this.ShowMessageAsync("提示", "注意，本项目具有一定危险性。心脏病、高血压患者紧致体验。体验券5元一张。只接受硬币。",
                    MessageDialogStyle.AffirmativeAndNegative, mySettings);
                    break;
                case "VR":
                    result = await this.ShowMessageAsync("提示", "注意，本项目具有一定危险性。心脏病、高血压患者紧致体验。体验券10元一张。只接受硬币。",
                    MessageDialogStyle.AffirmativeAndNegative, mySettings);
                    break;
                case "FullSound":
                    result = await this.ShowMessageAsync("提示", "注意，本项目具有一定危险性。心脏病、高血压患者紧致体验。体验券5元一张。只接受硬币。",
                    MessageDialogStyle.AffirmativeAndNegative, mySettings);
                    break;
                case "OTHER":
                    break;
                default:
                    break;
            }


            if (result == MessageDialogResult.Affirmative)
            {
                //确认注意选项后，才进入下一步，否则返回主菜单。
                //2 获取数据库票务数据并显示
                // return item_list <--GetTicketInfoFromDatabase(string time,string item)
                // Display(item_list)
                //3 等待用户选择  数量  获取硬币数量 int coinNum
                // if button_OK  clicked 确认支付后，开始进入投币接收状态
                ButtonInsert.Visibility = System.Windows.Visibility.Visible;
                ButtonPlus.Visibility = System.Windows.Visibility.Visible;
                ButtonMinus.Visibility = System.Windows.Visibility.Visible;
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
                IMAGE_FULLSOUND.Visibility = Visibility.Hidden;
                IMAGE_360.Visibility = Visibility.Hidden;
                IMAGE_VR.Visibility = Visibility.Hidden;
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
            if (coinAccepter.GetCurrentCoinsNum() != 0)
            {
                Hopper();
                //coinHopper.returnCoin(coinAccepter.GetCurrentCoinsNum());
                //CoinHopper();//Button_CoinHopper(this,System.Windows.)//调用退币模块
                //FlyoutTool.CloseCommand.Execute
            } //此处有bug 投币过程中不能退出
            //threadCoinAccepter.Abort();
            //else
            {
                TicketNum = 1;
                TICKETNUMBER.Text = "1";

                IMAGE_FULLSOUND.Visibility = Visibility.Hidden;
                IMAGE_360.Visibility = Visibility.Hidden;
                IMAGE_VR.Visibility = Visibility.Hidden;
            }
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
            Hopper();
        }

        private void Button_start_insert(object sender, RoutedEventArgs e)
        {
            //FlyoutTool.CloseButtonVisibility = System.Windows.Visibility.Hidden;
            FlyoutTool.IsPinned = true;
            Buttonhopper.Visibility = System.Windows.Visibility.Visible; //取消退币按钮，改为直接点击退出并退币。
            ButtonInsert.Visibility = System.Windows.Visibility.Collapsed;
            ButtonPlus.Visibility   = System.Windows.Visibility.Collapsed;
            ButtonMinus.Visibility  = System.Windows.Visibility.Collapsed;
            TicketNum = int.Parse( TICKETNUMBER.Text );
            CoinNum = TicketNum * Factor;
            AcceptCoinThread(CoinNum);  //4 等待硬币投入 开辟一个新线程
        }

        private void plus(object sender, RoutedEventArgs e)
        {
            TicketNum++;
            CoinNum = TicketNum * Factor;
            TICKETNUMBER.Text = TicketNum.ToString();
        }

        private void minus(object sender, RoutedEventArgs e)
        {
            if (TicketNum > 1)
            {
                TicketNum--;
                CoinNum = TicketNum * Factor;                
            }
            TICKETNUMBER.Text = TicketNum.ToString();
        }
        #endregion

        #region 硬币接收
        /// <summary>接收硬币线程
        /// 接收硬币函数
        /// </summary>
        /// <param name="CoinNum"></param>
        public void AcceptCoinThread(int CoinNum)
        {
            //if (threadCoinAccepter == null)
            {
                //MessageBox.Show("new coin thread");
                threadCoinAccepter = new Thread(new ParameterizedThreadStart(WaitForCoins));
                //MessageBox.Show(CoinNum.ToString());
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
            while (!COINHOPPER && true) //退币选项未被按下
            {
                //INSTANTNUMBER.Text = "5";//coinAccepter.GetCurrentCoinsNum().ToString();
                if (coinAccepter.CheckAllIn(int.Parse(CoinNum.ToString())))  //硬币数量达到要求数量 （5个）
                {
                    INSTANTCOINNUMBER.Dispatcher.Invoke(new Action(() => INSTANTCOINNUMBER.Text = coinAccepter.GetCurrentCoinsNum().ToString()));
                    //coinAccepter.ClearCurrentCoinsNum();
                    //PrintTicket("360自行车");
                    Buttonhopper.Dispatcher.Invoke(new Action(() => Buttonhopper.Visibility = System.Windows.Visibility.Collapsed));
                    ShowPrintInfo();
                    PrintTicket(TicketName, TicketNum,Factor);
                    //打印之后必须清除当前硬币数量  存在bug！！
                    coinAccepter.ClearCurrentCoinsNum();
                    ButtonInsert.Dispatcher.Invoke(new Action(() => ButtonInsert.Visibility = System.Windows.Visibility.Visible));

                    coinAccepter.ClearCurrentCoinsNum();

                    TicketNum = 1;
                    CoinNum = 5;
                    //FlyoutTool.CloseButtonVisibility = System.Windows.Visibility.Hidden;
                    INSTANTCOINNUMBER.Dispatcher.Invoke(new Action(() => INSTANTCOINNUMBER.Text = "0"));
                    TICKETNUMBER.Dispatcher.Invoke(new Action(() => TICKETNUMBER.Text = "1"));
                    
                    FlyoutTool.Dispatcher.Invoke(new Action(() =>  FlyoutTool.IsPinned = false));
                    ButtonPlus.Dispatcher.Invoke(new Action(() => ButtonPlus.Visibility = System.Windows.Visibility.Visible));
                    ButtonMinus.Dispatcher.Invoke(new Action(() => ButtonMinus.Visibility = System.Windows.Visibility.Visible));
                    break;
                }
                else
                {
                    if (PreCoinNum != coinAccepter.GetCurrentCoinsNum())
                    {
                        PreCoinNum = coinAccepter.GetCurrentCoinsNum();
                        INSTANTCOINNUMBER.Dispatcher.Invoke(new Action(() => INSTANTCOINNUMBER.Text = coinAccepter.GetCurrentCoinsNum().ToString()));
                    }
                }
            }
            //MessageBox.Show("threadabort");
            //停止投币器线程
            Console.WriteLine( "current thread " + threadCoinAccepter.Name);
            threadCoinAccepter.Abort();
            Console.WriteLine( "current thread " + threadCoinAccepter.ToString());
        }

        private void ShowPrintInfo()
        {
            TicketPrintStatus.Dispatcher.Invoke(new Action(() => TicketPrintStatus.Visibility=System.Windows.Visibility.Visible));
            Thread.Sleep(2000);
            TicketPrintStatus.Dispatcher.Invoke(new Action(() => TicketPrintStatus.Visibility = System.Windows.Visibility.Collapsed));
        }

        # endregion

        #region 退币
        private void Hopper()
        {
            Thread.Sleep(1000);
            Buttonhopper.Visibility = System.Windows.Visibility.Collapsed; //取消退币按钮，改为直接点击退出并退币。
            ButtonInsert.Visibility = System.Windows.Visibility.Visible;
            ButtonPlus.Visibility = System.Windows.Visibility.Visible;
            ButtonMinus.Visibility = System.Windows.Visibility.Visible;
            COINHOPPER = true;//退币状态置位1
            HopperThread();
            COINHOPPER = false;//退币状态置位0
        }

        private void HopperThread()           
        //if (threadCoinAccepter == null)
        {
            //MessageBox.Show("new coin thread");
            threadCoinHopper = new Thread(new ParameterizedThreadStart(CoinHopper));
            //MessageBox.Show(CoinNum.ToString());
            CoinNum = coinAccepter.GetCurrentCoinsNum();
            threadCoinHopper.Start(CoinNum);
        }

        private void CoinHopper(object CoinNum)
        {
            try
            {
                ButtonInsert.Visibility = System.Windows.Visibility.Visible;
                Buttonhopper.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch { }
            try
            {
                coinAccepter.setReject();//禁止收币功能
                //投币线程强制终止
                threadCoinAccepter.Abort();
            }
            catch { }
            //退币
            //调用退币模块
            CoinNum = coinAccepter.GetCurrentCoinsNum();
           // MessageBox.Show("退币数量为 " + CoinNum.ToString() + "\n点击确认后开始退币");
            if ((int)CoinNum != 0)
            {
                coinHopper.returnCoin((int)CoinNum);
                TicketNum = 1;
                CoinNum = 5;
                //TICKETNUMBER.Text = "1";
                //CoinHopper(int CoinNum); //退币 数量为CoinNum
                //还需要清零当前硬币数量
                coinAccepter.ClearCurrentCoinsNum();
                FlyoutTool.Dispatcher.Invoke(new Action(() => INSTANTCOINNUMBER.Text = "0"));
            }
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

