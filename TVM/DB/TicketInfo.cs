using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVM
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
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
    }
}
