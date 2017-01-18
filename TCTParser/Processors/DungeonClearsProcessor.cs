using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTParser
{
    class DungeonClearsProcessor : ListParser
    {

        const int ID_OFFSET = 0;
        const int COUNT_OFFSET = 4 * 2;

        List<DungClear> ClearList = new List<DungClear>();

        public void UpdateClears(string p)
        {
            ClearList.Clear();
            ParseClearList(p);

            foreach (DungClear dgClear in ClearList)
            {
                var c = new TCTData.RegionIDToName();

                if(Tera.TeraLogic.DungList.Find(x => x.Id == dgClear.ID) != null)
                {
                    string dgName = Tera.TeraLogic.DungList.Find(x => x.Id == dgClear.ID).ShortName; //find this dungeon name
                    DataParser.CurrentChar().Dungeons.Find(y => y.Name == dgName).Clears = dgClear.Clears; //update clears              
                }

            }

        }

        private void ParseClearList(string p)
        {
            var stringList = ParseList(p);
            foreach (var dung in stringList)
            {
                ClearList.Add(ParseElement(dung));
            }
        }
        private DungClear ParseElement(string e)
        {
            return new DungClear(StringUtils.Hex2BStringToInt(e.Substring(ID_OFFSET)), StringUtils.Hex2BStringToInt(e.Substring(COUNT_OFFSET)));
        }

        private class DungClear
        {
            public int ID { get; set; }
            public int Clears { get; set; }

            public DungClear(int id, int clears)
            {
                ID = id;
                Clears = clears;
            }
        }
    }
}
