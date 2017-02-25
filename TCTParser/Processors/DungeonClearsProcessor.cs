using System;
using System.Collections.Generic;
using System.IO;
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
            if(GetCharId(p) != DataRouter.currentCharId)
            {
                Console.WriteLine("Current id: {0} -- Received id:{1}", DataRouter.currentCharId, GetCharId(p));
                return;
            }
            ParseClearList(p);

            foreach (DungClear dgClear in ClearList)
            {

                if(TCTData.Data.DungList.Find(x => x.Id == dgClear.ID) != null)
                {
                    string dgName = TCTData.Data.DungList.Find(x => x.Id == dgClear.ID).ShortName; //find this dungeon name

                    DataRouter.CurrentChar.Dungeons.Find(y => y.Name == dgName).Clears = dgClear.Clears; //update clears 
                }

            }

            /* DEBUG
             * 
            var filePath = Environment.CurrentDirectory + "/" + DataRouter.CurrentChar.Name + ".txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(p);
            }
            */
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
            return new DungClear
            (
                id: StringUtils.Hex2BStringToInt(e.Substring(ID_OFFSET)), 
                clears: StringUtils.Hex2BStringToInt(e.Substring(COUNT_OFFSET))
            );
        }
        private int GetCharId(string p)
        {
            return StringUtils.Hex4BStringToInt(p.Substring(16));
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
