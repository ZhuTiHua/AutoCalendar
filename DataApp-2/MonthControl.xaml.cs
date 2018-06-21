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

namespace DataApp_2
{
    /// <summary>
    /// MonthControl.xaml 的交互逻辑
    /// </summary>
    public partial class MonthControl : UserControl
    {
        int _yearid;
        int _monthid;

        public int YearId
        {
            get { return _yearid; }
            set { _yearid = value; }
        }

        public int MonthId
        {
            get { return _monthid; }
            set { _monthid = value; }
        }
        
        public MonthControl()
        {
            InitializeComponent();
        }

        public void MonthControlLoad(int year, int month)
        {
            YearId = year;
            MonthId = month;
            monthlbl.Content = string.Format("{0}年{1}月", YearId, MonthId);
            //加载天
            for(int i = 0; i < 7; i++)
            {
                WeekDayControl weekdaycontrol = new WeekDayControl();
                weekdaycontrol.LoadFirstLine(i);
                MonthPanel.Children.Add(weekdaycontrol);
            }

            //计算这个月的1号是周几
            string strDate = string.Format("{0}-{1}-01", YearId, MonthId);
            DayOfWeek weekday = Convert.ToDateTime(strDate).DayOfWeek;
            int firstemptyNum = StaticGlobal.weekdayDic[weekday.ToString()];
            for(int i = 0; i < firstemptyNum; i++)
            {
                DayControl daycontrol = new DayControl();
                daycontrol.LoadEmpty();
                MonthPanel.Children.Add(daycontrol);
            }

            for(int i = 0;i < StaticGlobal.days[MonthId]; i++)
            {
                DayControl daycontrol = new DayControl();
                daycontrol.LoadDay(YearId, MonthId, i + 1);
                MonthPanel.Children.Add(daycontrol);
            }

            for(int i = 0; i < 42 - StaticGlobal.days[MonthId] - firstemptyNum; i++)
            {
                DayControl daycontrol = new DayControl();
                daycontrol.LoadEmpty();
                MonthPanel.Children.Add(daycontrol);
            }
        }
    }
}
