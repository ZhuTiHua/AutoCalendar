using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataApp_2
{
    public class Compute
    {
        [DllImport("SampleCppWrapper.dll")]
        public static extern int Add(int n1, int n2);
        [DllImport("SampleCppWrapper.dll")]
        public static extern int Sub(int n1, int n2);
    }
}
