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
    /// DayControl.xaml 的交互逻辑
    /// </summary>
    public partial class DayControl : UserControl
    {
        int _yearid;
        int _monthid;
        int _dayid;
        string _date;
        bool _isempty;   //是否是空格
        bool _ispicked;  //是否被选中

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

        public int DayId
        {
            get { return _dayid; }
            set { _dayid = value; }
        }

        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public bool IsEmpty
        {
            get { return _isempty; }
            set { _isempty = value; }
        }

        public bool IsPicked
        {
            get { return _ispicked; }
            set { _ispicked = value; }
        }

        public DayControl()
        {
            InitializeComponent();
        }

        public void LoadEmpty()
        {
            Color color = (Color)ColorConverter.ConvertFromString("LightGray");
            SolidColorBrush brush = new SolidColorBrush(color);
            this.Background = brush;
            IsEmpty = true;
        }

        public void LoadDay(int yeadid, int monthid, int dayid)
        {
            YearId = yeadid;
            MonthId = monthid;
            DayId = dayid;
            Date = YearId.ToString() + "-" + MonthId.ToString().PadLeft(2, '0') + "-" + DayId.ToString().PadLeft(2, '0');
            HolidayTxt.Text = StaticGlobal.holidayDic[Date];
            DayTxt.Text = dayid.ToString();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (StaticGlobal.allowPick)
            {
                MainWindow MW = (MainWindow)Application.Current.Windows[0];
                if (!IsEmpty)   //不是空格
                {
                    if (!IsPicked)
                    {
                        if (StaticGlobal.pickedNum < 4)
                        {
                            Color color = (Color)ColorConverter.ConvertFromString("Pink");
                            SolidColorBrush brush = new SolidColorBrush(color);
                            this.Background = brush;
                            IsPicked = true;
                            StaticGlobal.pickedNum += 1;
                            if (StaticGlobal.pickedNum == 4)
                            {
                                foreach (MonthControl monthcontrol in MW.YearPanel.Children)
                                {
                                    foreach (var item in monthcontrol.MonthPanel.Children)
                                    {
                                        DayControl daycontrol = item as DayControl;
                                        if (daycontrol != null)
                                        {
                                            if (daycontrol.IsPicked)
                                            {
                                                StaticGlobal.pickeddays.Add(daycontrol.Date);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("只能选择4个时间作为寒暑假起始！", "提示");
                        }
                    }
                    else
                    {
                        Color color = (Color)ColorConverter.ConvertFromString("White");
                        SolidColorBrush brush = new SolidColorBrush(color);
                        this.Background = brush;
                        IsPicked = false;
                        StaticGlobal.pickedNum -= 1;
                    }
                    MW.ChangeButton();
                }
                else   //啥都不做
                {
                }
            }
            else
            {
            }
        }
    }
}
