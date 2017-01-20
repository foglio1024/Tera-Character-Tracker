using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTData.Enums
{
    public enum DungeonTier
    {
        Solo,
        Tier2,
        Tier3,
        Tier4,
        Tier5
    }

    public enum NotificationImage
    {
        Default,
        Crystalbind,
        Marks,
        Goldfinger,
        Scales,
        Connected,
        Credits
    }

    public enum NotificationType
    {
        Standard,
        Counter
    }

    public enum Class
    {
        Warrior = 0,
        Lancer = 1,
        Slayer = 2,
        Berserker = 3,
        Sorcerer = 4,
        Archer = 5,
        Priest = 6,
        Mystic = 7,
        Reaper = 8,
        Gunner = 9,
        Brawler = 10,
        Ninja = 11,
        Moon_Dancer = 12

    }
    public enum Laurel
    {
        None,
        Bronze,
        Silver,
        Gold,
        Diamond,
        Champion
    }
    public enum GuildSize
    {
        Small,
        Medium,
        Big
    }
    public enum GuildQuestStatus
    {
        Available,
        Taken,
        Completed,
        NotFound = 9999
    }

    public enum CcbNotificationMode
    {
        TeleportOnly = 0,
        EverySection = 1
    }

}
