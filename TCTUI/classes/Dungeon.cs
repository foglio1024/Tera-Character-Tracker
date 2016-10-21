using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace Tera
{
    public enum DungeonTier
    {
        Solo,
        Tier2,
        Tier3,
        Tier4,
        Tier5
    }
    [Serializable]
    public class Dungeon
    {
        
        string shortName;
        int maxBaseRuns;
        int requiredIlvl;
        int id;
        DungeonTier tier;

        [XmlAttribute("ShortName")]
        public string ShortName { get { return shortName; } set { shortName = value; } }

        [XmlAttribute("MaxBaseRuns")]
        public int MaxBaseRuns { get { return maxBaseRuns; } set { maxBaseRuns = value; } }

        [XmlAttribute("RequiredIlvl")]
        public int RequiredIlvl { get { return requiredIlvl; } set { requiredIlvl = value; } }

        [XmlAttribute("Tier")]
        public DungeonTier Tier { get { return tier; } set { tier = value; } }

        [XmlAttribute("Id")]
        public int Id { get { return id; } set { id = value; } }

        public Dungeon() { }
        public Dungeon(string _shortName, int _runs, int _ilvl, DungeonTier _tier, int _id)
        {
            shortName = _shortName;
            maxBaseRuns = _runs;
            requiredIlvl = _ilvl;
            tier = _tier;
            id = _id;
        }

    }
}
