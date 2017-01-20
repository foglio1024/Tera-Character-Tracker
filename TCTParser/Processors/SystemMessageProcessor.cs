using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TCTData.Enums;
using Tera;
using Tera.Game;

namespace TCTParser
{
    public class SystemMessageProcessor
    {
        const int DUNGEON_ENGAGED_ID = 2229;
        const int VANGUARD_COMPLETED_ID = 2952;

        const int ID_OFFSET = 8 * 2;
        const int DUNGEON_ID_OFFSET = 60 * 2;
        const int QUEST_ID_OFFSET = 50 * 2;
        const int TASK_ID_OFFSET = 68 * 2;

        public void ParseSystemMessage(string p)
        {
            switch (DataParser.SystemOpCodeNamer.GetName(GetSystemOpCode(p)))
            {
                case "SMT_GRANT_DUNGEON_COOLTIME_AND_COUNT":
                    EngageDungeon(p);
                    break;
            }
        }
        
        private ushort GetSystemOpCode(string p)
        {
            string s = "";
            try
            {
                s = StringUtils.GetStringFromHex(p, ID_OFFSET, "0B00");
            }
            catch (Exception)
            {
                s = StringUtils.GetStringFromHex(p, ID_OFFSET, "0000");
            }
            ushort id = 0;
            UInt16.TryParse(s, out id);

            return id;
        }

        private void EngageDungeon(string p)
        {
            uint dungId = 0;
            UInt32.TryParse(StringUtils.GetStringFromHex(p, DUNGEON_ID_OFFSET, "0000"), out dungId);
            XElement t = TCTData.TCTDatabase.StrSheet_Dungeon.Descendants().Where(x => (string)x.Attribute("id") == dungId.ToString()).FirstOrDefault();

            var dgName = t.Attribute("string").Value;
            if (dgName != null)
            {

                UI.UpdateLog(DataParser.currentCharName + " > " + dgName + " engaged.");
                UI.SendDefaultNotification(dgName + " engaged.");

                try
                {
                    DataParser.CurrentChar.Dungeons.Find(d => d.Name.Equals(TeraLogic.DungList.Find(dg => dg.Id == dungId).ShortName)).Runs--;
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
            XElement s = TCTData.TCTDatabase.EventMatching.Descendants().Where(x => (string)x.Attribute("questId") == questId.ToString()).FirstOrDefault();
            var d = s.Descendants().Where(x => (string)x.Attribute("type") == "reputationPoint").FirstOrDefault();

            if (d != null)
            {

                int addedCredits = 0;
                Int32.TryParse(d.Attribute("amount").Value, out addedCredits);
                addedCredits = addedCredits * 2;

                Tera.TeraLogic.CharList.Find(ch => ch.Name.Equals(DataParser.currentCharName)).Credits += addedCredits;

                UI.UpdateLog("Earned " + addedCredits + " Vanguard Initiative credits. Total: " + DataParser.CurrentChar.Credits + ".");
                UI.SendNotification("Earned " + addedCredits + " Vanguard Initiative credits. \nCurrent credits: " +DataParser.CurrentChar.Credits + ".", NotificationImage.Credits, NotificationType.Standard, UI.Colors.SolidGreen, true, false, false);
            }
        }

    }

}
