using System;
using TCTData.Enums;
using TCTData.Events;
namespace TCTData
{
    public static class ResetManager
    {
        public static bool dailyReset = false;
        public static bool weeklyReset = false;
        public static void ResetDailyData()
        {
            /*resets dungeons runs*/
            int tc = 1;
            foreach (var c in Data.CharList)
            {
                if (Data.AccountList.Find(a => a.Id == c.AccountId).TeraClub)
                {
                    tc = 2;
                }
                else
                {
                    tc = 1;
                }

                foreach (var d in c.Dungeons)
                {
                    if (d.Name.Equals("CA") || d.Name.Equals("AH") || d.Name.Equals("GL") || d.Name.Equals("EA"))
                    {
                        if (Data.DungList.Find(x => x.ShortName == d.Name) != null)
                        {
                            d.Runs = Data.DungList.Find(x => x.ShortName == d.Name).MaxBaseRuns;
                        }

                    }
                    else if (d.Name.Equals("HH"))
                    {
                        if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
                        {
                            if (Data.DungList.Find(x => x.ShortName == d.Name) != null)
                            {
                                d.Runs = 0;
                            }
                        }
                    }
                    else
                    {
                        if (Data.DungList.Find(x => x.ShortName == d.Name) != null)
                        {
                            d.Runs = Data.DungList.Find(x => x.ShortName == d.Name).MaxBaseRuns * tc;
                        }
                    }
                }
            }

            /*reset dailies*/
            foreach (var c in Data.CharList)
            {
                c.Dailies = 8;
            }

        }
        public static void ResetWeeklyData()
        {
            foreach (var c in Data.CharList)
            {
                c.Weekly = 0;
            }
        }
        public static void TryReset()
        {
            if (dailyReset)
            {
                ResetDailyData();
                EventsWrapper.SendLogEntry(new LogEntryEventArgs("Daily data has been reset."));
                EventsWrapper.SendNotification(new NotificationEventArgs("Daily data has been reset.", NotificationImage.Default, NotificationType.Standard, TCTData.Colors.SolidGreen, true, true, false));
                //UI.UpdateLog("Daily data has been reset.");
                //UI.SendNotification("Daily data has been reset.", NotificationImage.Default, NotificationType.Standard, TCTData.Colors.SolidGreen, true, true, false);
                dailyReset = false;
            }
            if (weeklyReset)
            {
                ResetWeeklyData();
                //UI.UpdateLog("Weekly data has been reset.");
                //UI.SendNotification("Weekly data has been reset.", NotificationImage.Default, NotificationType.Standard, TCTData.Colors.SolidGreen, true, true, false);
                EventsWrapper.SendLogEntry(new LogEntryEventArgs("Weekly data has been reset."));
                EventsWrapper.SendNotification(new NotificationEventArgs("Weekly data has been reset.", NotificationImage.Default, NotificationType.Standard, TCTData.Colors.SolidGreen, true, true, false));

                weeklyReset = false;
            }
        }
        public static void ResetCheck()
        {
            DateTime lastReset;
            if (DateTime.Now.Hour >= TCTConstants.DAILY_RESET_HOUR)
            {
                lastReset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, TCTConstants.DAILY_RESET_HOUR, 0, 0);
            }
            else
            {
                lastReset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1, TCTConstants.DAILY_RESET_HOUR, 0, 0);
            }

            if (TCTProps.LastClosed < lastReset)
            {
                dailyReset = true;
                if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
                {
                    weeklyReset = true;
                }
            }

        }

    }
}
