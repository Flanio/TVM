using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;


namespace TVM
{
    public class Printer
    {
        # region 属性
        private string dt;
        #endregion
        # region 门票打印模块

        # region DLL import
        [DllImport("MsprintsdkRM.dll")]
        public static extern int SetDevname(int iDevtype, string cDevname, int iBaudrate);
        [DllImport("MsprintsdkRM.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetPrintport(string strPort, int iBaudrate);
        [DllImport("MsprintsdkRM.dll")]
        public static extern int SetInit();
        [DllImport("MsprintsdkRM.dll")]
        public static extern int SetClose();
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
        public void PrintTicket(string ticketName,int ticketNum,int factor)
        {
            if (SetPrintport("USB001", 38400).ToString().Equals("1"))
                MessageBox.Show("SetPrintport Failed");
            if (SetInit().ToString().Equals("1"))
                MessageBox.Show("SetInit Failed");
            string CticketName = ticketName;
            for(int i = 0 ; i < ticketNum ; i++)
            {
                PrintFeedline(1);
                //SetInit().ToString();
                PrintDiskbmpfile("logo2.bmp");
                PrintString("----------------------", 0);
                dt = System.DateTime.Now.ToLongDateString().ToString();
                Console.WriteLine(ticketName);
                PrintString(ticketName + "\n"+"票价 "+factor+"元"+"\n"+"有效期 " + dt + "\n"+"心脏病高血压患者禁止体验"+"\n"+"限当日指定场次有效", 0);
                //SetInit().ToString();
                PrintString("----------------------", 0);
                PrintFeedline(10);
                PrintCutpaper(0);
                Thread.Sleep(2000); //延时打印 否则出错 打印每张票时间间隔为2s
            }
            SetClose();
        }
        # endregion
    }
}
