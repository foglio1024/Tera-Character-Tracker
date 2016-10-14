using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tera.classes
{
    class DotParse
    {
        public static string dot2u(string s)
        {
            s.Replace(".", "_");
            return s;
        }
    }
}
