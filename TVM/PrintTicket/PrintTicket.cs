using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TVM
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
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
