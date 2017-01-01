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
        public NotificationType Type { get; set; }
        public Color Color { get; set; }
        public bool IsRepeatable { get; set; }
        public bool Sound { get; set; }
        public bool IsDelivered { get; private set; }
        public bool Right { get; set; }
        public NotificationInfo(string _c, NotificationImage _img, NotificationType t, Color _co, bool _repeat, bool _sound, bool right)
        {
            Content = _c;
            Image = _img;
            Type = t;
            Color = _co;
            IsRepeatable = _repeat;
            Sound = _sound;
            Right = right;

            IsDelivered = false;
        }

        public void SetDelivered()
        {
            IsDelivered = true;
        }
    }
}
