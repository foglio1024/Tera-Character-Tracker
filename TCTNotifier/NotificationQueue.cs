using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTNotifier
{
    public class NotificationQueue : List<NotificationInfo>
    {
        public bool Busy { get; set; } = false;

        internal event NotificationAddedEventHandler Added;
        internal event NotificationEndedEventHandler Over;

        protected virtual void OnNewNotification(EventArgs e)
        {
            Added?.Invoke(this, e, this.Last());
        }
        protected virtual void OnNotificationEnded(EventArgs e)
        {
            Over?.Invoke(this, e);
        }

        public new void Add(NotificationInfo value)
        {
            this.Insert(this.Count, value);
            OnNewNotification(EventArgs.Empty);
        }
        public void SetBusyToFalseOnEnd()
        {
            Busy = false;
            OnNotificationEnded(EventArgs.Empty);
        }
        public void SetBusy()
        {
            Busy = true;
        }
    }
}
