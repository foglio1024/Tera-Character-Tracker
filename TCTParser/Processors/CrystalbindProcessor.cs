using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TCTData.Enums;
using Tera;


namespace TCTParser.Processors

{
    internal class CrystalbindProcessor
    {
        const int CCB_ID = 4610;
        const int CHAR_ID_OFFSET = 4 * 2;
        const int CHAR_ID_LENGHT = 12;
        const int B_BUFF_ID_OFFSET = 20 * 2;
        const int E_BUFF_ID_OFFSET = 12 * 2;
        const int TIME_OFFSET = 24 * 2;
        bool ccbEnding = false;
        public bool Status { get; set; } = false;
        public long Time { get; set; } = 0;
        List<Buff> BuffList = new List<Buff>();

        public void Clear()
        {
            Status = false;
            Time = 0;
            BuffList.Clear();
            ccbEnding = false;
        }
        public void CheckCcb(uint locId, uint locNameId)
        {
            bool found = false;
            foreach (var buff in BuffList)
            {
                if (buff.Id == CCB_ID)
                {
                    found = true;
                    Status = true;
                    break;
                }
            }

            if (!found)
            {
                Status = false;
            }

            if (!Status)
            {
                if (TeraLogic.DungList.Find(x => x.Id == locId) != null)
                {
                    if (TeraLogic.DungList.Find(x => x.Id == locId).Tier >= DungeonTier.Tier3)
                    {
                        UI.SendNotification("Your Complete Crystalbind is off.", NotificationImage.Crystalbind, NotificationType.Standard, Colors.Red, true, true, false);
                    }
                }
                else if (TeraLogic.DungList.Find(x => x.Id == locNameId) != null)
                {
                    if (TeraLogic.DungList.Find(x => x.Id == locNameId).Tier >= DungeonTier.Tier3)
                    {
                        UI.SendNotification("Your Complete Crystalbind is off.", NotificationImage.Crystalbind, NotificationType.Standard, Colors.Red, true, true, false);
                    }
                }
            }

            else if (Time <= 1800000 && Time > 0)
            {
                if (TeraLogic.DungList.Find(x => x.Id == locId) != null)
                {
                    if (TeraLogic.DungList.Find(x => x.Id == locId).Tier >= DungeonTier.Tier3)
                    {
                        UI.SendNotification("Your Complete Crystalbind will expire soon.", NotificationImage.Crystalbind, NotificationType.Standard, Colors.Orange, true, true, false);
                    }
                }
                else if (TeraLogic.DungList.Find(x => x.Id == locNameId) != null)
                {
                    if (TeraLogic.DungList.Find(x => x.Id == locNameId).Tier >= DungeonTier.Tier3)
                    {
                        UI.SendNotification("Your Complete Crystalbind will expire soon.", NotificationImage.Crystalbind, NotificationType.Standard, Colors.Orange, true, true, false);
                    }
                }
            }
        }

        public void ParseNewBuff(string p, string currentCharId)
        {
            if (p.Substring(CHAR_ID_OFFSET, CHAR_ID_LENGHT) == currentCharId)
            {
                var b = new Buff();
                b.Id = StringUtils.Hex4BStringToInt(p.Substring(B_BUFF_ID_OFFSET, 8));
                b.TimeLeft = StringUtils.Hex4BStringToInt(p.Substring(TIME_OFFSET, 8));
                BuffList.Add(b);

                if (b.Id == CCB_ID)
                {
                    Status = true;
                    Time = b.TimeLeft;
                    ccbEnding = false;
                    DataParser.CurrentChar.Crystalbind = Time;
                }
            }
        }
        public void ParseEndingBuff(string p, string currentCharId)
        {
            if (p.Substring(CHAR_ID_OFFSET, CHAR_ID_LENGHT) == currentCharId)
            {
                var b = new Buff();
                b.Id = StringUtils.Hex4BStringToInt(p.Substring(E_BUFF_ID_OFFSET, 8));
                b.TimeLeft = 0;

                if (b.Id == CCB_ID)
                {
                    StartDeletionAsync();
                }
                else
                {
                    BuffList.Remove(BuffList.Find(x => x.Id == b.Id));
                }
            }
        }
        public void CancelDeletion()
        {
            ccbEnding = false;
        }

        async Task WaitCancelAsync()
        {
            await Task.Delay(2000);
        }
        async void StartDeletionAsync()
        {
            ccbEnding = true;
            await WaitCancelAsync();
            if (ccbEnding)
            {
                EndCcb();
            }

        }
        void EndCcb()
        {
            Status = false;
            Time = 0;
            DataParser.CurrentChar.Crystalbind = Time;
            BuffList.Clear();
            UI.SendNotification("Your Complete Crystalbind expired.", NotificationImage.Crystalbind, NotificationType.Standard, Colors.Red, true, true, false);
        }
        class Buff
        {
            public int Id { get; set; }
            public long TimeLeft { get; set; }
        }
    }
}
