using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataApp_2
{
    public class StaticGlobal
    {
        //闰年和非闰年对应的天数
        public static int[] days;

        public static List<HolidayMessage> holidayList = new List<HolidayMessage>();

        public static string[] weekdays = new string[7] { "日", "一", "二", "三", "四", "五", "六" };

        public static Dictionary<string, int> weekdayDic = new Dictionary<string, int>();

        public static Dictionary<string, string> holidayDic = new Dictionary<string, string>();

        public static string[] holidays = new string[7] {"元旦", "春节", "清明", "劳动节", "端午", "中秋", "国庆"};

        public static int pickedNum;     //记录被选中的日期数

        public static List<string> pickeddays = new List<string>();

        public static bool allowPick = true;

        public static Dictionary<string, string> holidayanddate = new Dictionary<string, string>();

        public static Dictionary<DateTime, string> dateandholiday = new Dictionary<DateTime, string>();

        public static List<string> singleweekendList = new List<string>();

        public static List<string> doubleweekendList = new List<string>();  //双休起始日期，注：既是双休又是节假日就按节假日算

        public static int totalholidayNum;
    }
}
