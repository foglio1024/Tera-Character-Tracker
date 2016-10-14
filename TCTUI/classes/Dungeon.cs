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
        Starter,
        Mid,
        MidHigh,
        High,
        Top
    }
    [Serializable]
    public class Dungeon
    {
        
        string fullName;
        string shortName;
        int maxBaseRuns;
        int requiredIlvl;
        int groupSize;
        string hex;
        DungeonTier tier;

        [XmlAttribute("Name")]
        public string FullName { get { return fullName; } set { fullName = value; } }

        [XmlAttribute("ShortName")]
        public string ShortName { get { return shortName; } set { shortName = value; } }

        [XmlAttribute("MaxBaseRuns")]
        public int MaxBaseRuns { get { return maxBaseRuns; } set { maxBaseRuns = value; } }

        [XmlAttribute("RequiredIlvl")]
        public int RequiredIlvl { get { return requiredIlvl; } set { requiredIlvl = value; } }

        [XmlAttribute("GroupSize")]
        public int GroupSize { get { return groupSize; } set { groupSize = value; } }

        [XmlAttribute("Tier")]
        public DungeonTier Tier { get { return tier; } set { tier = value; } }

        [XmlAttribute("Hex")]
        public string Hex { get { return hex; } set { hex = value; } }

        public Dungeon() { }
        public Dungeon(string _fullName, string _shortName, int _runs, int _ilvl, int _group, DungeonTier _tier, string _hex)
        {
            fullName = _fullName;
            shortName = _shortName;
            maxBaseRuns = _runs;
            requiredIlvl = _ilvl;
            groupSize = _group;
            tier = _tier;
            hex = _hex;
        }

    }
}
