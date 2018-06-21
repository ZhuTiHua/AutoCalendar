using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataApp_2
{
    public class Result
    {
        int _code;
        byte _data;

        public int code
        {
            get { return _code; }
            set { _code = value; }
        }

        public byte data
        {
            get { return _data; }
            set { _data = value; }
        }
    }
}
