using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DataApp_2
{
    public class Seek
    {
        [DllImport("LastCalender.dll")]
        public static extern int init(int n);

        [DllImport("LastCalender.dll")]
        public static extern int Test();

        [DllImport("LastCalender.dll")]
        public static extern void read_festival(int month, int day, int is_festival);

        [DllImport("LastCalender.dll")]
        public static extern void read_vacation(int start_month, int start_day, int end_month, int end_day, bool is_summer);

        [DllImport("LastCalender.dll")]
        public static extern void calcu_holiday();

        [DllImport("LastCalender.dll")]
        public static extern bool seek_status(int month, int day);

        [DllImport("LastCalender.dll")]
        public static extern int count_holiday();

        //以下均为测试函数

        [DllImport("LastCalender.dll")]
        public static extern int test_year();

        [DllImport("LastCalender.dll")]
        public static extern int test_festival();

        [DllImport("LastCalender.dll")]
        public static extern int test_vacation();
    }
}
