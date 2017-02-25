using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TCTData
{
    public static class Data
    {
        public static List<Character> CharList { get; set; }
        public static List<Dungeon> DungList { get; set; }
        public static List<Account> AccountList { get; set; }
        public static Dictionary<uint, string> GuildDictionary { get; set; }
        public static XDocument settings;

        /// <summary>
        /// Sorts characters list based on accounts
        /// </summary>
        public static void SortChars()
        {
            var sortedList = new List<Character>();
            foreach (var account in AccountList)
            {
                var list = new List<Character>();
                foreach (var character in CharList)
                {
                    if (character.AccountId == account.Id)
                    {
                        list.Add(character);
                    }
                }
                list.Sort();
                foreach (var item in list)
                {
                    sortedList.Add(item);
                    Console.WriteLine(item.Name + " " + item.Position);
                }
            }
            CharList = sortedList;
        }

        /// <summary>
        /// Adds a new character to TCTData.Data.CharList and invokes CharacterAddedEvent
        /// </summary>
        /// <param name="character">Character to be added (if not existing)</param>
        public static void AddCharacter(Character character)
        {
            bool found = false;
            foreach (var cl in CharList)
            {
                if (cl.Name == character.Name)
                {
                    found = true;
                    cl.Laurel = character.Laurel;
                    cl.Level = character.Level;
                    cl.CharClass = character.CharClass;
                    cl.GuildId = character.GuildId;
                    cl.LocationId = character.LocationId;
                    cl.LastOnline = character.LastOnline;
                    cl.AccountId = character.AccountId;
                    cl.Position = character.Position;

                    break;
                }

            }

            if (!found)
            {
                // add char to chList
                CharList.Add(character);

                // check for TC
                int tc = 1;
                if (AccountList.Find(a => a.Id == character.AccountId).TeraClub)
                {
                    tc = 2;
                }

                // initialize dungeons
                for (int j = 0; j < DungList.Count; j++)
                {
                    if (DungList[j].ShortName == "AH" || DungList[j].ShortName == "EA" || DungList[j].ShortName == "GL" || DungList[j].ShortName == "CA")
                    {
                        CharList.Last().Dungeons.Add(new CharDungeon(DungList[j].ShortName, DungList[j].MaxBaseRuns, 0));
                    }
                    else
                    {
                        CharList.Last().Dungeons.Add(new CharDungeon(DungList[j].ShortName, DungList[j].MaxBaseRuns * tc, 0));
                    }
                }

                // create and add strip to list

                CharacterAddedEvent.Invoke(CharList.Count-1);

                //UI.MainWin.CreateStrip(CharList.Count - 1);
            }
        }

        ///<summary>
        ///Used to trigger new CharacterStrip in TCTUI.TeraMainWindow
        /// </summary>
        public static event Action<int> CharacterAddedEvent;

        public static void CheckDungeonsList()
        {
            bool found = false;

            for (int i = 0; i < TCTData.Data.CharList.Count; i++)
            {
                for (int j = 0; j < TCTData.Data.DungList.Count; j++)
                {
                    found = false;

                    for (int h = 0; h < TCTData.Data.CharList[i].Dungeons.Count; h++)
                    {
                        if (TCTData.Data.CharList[i].Dungeons[h].Name == TCTData.Data.DungList[j].ShortName)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        int tc = 1;
                        if (TCTData.Data.AccountList.Find(x => x.Id == TCTData.Data.CharList[i].AccountId).TeraClub)
                        {
                            tc = 2;
                        }
                        TCTData.Data.CharList[i].Dungeons.Insert(j, new CharDungeon(TCTData.Data.DungList[j].ShortName, TCTData.Data.DungList[j].MaxBaseRuns * tc, 0));
                    }
                }
            }

            found = false;

            for (int i = 0; i < TCTData.Data.CharList.Count; i++)
            {
                for (int h = 0; h < TCTData.Data.CharList[i].Dungeons.Count; h++)
                {
                    found = false;

                    for (int j = 0; j < TCTData.Data.DungList.Count; j++)
                    {
                        if (TCTData.Data.CharList[i].Dungeons[h].Name == TCTData.Data.DungList[j].ShortName)
                        {
                            found = true;
                            break;
                        }

                    }

                    if (!found)
                    {
                        TCTData.Data.CharList[i].Dungeons.RemoveAt(h);
                    }

                }
            }


        }

    }
}
