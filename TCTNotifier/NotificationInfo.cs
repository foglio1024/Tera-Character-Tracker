using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TCTData.Enums;

namespace TCTNotifier
{
    public class NotificationInfo
    {
        public string Content { get; set; }
        public NotificationImage Image { get; set; }
        public Color Color { get; set; }
        public bool IsRepeatable { get; set; }
        public bool Sound { get; set; }
        public NotificationInfo(string _c, NotificationImage _img, Color _co, bool _repeat, bool _sound)
        {
            Content = _c;
            Image = _img;
            Color = _co;
            IsRepeatable = _repeat;
            Sound = _sound;
        }
    }
}
