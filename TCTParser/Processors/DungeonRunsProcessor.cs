using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera;

namespace TCTParser
{
    class DungeonRunsProcessor : ListParser
    {
        const int RUNS_OFFSET = 12 * 2;
        const int ID_OFFSET = 0;

        List<DungCoolTime> CoolTimeList = new List<DungCoolTime>();

        public void UpdateCoolTimes(string p)
        {
            var oldList = CoolTimeList;
            CoolTimeList.Clear();
            ParseCoolTimeList(p);

            foreach (DungCoolTime dct in CoolTimeList)
            {
                if(TeraLogic.DungList.Find(x => x.Id == dct.ID) != null)
                {
                    DataParser.CurrentChar.Dungeons.Find(x => x.Name == TeraLogic.DungList.Find(y => y.Id == dct.ID).ShortName).Runs = dct.Runs;
                }
            }

            if (oldList != CoolTimeList)
            {
                UI.UpdateLog("Dungeon cooldowns list updated"); 
            }
        }

        private void ParseCoolTimeList(string p)
        {
            var stringList = ParseList(p);
            foreach (var dung in stringList)
            {
                CoolTimeList.Add(ParseElement(dung));
            }
        }
        private DungCoolTime ParseElement(string e)
        {
            return new DungCoolTime
            (
                id: StringUtils.Hex2BStringToInt(e.Substring(ID_OFFSET)),
                runs: StringUtils.Hex2BStringToInt(e.Substring(RUNS_OFFSET))
            );
        }

        private class DungCoolTime
        {
            public int ID { get; set; }
            public int Runs { get; set; }

            public DungCoolTime(int id, int runs)
            {
                ID = id;
                Runs = runs;
            }

        }
    }
}
