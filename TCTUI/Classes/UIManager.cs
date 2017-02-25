using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using TCTData;
using TCTUI.Converters;


namespace TCTUI
{
    public static class UIManager
    {
        public static List<Character> DeletedChars = new List<Character>();
        public static Dictionary<string, int> ResettedDailies = new Dictionary<string, int>();
        public static Dictionary<string, int> ResettedWeekly = new Dictionary<string, int>();

        public static List<Delegate> UndoList { get; set; }

        public static void SelectCharacter(string name)
        {

            UI.SelectedChar = TCTData.Data.CharList.Find(x => x.Name.Equals(name));
            var charIndex = (TCTData.Data.CharList.IndexOf(TCTData.Data.CharList.Find(x => x.Equals(UI.SelectedChar))));
            var w = UI.CharView;

        // set name and class
            w.charName.Text = UI.SelectedChar.Name;
            w.charClassTB.Text = UI.SelectedChar.CharClass;

        // create binding for class/laurel images
            DataBinder.BindParameterToImageSourceWithConverter(charIndex, "CharClass", w.classImg, "hd", new ClassToImage());
            DataBinder.BindParameterToImageSourceWithConverter(charIndex, "Laurel", w.laurelImg, "hd", new LaurelToImage());

        // create bindings for text blocks
            w.charName.SetBinding(      TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "Name"));
            w.guildNameTB.SetBinding(   TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "GuildId", new Guild_IdToName(), null));
            w.locationTB.SetBinding(    TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "LocationId" , new LocationIdToName(), null));
            w.lastOnlineTB.SetBinding(  TextBlock.TextProperty, DataBinder.GenericCharBinding(charIndex, "LastOnline", new UnixToDateTime(), null));
            
        // create bindings for dungeon counters
            DataBinder.CreateDgBindings(charIndex, w);
            DataBinder.CreateDgClearsBindings(charIndex, w);

        // highlight character row and scroll into view
            foreach (var ns in TCTUI.TeraMainWindow.CharacterStrips)
            {
                if (ns.Tag != null)
                {
                    if (ns.Tag.Equals(name))
                    {
                        ns.rowSelect(true);
                        UI.CharListContainer.chContainer.ScrollIntoView(ns);
                    }
                    else
                    {
                        ns.rowSelect(false);
                    }
                }
            }

        // set guild image
            if(File.Exists(Environment.CurrentDirectory + "\\content/data/guild_images/" + TCTData.Data.CharList[charIndex].GuildId.ToString() + ".bmp"))
            {
                try
                {
                    System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\content/data/guild_images/" + TCTData.Data.CharList[charIndex].GuildId.ToString() + ".bmp");
                    UI.MainWin.SetGuildImage(bmp);
                }
                catch
                {
                    UI.UpdateLog("Error while setting guild image. Using default image.");
                }
            }
            else
            {
                UI.UpdateLog("Guild image not found. Using default image.");
                System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\content/data/guild_images/" + "0" + ".bmp");
                UI.MainWin.SetGuildImage(bmp);
            }

        }
    }
}
