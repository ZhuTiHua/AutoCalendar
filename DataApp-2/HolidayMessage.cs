using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataApp_2
{
    [Serializable]
    public class HolidayMessage
    {
        string _day;
        byte _flag;

        public string Day
        {
            get { return _day; }
            set { _day = value; }
        }

        public byte Flag
        {
            get { return _flag; }
            set { _flag = value; }
        }

        public HolidayMessage(string day)
        {
            Day = day;
        }

        public HolidayMessage() { }
    }
}
