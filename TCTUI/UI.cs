using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tera
{
    public static class UI
    {
        static public TeraMainWindow MainWin;
        static public void UpdateLog(string data)
        {
            MainWin.UpdateLog(data);
        }
    }
}
