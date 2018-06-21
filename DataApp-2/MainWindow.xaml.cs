using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using Newtonsoft;
using System.Runtime.Serialization.Json;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace DataApp_2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);
        static bool isDetail = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //读年份xml文件
            FileStream fs = File.Open("Year.xml", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string text = sr.ReadToEnd();
            var yearResult = XmlSerializeHelper.DeSerialize<int>(text);
            fs.Close();   //一定要加上这一句

            ////测试代码
            //int sum = Test.add(7, 8);
            //int sub = Compute.Sub(40, 4);

            int thisYear = DateTime.Now.Year;
            if(thisYear == yearResult)
            {
                //读xml文件中的假期原始数据
                FileStream fsholoiday = File.Open("HolidayOriginalData.xml", FileMode.Open, FileAccess.Read);
                StreamReader srholoiday = new StreamReader(fsholoiday);
                string textholoiday = srholoiday.ReadToEnd();
                var list = XmlSerializeHelper.DeSerialize<List<HolidayMessage>> (textholoiday);
                StaticGlobal.holidayList = list;
                fsholoiday.Close();
            }
            else
            {
                //写入年份xml文件
                using (Stream fStream = new FileStream("Year.xml", FileMode.Create,
                    FileAccess.Write, FileShare.None))
                {
                    XmlSerializer xmlFormat = new XmlSerializer(typeof(int));
                    xmlFormat.Serialize(fStream, thisYear);
                }

                //获取今年所有法定假日
                DateTime beginDate = (DateTime.Parse("2017-12-31"));
                DateTime endDate = (DateTime.Parse("2018-12-31"));
                while (beginDate < endDate)
                {
                    beginDate = beginDate.AddDays(1);
                    StaticGlobal.holidayList.Add(new HolidayMessage(beginDate.ToString("yyyy-MM-dd")));
                }
                string urlbase = "http://api.goseek.cn/Tools/holiday?date=";
                string url = string.Empty;
                string str = string.Empty;
                for (int i = 0; i < StaticGlobal.holidayList.Count; i++)
                {
                    url = urlbase + StaticGlobal.holidayList[i].Day.Replace("-", "");
                    WebRequest wRequest = WebRequest.Create(url);
                    wRequest.Method = "GET";
                    wRequest.ContentType = "text/html;charset=UTF-8";
                    WebResponse wResponse = wRequest.GetResponse();
                    Stream stream = wResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    str = reader.ReadToEnd();   //url返回的值
                    Result result = JsonToObject<Result>(str);  //注意，返回json格式为{"code":10001,"data":2}，所以Result类的属性名称必须也是code和data，不然json转对象时会出错
                    StaticGlobal.holidayList[i].Flag = result.data;
                }

                //写入xml文件
                using (Stream fStream = new FileStream("HolidayOriginalData.xml", FileMode.Create,
                    FileAccess.Write, FileShare.None))
                {
                    XmlSerializer xmlFormat = new XmlSerializer(typeof(List<HolidayMessage>));
                    xmlFormat.Serialize(fStream, StaticGlobal.holidayList);
                }
            }
            
            Seek.init(thisYear);   //初始化，new一个seek类对象
            ////测试代码
            //int test1 = Seek.test_year();
            //判断今年是否是闰年
            if (IsLeap(thisYear))   //是闰年
                StaticGlobal.days = new int[13] { 0, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            else
                StaticGlobal.days = new int[13] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            StaticGlobal.weekdayDic.Add("Sunday", 0);
            StaticGlobal.weekdayDic.Add("Monday", 1);
            StaticGlobal.weekdayDic.Add("Tuesday", 2);
            StaticGlobal.weekdayDic.Add("Wednesday", 3);
            StaticGlobal.weekdayDic.Add("Thursday", 4);
            StaticGlobal.weekdayDic.Add("Friday", 5);
            StaticGlobal.weekdayDic.Add("Saturday", 6);

            //read_festival
            for (int i = 0; i < StaticGlobal.holidayList.Count; i++)
            {
                DateTime date = Convert.ToDateTime(StaticGlobal.holidayList[i].Day);
                int month = date.Month;
                int day = date.Day;
                Seek.read_festival(month, day, StaticGlobal.holidayList[i].Flag);
            }

            ////测试代码
            //List<HolidayMessage> onetwo = (from r in StaticGlobal.holidayList where r.Flag == 0 select r).ToList();
            //int count = onetwo.Count;
            //int test2 = Seek.test_festival();

            int holidayindex = 0;
            bool isHoliday = false;
            for(int i = 0; i < StaticGlobal.holidayList.Count; i++)
            {
                switch (StaticGlobal.holidayList[i].Flag)
                {
                    case 2:
                        int m = i;
                        while (StaticGlobal.holidayList[m].Flag == 2)
                        {
                            StaticGlobal.holidayDic.Add(StaticGlobal.holidayList[m].Day,
                                StaticGlobal.holidays[holidayindex]);
                            isHoliday = true;
                            m += 1;
                        }
                        if (isHoliday)
                        {
                            holidayindex += 1;
                            isHoliday = false;
                        }
                        i = m - 1;
                        break;
                    case 1:
                        StaticGlobal.holidayDic.Add(StaticGlobal.holidayList[i].Day, "周末");
                        break;
                    default:
                        StaticGlobal.holidayDic.Add(StaticGlobal.holidayList[i].Day, "");
                        break;
                }
            }

            //中秋和国庆混在一起的情况
            //由于StaticGlobal.holidays中先中秋，所以这里只找中秋
            List<string> MiddleList = (StaticGlobal.holidayDic.Where(q => q.Value == "中秋").Select(q => q.Key)).ToList();
            if(MiddleList.Count > 7)
            {
                for(int i = 0; i < MiddleList.Count; i++)
                {
                    StaticGlobal.holidayDic[MiddleList[i]] = "中秋&&国庆";
                }
            }

            //初始化
            for (int i = 0; i < StaticGlobal.holidays.Length; i++)
            {
                StaticGlobal.holidayanddate.Add(StaticGlobal.holidays[i], null);
            }

            //加载日历
            for (int i = 1; i <= 12; i++)
            {
                MonthControl monthcontrol = new MonthControl();
                monthcontrol.MonthControlLoad(thisYear, i);
                YearPanel.Children.Add(monthcontrol);
            }
        }

        private static bool IsLeap(int year)
        {
            if (year % 4 == 0)
            {
                if (year % 100 == 0)
                {
                    if (year % 400 == 0)
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
            else
            {
                return false;
            }
        }

        //将Jason对象转化为类对象
        public static T JsonToObject<T>(string jsonText)
        {
            DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonText));
            T obj = (T)s.ReadObject(ms);
            ms.Dispose();
            return obj;
        }

        private void CloseImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CloseImage_MouseEnter(object sender, MouseEventArgs e)
        {
            Uri uri = new Uri(@"关闭红.png", UriKind.Relative);
            ImageBrush ib = new ImageBrush();
            CloseImage.Source = new BitmapImage(uri);
        }

        private void CloseImage_MouseLeave(object sender, MouseEventArgs e)
        {
            Uri uri = new Uri(@"关闭.png", UriKind.Relative);
            ImageBrush ib = new ImageBrush();
            CloseImage.Source = new BitmapImage(uri);
        }

        private void ReductionImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void ReductionImage_MouseEnter(object sender, MouseEventArgs e)
        {
            if(WindowState == WindowState.Normal)
            {
                Uri uri = new Uri(@"最大化红.png", UriKind.Relative);
                ImageBrush ib = new ImageBrush();
                ReductionImage.Source = new BitmapImage(uri);
            }
            else
            {
                Uri uri = new Uri(@"窗口还原红.png", UriKind.Relative);
                ImageBrush ib = new ImageBrush();
                ReductionImage.Source = new BitmapImage(uri);
            }
        }

        private void ReductionImage_MouseLeave(object sender, MouseEventArgs e)
        {
            if(WindowState == WindowState.Normal)
            {
                Uri uri = new Uri(@"最大化.png", UriKind.Relative);
                ImageBrush ib = new ImageBrush();
                ReductionImage.Source = new BitmapImage(uri);
            }
            else
            {
                Uri uri = new Uri(@"窗口还原.png", UriKind.Relative);
                ImageBrush ib = new ImageBrush();
                ReductionImage.Source = new BitmapImage(uri);
            }
        }

        private void MinImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Minimized;
        }

        private void MinImage_MouseEnter(object sender, MouseEventArgs e)
        {
            Uri uri = new Uri(@"最小化红.png", UriKind.Relative);
            ImageBrush ib = new ImageBrush();
            MinImage.Source = new BitmapImage(uri);
        }

        private void MinImage_MouseLeave(object sender, MouseEventArgs e)
        {
            Uri uri = new Uri(@"最小化.png", UriKind.Relative);
            ImageBrush ib = new ImageBrush();
            MinImage.Source = new BitmapImage(uri);
        }

        private void CommitBtn_Click(object sender, RoutedEventArgs e)
        {
            holidayPanel.Visibility = Visibility.Collapsed;
            //StaticGlobal.pickeddays.Clear();
            StaticGlobal.pickedNum = 0;
            foreach (MonthControl monthcontrol in YearPanel.Children)
            {
                foreach(var item in monthcontrol.MonthPanel.Children)
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
            DateTime date;
            int startmonth;
            int startday;
            int endmonth;
            int endday;
            for (int i = 0; i < 4; i += 2)
            {
                date = Convert.ToDateTime(StaticGlobal.pickeddays[i]);
                startmonth = date.Month;
                startday = date.Day;
                date = Convert.ToDateTime(StaticGlobal.pickeddays[i + 1]);
                endmonth = date.Month;
                endday = date.Day;
                if (i < 1)  //寒假
                    Seek.read_vacation(startmonth, startday, endmonth, endday, false);
                else
                    Seek.read_vacation(startmonth, startday, endmonth, endday, true);
            }

            ProgressGrid.Visibility = Visibility.Visible;
            YearPanel.Visibility = Visibility.Collapsed;
            CalculateHoliday();

            ProgressGrid.Visibility = Visibility.Collapsed;
            YearPanel.Visibility = Visibility.Visible;
            MessageBox.Show("加载完成！", "提示");
            CommitBtn.IsEnabled = false;
            RenewBtn.IsEnabled = true;
            StaticGlobal.allowPick = false;
            detailImage.Visibility = Visibility.Visible;
            StaticGlobal.pickeddays.Clear();
        }

        public void CalculateHoliday()
        {
            DownLoadImport();

            //计算假期
            Seek.calcu_holiday();
            StaticGlobal.totalholidayNum = Seek.count_holiday();

            MainWindow MW = (MainWindow)Application.Current.Windows[0];
            foreach (MonthControl monthcontrol in MW.YearPanel.Children)
            {
                foreach (var item in monthcontrol.MonthPanel.Children)
                {
                    DayControl daycontrol = item as DayControl;
                    if (daycontrol != null)
                    {
                        if (!daycontrol.IsEmpty)   //不是空格
                        {
                            DateTime date = Convert.ToDateTime(daycontrol.Date);
                            int month = date.Month;
                            int day = date.Day;
                            bool isHoliday = Seek.seek_status(month, day);
                            if (isHoliday)
                            {
                                Color color = (Color)ColorConverter.ConvertFromString("LightBlue");
                                SolidColorBrush brush = new SolidColorBrush(color);
                                daycontrol.Background = brush;

                                //计算假期详情(放假原因)，三种情况：工作日、单（双）休日、节假日，这里不管寒暑假，寒暑假直接根据选择打印出来
                                if(daycontrol.HolidayTxt.Text != "")
                                {
                                    StaticGlobal.dateandholiday.Add(Convert.ToDateTime(daycontrol.Date), daycontrol.HolidayTxt.Text);
                                }
                            }
                        }
                    }
                }
            }

            //判断单（双）休日、节假日，StaticGlobal.dateandholiday只包含周末和节假日
            Dictionary<string, bool> holidayFlags = new Dictionary<string, bool>();
            for (int i = 0; i < StaticGlobal.holidays.Length; i++)
            {
                holidayFlags.Add(StaticGlobal.holidays[i], false);
            }

            foreach (KeyValuePair<DateTime, string> kvp in StaticGlobal.dateandholiday)
            {
                if(kvp.Value == "周末")
                {
                    DateTime yesterday = kvp.Key.AddDays(-1);
                    DateTime tomorrow = kvp.Key.AddDays(1);
                    if (StaticGlobal.dateandholiday.ContainsKey(tomorrow))
                    {
                        if (StaticGlobal.dateandholiday[tomorrow] == "周末")
                            StaticGlobal.doubleweekendList.Add(kvp.Key.Year + "-" + kvp.Key.Month.ToString().PadLeft(2, '0') + "-" + kvp.Key.Day.ToString().PadLeft(2, '0'));
                        else
                        {
                            if (StaticGlobal.dateandholiday.ContainsKey(yesterday))
                            {
                                if (StaticGlobal.dateandholiday[yesterday] != "周末")
                                    StaticGlobal.singleweekendList.Add(kvp.Key.Year + "-" + kvp.Key.Month.ToString().PadLeft(2, '0') + "-" + kvp.Key.Day.ToString().PadLeft(2, '0'));
                                else { }
                            }
                        }
                    }
                    else
                    {
                        if (StaticGlobal.dateandholiday.ContainsKey(yesterday))
                        {
                            if (StaticGlobal.dateandholiday[yesterday] != "周末")
                                StaticGlobal.singleweekendList.Add(kvp.Key.Year + "-" + kvp.Key.Month.ToString().PadLeft(2, '0') + "-" + kvp.Key.Day.ToString().PadLeft(2, '0'));
                            else { }
                        }
                        else
                            StaticGlobal.singleweekendList.Add(kvp.Key.Year + "-" + kvp.Key.Month.ToString().PadLeft(2, '0') + "-" + kvp.Key.Day.ToString().PadLeft(2, '0'));
                    }
                }
                else
                {
                   if(holidayFlags[kvp.Value] == false)
                    {
                        holidayFlags[kvp.Value] = true;
                        StaticGlobal.holidayanddate[kvp.Value] = kvp.Key.Year.ToString() + "-" + kvp.Key.Month.ToString().PadLeft(2, '0') + "-" + kvp.Key.Day.ToString().PadLeft(2, '0');
                    }
                    else { }
                }
            }
            //显示
            totalHolidayTxt.Text = StaticGlobal.totalholidayNum.ToString();
            //元旦非常特殊，要单独考虑
            //StaticGlobal.holidayanddate["元旦"] = "2018-01-01";   //c++判断有误，这句应该删掉
            DateTime yuandan = Convert.ToDateTime(StaticGlobal.holidayanddate["元旦"]);
            DateTime second = yuandan.AddDays(1);
            DateTime third = yuandan.AddDays(2);
            if (StaticGlobal.holidayDic[second.Year.ToString() + "-" + second.Month.ToString().PadLeft(2, '0') + "-" + second.Day.ToString().PadLeft(2, '0')] == "元旦")
            {
                if (StaticGlobal.holidayDic[third.Year.ToString() + "-" + third.Month.ToString().PadLeft(2, '0') + "-" + third.Day.ToString().PadLeft(2, '0')] == "元旦") { }   //什么都不做，就是元旦的第一天
                else
                    StaticGlobal.holidayanddate["元旦"] = yuandan.AddDays(-1).Year.ToString() 
                        + "-" 
                        + yuandan.AddDays(-1).Month.ToString().PadLeft(2, '0') 
                        + "-" + yuandan.AddDays(-1).Day.ToString().PadLeft(2, '0');
            }
            else
                StaticGlobal.holidayanddate["元旦"] = yuandan.AddDays(-2).Year.ToString()
                        + "-"
                        + yuandan.AddDays(-2).Month.ToString().PadLeft(2, '0')
                        + "-" + yuandan.AddDays(-2).Day.ToString().PadLeft(2, '0');

            //节日
            yuandanStart.Text = StaticGlobal.holidayanddate["元旦"];
            //yuandanStart.Text = Convert.ToDateTime(StaticGlobal.holidayanddate["元旦"]).Year.ToString() 
            //    + "-" 
            //    + Convert.ToDateTime(StaticGlobal.holidayanddate["元旦"]).Month.ToString().PadLeft(2, '0') 
            //    + "-" 
            //    + Convert.ToDateTime(StaticGlobal.holidayanddate["元旦"]).Day.ToString().PadLeft(2, '0');
            yuandanEnd.Text = (Convert.ToDateTime(StaticGlobal.holidayanddate["元旦"]).AddDays(2)).Year.ToString() 
                + "-" 
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["元旦"]).AddDays(2)).Month.ToString().PadLeft(2, '0') 
                + "-" 
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["元旦"]).AddDays(2)).Day.ToString().PadLeft(2, '0');
            chunjieStart.Text = StaticGlobal.holidayanddate["春节"];
            chunjieEnd.Text = (Convert.ToDateTime(StaticGlobal.holidayanddate["春节"]).AddDays(6)).Year.ToString()
                + "-"
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["春节"]).AddDays(6)).Month.ToString().PadLeft(2, '0')
                + "-"
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["春节"]).AddDays(6)).Day.ToString().PadLeft(2, '0');
            qingmingStart.Text = StaticGlobal.holidayanddate["清明"];
            qingmingEnd.Text = (Convert.ToDateTime(StaticGlobal.holidayanddate["清明"]).AddDays(2)).Year.ToString() 
                + "-" 
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["清明"]).AddDays(2)).Month.ToString().PadLeft(2, '0') 
                + "-" 
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["清明"]).AddDays(2)).Day.ToString().PadLeft(2, '0');
            laodongStart.Text = StaticGlobal.holidayanddate["劳动节"];
            laodongEnd.Text = (Convert.ToDateTime(StaticGlobal.holidayanddate["劳动节"]).AddDays(2)).Year.ToString()
                + "-"
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["劳动节"]).AddDays(2)).Month.ToString().PadLeft(2, '0')
                + "-"
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["劳动节"]).AddDays(2)).Day.ToString().PadLeft(2, '0');
            duanwuStart.Text = StaticGlobal.holidayanddate["端午"];
            duanwuEnd.Text = (Convert.ToDateTime(StaticGlobal.holidayanddate["端午"]).AddDays(2)).Year.ToString()
                + "-"
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["端午"]).AddDays(2)).Month.ToString().PadLeft(2, '0')
                + "-"
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["端午"]).AddDays(2)).Day.ToString().PadLeft(2, '0');
            zhongqiuStart.Text = StaticGlobal.holidayanddate["中秋"];
            zhongqiuEnd.Text = (Convert.ToDateTime(StaticGlobal.holidayanddate["中秋"]).AddDays(2)).Year.ToString()
                + "-"
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["中秋"]).AddDays(2)).Month.ToString().PadLeft(2, '0')
                + "-"
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["中秋"]).AddDays(2)).Day.ToString().PadLeft(2, '0');
            guoqingStart.Text = StaticGlobal.holidayanddate["国庆"];
            guoqingEnd.Text = (Convert.ToDateTime(StaticGlobal.holidayanddate["国庆"]).AddDays(6)).Year.ToString()
                + "-"
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["国庆"]).AddDays(6)).Month.ToString().PadLeft(2, '0')
                + "-"
                + (Convert.ToDateTime(StaticGlobal.holidayanddate["国庆"]).AddDays(6)).Day.ToString().PadLeft(2, '0');
            //暑假
            shujiaStart.Text = StaticGlobal.pickeddays[0];
            shujiaEnd.Text = StaticGlobal.pickeddays[1];
            //寒假
            hanjiaStart.Text = StaticGlobal.pickeddays[2];
            hanjiaEnd.Text = StaticGlobal.pickeddays[3];
            //单休
            for(int i = 0; i < StaticGlobal.singleweekendList.Count; i++)
            {
                singleweekendList.Items.Add(StaticGlobal.singleweekendList[i]);
            }
            //双休
            for(int i = 0; i < StaticGlobal.doubleweekendList.Count; i++)
            {
                doubleweekendList.Items.Add(StaticGlobal.doubleweekendList[i] + " —— " 
                    + (Convert.ToDateTime(StaticGlobal.doubleweekendList[i]).AddDays(1)).Year 
                    + "-" 
                    + (Convert.ToDateTime(StaticGlobal.doubleweekendList[i]).AddDays(1)).Month.ToString().PadLeft(2, '0') 
                    + "-" 
                    + (Convert.ToDateTime(StaticGlobal.doubleweekendList[i]).AddDays(1)).Day.ToString().PadLeft(2, '0'));
            }
        }

        private void DownLoadImport()
        {
            LoadProgressBar.Maximum = 100;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(LoadProgressBar.SetValue);
            for (int i = 0; i < 100; i++)
            {
                Dispatcher.Invoke(
                    updatePbDelegate,
                    DispatcherPriority.Background,
                    new object[]
                    {
                        System.Windows.Controls.Primitives.RangeBase.ValueProperty, Convert.ToDouble(i + 1)
                    });
            }
        }

        public void ChangeButton()
        {
            if(StaticGlobal.pickedNum == 4)
            {
                CommitBtn.IsEnabled = true;
                summerbeginTxt.Text = StaticGlobal.pickeddays[0];
                summerendTxt.Text = StaticGlobal.pickeddays[1];
                winterbeginTxt.Text = StaticGlobal.pickeddays[2];
                winterendTxt.Text = StaticGlobal.pickeddays[3];
                holidayPanel.Visibility = Visibility.Visible;
            }
            else
            {
                CommitBtn.IsEnabled = false;
                summerbeginTxt.Text = "";
                summerendTxt.Text = "";
                winterbeginTxt.Text = "";
                winterendTxt.Text = "";
                holidayPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void RenewBtn_Click(object sender, RoutedEventArgs e)
        {
            Details.Width = new GridLength(0);
            isDetail = false;
            Uri uri = new Uri(@"查看详情.png", UriKind.Relative);
            ImageBrush ib = new ImageBrush();
            detailImage.Source = new BitmapImage(uri);
            tooltipTxt.Text = "查看详情";

            StaticGlobal.pickeddays.Clear();
            StaticGlobal.holidayanddate.Clear();
            StaticGlobal.dateandholiday.Clear();
            StaticGlobal.singleweekendList.Clear();
            StaticGlobal.doubleweekendList.Clear();
            StaticGlobal.totalholidayNum = 0;

            StaticGlobal.pickedNum = 0;
            summerbeginTxt.Text = "";
            summerendTxt.Text = "";
            winterbeginTxt.Text = "";
            winterendTxt.Text = "";
            holidayPanel.Visibility = Visibility.Collapsed;
            detailImage.Visibility = Visibility.Collapsed;
            YearPanel.Children.Clear();
            int thisYear = DateTime.Now.Year;
            for (int i = 1; i <= 12; i++)
            {
                MonthControl monthcontrol = new MonthControl();
                monthcontrol.MonthControlLoad(thisYear, i);
                YearPanel.Children.Add(monthcontrol);
            }
            CommitBtn.IsEnabled = false;
            RenewBtn.IsEnabled = false;
            StaticGlobal.allowPick = true;
        }

        private void detailImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDetail)
            {
                Details.Width = new GridLength(360);
                Uri uri = new Uri(@"收起.png", UriKind.Relative);
                ImageBrush ib = new ImageBrush();
                detailImage.Source = new BitmapImage(uri);
                isDetail = true;
            }
            else
            {
                Details.Width = new GridLength(0);
                Uri uri = new Uri(@"查看详情.png", UriKind.Relative);
                ImageBrush ib = new ImageBrush();
                detailImage.Source = new BitmapImage(uri);
                isDetail = false;
            }
        }

        private void detailImage_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isDetail)
            {
                Uri uri = new Uri(@"查看详情蓝.png", UriKind.Relative);
                ImageBrush ib = new ImageBrush();
                detailImage.Source = new BitmapImage(uri);
                tooltipTxt.Text = "查看详情";
            }
            else
            {
                Uri uri = new Uri(@"收起蓝.png", UriKind.Relative);
                ImageBrush ib = new ImageBrush();
                detailImage.Source = new BitmapImage(uri);
                tooltipTxt.Text = "收起";
            }
        }

        private void detailImage_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!isDetail)
            {
                Uri uri = new Uri(@"查看详情.png", UriKind.Relative);
                ImageBrush ib = new ImageBrush();
                detailImage.Source = new BitmapImage(uri);
                tooltipTxt.Text = "查看详情";
            }
            else
            {
                Uri uri = new Uri(@"收起.png", UriKind.Relative);
                ImageBrush ib = new ImageBrush();
                detailImage.Source = new BitmapImage(uri);
                tooltipTxt.Text = "收起";
            }
        }
    }
}
