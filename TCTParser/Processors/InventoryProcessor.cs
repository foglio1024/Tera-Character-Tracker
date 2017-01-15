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


        List<string> itemStrings = new List<string>();
        List<int> indexesArray = new List<int>();
        List<InventoryItem> itemsList = new List<InventoryItem>();

        public bool multiplePackets = false;
        public bool justLoggedIn = true;
        public string p1;
        public string p2;
        public string inv;

        public void Clear()
        {
            multiplePackets = false;
            p1 = null;
            p2 = null;
            itemsList.Clear();
            itemStrings.Clear();
            indexesArray.Clear();
        }
        public void MergeInventory()
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
        public void FastMergeInventory()
        {
            if (p2 != null)
            {
                inv = p1 + p2;
                fillItemStrings(p1);
                indexesArray.Clear();
                fillItemStrings(p2);
            }
            else
            {
                inv = p1;
                fillItemStrings(p1);
            }
        }

        public int GetTokenAmountFast(string id)
        {
            int amount = 0;
            foreach (string item in itemStrings)
            {
                if (item.Substring(16, 8) == id)
                {
                    amount = GetItemFastById(item, id);
                }
            }
            return amount;
        }

        int GetMarks(string content)
        {
            fillItemList(content);
            if (itemsList.Find(x => x.Name == "Elleon's Mark of Valor") != null)
            {
                return itemsList.Find(x => x.Name == "Elleon's Mark of Valor").Amount;
            }
            else return 0;


        }
        int GetGoldfinger(string content)
        {
            fillItemList(content);
            if (itemsList.Find(x => x.Name == "Goldfinger Token") != null)
            {
                return itemsList.Find(x => x.Name == "Goldfinger Token").Amount;
            }
            else return 0;

        }
        int GetMarksFast(string content)
        {
            return GetItemFastById(content, MARK_ID);
        }
        int GetGoldfingerFast(string content)
        {
            return GetItemFastById(content, GFIN_ID);
        }
        int GetDragonwingScaleFast(string content)
        {
            return GetItemFastById(content, SCALE_ID);
        }
        int GetItemFastById(string content, string id)
        {
            if (content.Contains(id))
            {
                return StringUtils.Hex4BStringToInt(content.Substring(content.IndexOf(id) + AMOUNT_OFFSET_FROM_ID, 8));
            }
            else return 0;

        }
        public int[] GetTokensAmounts(string content)
        {
            int marks = 0;
            int gft = 0;
            int dragonwing = 0;


            if (itemsList.Find(x => x.Name == "Goldfinger Token") != null)
            {
                gft = itemsList.Find(x => x.Name == "Goldfinger Token").Amount;
            }

            if (itemsList.Find(x => x.Name == "Elleon's Mark of Valor") != null)
            {
                marks = itemsList.Find(x => x.Name == "Elleon's Mark of Valor").Amount;
            }

            if (itemsList.Find(x => x.Name == "Dragonwing Scale") != null)
            {
                dragonwing = itemsList.Find(x => x.Name == "Dragonwing Scale").Amount;
            }

            int[] amount = { marks, gft, dragonwing };
            return amount;
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
        InventoryItem stringToItem(string s)
        {
            int itemId = StringUtils.Hex4BStringToInt(s.Substring(ID_OFFSET, 8));
            int amount = StringUtils.Hex4BStringToInt(s.Substring(AMOUNT_OFFSET, 8));
            string name = "Unknown";
            XElement e;
            foreach (var doc in TeraLogic.StrSheet_Item_List)
            {
                e = doc.Descendants().Where(x => (string)x.Attribute("id") == itemId.ToString()).FirstOrDefault();
                if (e != null)
                {
                    name = e.Attribute("string").Value;
                    break;
                }
            }


            return new InventoryItem(itemId, amount, name);
        }
        void fillItemList(string content)
        {
            fillItemStrings(content);
            foreach (var str in itemStrings)
            {
                itemsList.Add(stringToItem(str));
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
            public string Name { get; set; }
            public InventoryItem(int _itemId, int _amount, string _name)
            {
                Id = _itemId;
                Amount = _amount;
                Name = _name;
            }

            public override string ToString()
            {
                return Name + " id:" + Id + " (" + Amount + ")";
            }
        }

    }

}
