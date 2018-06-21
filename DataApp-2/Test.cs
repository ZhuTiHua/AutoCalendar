using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace DataApp_2
{
    public class Test
    {
        [DllImport("MyDLL.dll", EntryPoint = "add", CallingConvention = CallingConvention.Winapi)] //引入dll，并设置字符集
        public static extern Int32 add(Int32 a, Int32 b);
    }
}
