using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TCTData.Enums;
using Tera;
using TCTUI;

namespace TCTParser.Processors
{
    class CreditsUpdateProcessor
    {
        const int VANGUARD_REP_ID = 609;

        const int REP_ID_OFFSET = 20 * 2;
        const int AMOUNT_OFFSET = 32 * 2;
        
        public void UpdateCredits(string p)
        {
            if(GetId(p) == VANGUARD_REP_ID)
            {
                if(DataRouter.CurrentChar.Credits != GetAmount(p))
                {
                    var difference = GetAmount(p) - DataRouter.CurrentChar.Credits;
                    DataRouter.CurrentChar.Credits = GetAmount(p);

                    if(difference > 0) //earned
                    {
                        UI.UpdateLog(DataRouter.CurrentChar.Name + " > " + "gained " + difference + " Vanguard credits. Current amount: " + DataRouter.CurrentChar.Credits + ".");

                        if (DataRouter.CurrentChar.Credits < 8500)
                        {
                            UI.SendNotification(DataRouter.CurrentChar.Name + " gained " + difference + " Vanguard Credits." + "\n" + "Current amount: " + DataRouter.CurrentChar.Credits + ".", NotificationImage.Credits, NotificationType.Standard, TCTData.Colors.BrightGreen, true, false, false);
                        }
                        else
                        {
                            UI.SendNotification(DataRouter.CurrentChar.Name + " gained " + difference + " Vanguard Credits." + "\n" + "Current amount: " + DataRouter.CurrentChar.Credits + ", you've almost reached your maximum credits.", NotificationImage.Credits, NotificationType.Standard, Colors.Orange, true, true, false);
                        }

                    }
                    else //spent
                    {
                        difference = -difference;
                        UI.UpdateLog(DataRouter.CurrentChar.Name + " > " + "spent " + difference + " Vanguard credits. Current amount: " + DataRouter.CurrentChar.Credits + ".");
                        UI.SendNotification(DataRouter.CurrentChar.Name + " spent " + difference + " Vanguard Credits." + "\n" + "Current amount: " + DataRouter.CurrentChar.Credits + ".", NotificationImage.Credits, NotificationType.Standard, TCTData.Colors.BrightRed, true, false, false);
                    }

                }
            }
        }

        int GetId(string p)
        {
            return StringUtils.Hex4BStringToInt(p.Substring(REP_ID_OFFSET));
        }

        int GetAmount(string p)
        {
            return StringUtils.Hex4BStringToInt(p.Substring(AMOUNT_OFFSET));
        }

    }
}
