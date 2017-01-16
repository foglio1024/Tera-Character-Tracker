using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tera;

namespace TCTParser.Processors
{
    internal class InventoryProcessor
    {
        const int FIRST_POINTER = 6;
        const int ID_OFFSET = 8 * 2;
        const int POSITION_OFFSET = 32 * 2;
        const int AMOUNT_OFFSET = 36 * 2;
        const int AMOUNT_OFFSET_FROM_ID = 56;
        const int MULTIPLE_FLAG = 25 * 2;
        const int HEADER_LENGHT = 61 * 2;
        const int ILVL_OFFSET = 35 * 2;

        public const int MARK_ID = 151643;
        public const int GFIN_ID = 566;
        public const int SCALE_ID = 45474;

        public int MarksOfValor { get; private set; }
        public int GoldfingerTokens { get; private set; }
        public int DragonwingScales { get; private set; }


        List<string> itemStrings = new List<string>();
        List<int> indexesArray = new List<int>();
        List<InventoryItem> itemsList = new List<InventoryItem>();

        public bool multiplePackets = false;
        public bool justLoggedIn = true;
        public string p1;
        public string p2;

        public void Clear()
        {
            multiplePackets = false;
            p1 = null;
            p2 = null;
            itemsList.Clear();
            itemStrings.Clear();
            indexesArray.Clear();
            MarksOfValor = 0;
            GoldfingerTokens = 0;
            DragonwingScales = 0;
        }

        private void MergeInventory()
        {
            if (p2 != null)
            {
                fillItemList(p1);
                indexesArray.Clear();
                itemStrings.Clear();
                fillItemList(p2);
            }
            else
            {
                fillItemList(p1);
            }
        }

        public void ParseInventory()
        {
            MergeInventory();
            GetTokensAmounts();
        }

        private void GetTokensAmounts()
        {
            if (itemsList.Find(x => x.Id == GFIN_ID) != null)
            {
                GoldfingerTokens = itemsList.Find(x => x.Id == GFIN_ID).Amount;
            }

            if (itemsList.Find(x => x.Id == MARK_ID) != null)
            {
                MarksOfValor = itemsList.Find(x => x.Id == MARK_ID).Amount;
            }

            if (itemsList.Find(x => x.Id == SCALE_ID) != null)
            {
                DragonwingScales = itemsList.Find(x => x.Id == SCALE_ID).Amount;
            }       
        }

        public int GetItemLevel(string content)
        {
            return StringUtils.Hex2BStringToInt(content.Substring(ILVL_OFFSET, 4));
        }
        void fillIndexesArray(string content)
        {
            int currentPointer = FIRST_POINTER;

            do
            {
                if (currentPointer < content.Length)
                {
                    int lastPointer = readPointer(content, currentPointer * 2);
                    indexesArray.Add(lastPointer);
                    currentPointer = readPointer(content, lastPointer * 2 + 4);
                }
                else
                {
                    currentPointer = 0;
                }
            }

            while (currentPointer != 0);
        }
        void fillItemStrings(string p)
        {
            fillIndexesArray(p);
            int itemLenght = 0;
            for (int i = 0; i < indexesArray.Count; i++)
            {
                if (i != indexesArray.Count - 1)
                {
                    itemLenght = indexesArray[i + 1] - indexesArray[i];
                    itemStrings.Add(p.Substring(indexesArray[i] * 2 + 4, itemLenght * 2));
                }
                else
                {
                    itemStrings.Add(p.Substring(indexesArray[i] * 2 + 4));
                }
            }
        }
        InventoryItem StringToItem(string s)
        {
            int itemId = StringUtils.Hex4BStringToInt(s.Substring(ID_OFFSET, 8));
            int amount = StringUtils.Hex4BStringToInt(s.Substring(AMOUNT_OFFSET, 8));
            //string name = "Unknown";
            //XElement e;
            //foreach (var doc in TeraLogic.StrSheet_Item_List)
            //{
            //    e = doc.Descendants().Where(x => (string)x.Attribute("id") == itemId.ToString()).FirstOrDefault();
            //    if (e != null)
            //    {
            //        name = e.Attribute("string").Value;
            //        break;
            //    }
            //}


            return new InventoryItem(itemId, amount);
        }
        void fillItemList(string content)
        {
            fillItemStrings(content);
            foreach (var str in itemStrings)
            {
                itemsList.Add(StringToItem(str));
            }
        }
        int readPointer(string content, int start)
        {

            return StringUtils.Hex2BStringToInt(content.Substring(start, 4));
        }

        class InventoryItem
        {
            public int Id { get; set; }
            public int Amount { get; set; }
            //public string Name { get; set; }
            public InventoryItem(int _itemId, int _amount /*, string _name*/)
            {
                Id = _itemId;
                Amount = _amount;
                //Name = _name;
            }

            public override string ToString()
            {
                return " id:" + Id + " (" + Amount + ")";
            }
        }

    }

}
