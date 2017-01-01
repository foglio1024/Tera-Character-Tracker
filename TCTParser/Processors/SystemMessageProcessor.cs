using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TCTData.Enums;
using Tera;

namespace TCTParser.Processors
{
    internal class SystemMessageProcessor
    {
        const int DUNGEON_ENGAGED_ID = 2229;
        const int VANGUARD_COMPLETED_ID = 2952;

        const int ID_OFFSET = 8 * 2;
        const int DUNGEON_ID_OFFSET = 60 * 2;
        const int QUEST_ID_OFFSET = 50 * 2;
        const int TASK_ID_OFFSET = 68 * 2;
        public void ParseSystemMessage(string p)
        {
            string s = "";
            try
            {
                s = StringUtils.GetStringFromHex(p, ID_OFFSET, "0B00");
            }
            catch (Exception e)
            {
                s = StringUtils.GetStringFromHex(p, ID_OFFSET, "0000");
            }
            Int32.TryParse(s, out int id);

            switch (id)
            {
                case DUNGEON_ENGAGED_ID:
                    EngageDungeon(p);
                    break;
                    //    case VANGUARD_COMPLETED_ID:
                    //        if(p.Substring(TASK_ID_OFFSET,2) == "31")
                    //        {
                    //            CompleteVanguard(p);
                    //        }
                    //        break;
            }
        }
        private void EngageDungeon(string p)
        {
            UInt32.TryParse(StringUtils.GetStringFromHex(p, DUNGEON_ID_OFFSET, "0000"), out uint dungId);
            XElement t = TeraLogic.StrSheet_Dungeon.Descendants().Where(x => (string)x.Attribute("id") == dungId.ToString()).FirstOrDefault();

            var dgName = t.Attribute("string").Value;
            if (dgName != null)
            {

                Tera.UI.UpdateLog(DataParser.currentCharName + " > " + dgName + " engaged.");
                TCTNotifier.NotificationProvider.SendNotification(dgName + " engaged.");

                try
                {
                    DataParser.CurrentChar().Dungeons.Find(d => d.Name.Equals(TeraLogic.DungList.Find(dg => dg.Id == dungId).ShortName)).Runs--;
                }
                catch
                {
                    Tera.UI.UpdateLog("Dungeon not found. Can't subtract run from entry counter.");
                }

            }


        }
        private void CompleteVanguard(string p)
        {
            Console.WriteLine("Completed vanguard.");
            int questId = 0;
            Int32.TryParse(StringUtils.GetStringFromHex(p, QUEST_ID_OFFSET, "0B00"), out questId);
            XElement s = Tera.TeraLogic.EventMatching.Descendants().Where(x => (string)x.Attribute("questId") == questId.ToString()).FirstOrDefault();
            var d = s.Descendants().Where(x => (string)x.Attribute("type") == "reputationPoint").FirstOrDefault();

            if (d != null)
            {

                int addedCredits = 0;
                Int32.TryParse(d.Attribute("amount").Value, out addedCredits);
                addedCredits = addedCredits * 2;

                Tera.TeraLogic.CharList.Find(ch => ch.Name.Equals(DataParser.currentCharName)).Credits += addedCredits;

                UI.UpdateLog("Earned " + addedCredits + " Vanguard Initiative credits. Total: " + DataParser.CurrentChar().Credits + ".");
                TCTNotifier.NotificationProvider.SendNotification("Earned " + addedCredits + " Vanguard Initiative credits. \nCurrent credits: " +DataParser.CurrentChar().Credits + ".", NotificationImage.Credits, System.Windows.Media.Color.FromArgb(255, 0, 255, 100), true, false);
            }
        }

    }

}
