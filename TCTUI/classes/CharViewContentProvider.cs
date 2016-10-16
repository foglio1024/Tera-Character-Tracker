using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tera
{
    public class CharViewContentProvider
    {
        public Character SelectedChar { get; set; }

        public int getCharIndex()
        {
            return TeraLogic.CharList.IndexOf(SelectedChar);
        }
        
    }
}
