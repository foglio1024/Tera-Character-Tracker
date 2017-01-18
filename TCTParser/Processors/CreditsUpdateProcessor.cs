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
    class CreditsUpdateProcessor
    {
        const int VANGUARD_REP_ID = 609;

        const int REP_ID_OFFSET = 20 * 2;
        const int AMOUNT_OFFSET = 32 * 2;
        
        public void UpdateCredits(string p)
        {
            if(GetId(p) == VANGUARD_REP_ID)
            {
                if(DataParser.CurrentChar.Credits != GetAmount(p))
                {
                    var difference = GetAmount(p) - DataParser.CurrentChar.Credits;
                    DataParser.CurrentChar.Credits = GetAmount(p);

                    if(difference > 0) //earned
                    {
                        UI.UpdateLog(DataParser.CurrentChar.Name + " > " + "gained " + difference + " Vanguard credits. Current amount: " + DataParser.CurrentChar.Credits + ".");

                        if (DataParser.CurrentChar.Credits < 8500)
                        {
                            UI.SendNotification(DataParser.CurrentChar.Name + " gained " + difference + " Vanguard Credits." + "\n" + "Current amount: " + DataParser.CurrentChar.Credits + ".", NotificationImage.Credits, NotificationType.Standard, UI.Colors.SolidGreen, true, false, false);
                        }
                        else
                        {
                            UI.SendNotification(DataParser.CurrentChar.Name + " gained " + difference + " Vanguard Credits." + "\n" + "Current amount: " + DataParser.CurrentChar.Credits + ", you've almost reached your maximum credits.", NotificationImage.Credits, NotificationType.Standard, Colors.Orange, true, true, false);
                        }

                    }
                    else //spent
                    {
                        difference = -difference;
                        UI.UpdateLog(DataParser.CurrentChar.Name + " > " + "spent " + difference + " Vanguard credits. Current amount: " + DataParser.CurrentChar.Credits + ".");
                        UI.SendNotification(DataParser.CurrentChar.Name + " spent " + difference + " Vanguard Credits." + "\n" + "Current amount: " + DataParser.CurrentChar.Credits + ".", NotificationImage.Credits, NotificationType.Standard, Colors.Red, true, false, false);
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
