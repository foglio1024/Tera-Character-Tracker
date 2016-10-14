using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tera
{
    [System.Xml.Serialization.XmlType("Character", IncludeInSchema = true)]

    public class Character : INotifyPropertyChanged
    {
        //Variables
        private string name;
        private string charClass;
        private uint locationId;
        private uint guildId;
        private long lastOnline;
        private long crystalbind;
        private string laurel;
        private uint position;
        private uint lvl;
        private int credits;
        private int dailies;
        private int weekly;
        private int marks_of_valor;
        private int goldfinger_tokens;
        private bool isDirty;
        private string notes;

        private List<CharDungeon> dg = new List<CharDungeon>();

        //Notify
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string _prop)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(_prop));
            }
        }

        //Properties
        [XmlAttribute("Name")]
        public string Name {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }              
            }
        }

        [XmlAttribute("Class")]
        public string CharClass
        {
            get { return charClass; }
            set
            {
                if (charClass != value)
                {
                    charClass = value;
                    NotifyPropertyChanged("CharClass");
                }
            }
        }

        [XmlAttribute("Notes")]
        public string Notes
        {
            get { return notes; }
            set
            {
                if (notes != value)
                {
                    notes = value;
                    NotifyPropertyChanged("Notes");
                }
            }
        }

        [XmlAttribute("LocationId")]
        public uint LocationId
        {
            get { return locationId; }
            set
            {
                LastOnline = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                if (locationId!= value)
                {
                    locationId = value;
                    NotifyPropertyChanged("LocationId");
                }
            }
        }

        [XmlAttribute("GuildId")]
        public uint GuildId
        {
            get { return guildId; }
            set
            {
                if (guildId != value)
                {
                    guildId = value;
                    NotifyPropertyChanged("GuildId");
                }
            }
        }

        [XmlAttribute("LastOnline")]
        public long LastOnline
        {
            get { return lastOnline; }
            set
            {
                if (lastOnline != value)
                {
                    lastOnline = value;
                    NotifyPropertyChanged("LastOnline");
                }
            }

        }

        [XmlAttribute("Crystalbind")]
        public long Crystalbind
        {
            get { return crystalbind; }
            set
            {
                if (crystalbind != value)
                {
                    crystalbind = value;
                    NotifyPropertyChanged("Crystalbind");
                }
            }

        }

        [XmlAttribute("Level")]
        public uint Level
        {
            get { return lvl; }
            set
            {
                if(lvl != value)
                {
                    lvl = value;
                    NotifyPropertyChanged("Level");
                }
            }
        }

        [XmlAttribute("Position")]
        public uint Position {
            get { return position; } set { position = value; }
        }

        [XmlArray("Dungeons")]
        [XmlArrayItem("Dungeon")]
        public List<CharDungeon> Dungeons
        {
            get { return dg; }
            set
            {
                if (dg != value)
                {
                    dg = value;
                    NotifyPropertyChanged("Dungeons");
                }
            }
        }

        [XmlAttribute("Laurel")]
        public string Laurel {
            get { return laurel; }
            set {
                if (laurel != value)
                {
                    laurel = value;
                    NotifyPropertyChanged("Laurel");
                }
            }
        }

        [XmlAttribute("Credits")]
        public int Credits {
            get { return credits; }
            set {

                if (credits != value)
                {
                    credits = value;
                    NotifyPropertyChanged("Credits");

                }
            }
        }

        [XmlAttribute("Weekly")]
        public int Weekly {
            get { return weekly; }
            set {
              
                if (weekly != value)
                {
                    weekly = value;
                    NotifyPropertyChanged("Weekly");
                };
            }
        }

        [XmlAttribute("Dailies")]
        public int Dailies
        {
            get { return dailies; }
            set
            {

                if (dailies != value)
                {
                    dailies = value;
                    NotifyPropertyChanged("Dailies");
                };
            }
        }

        [XmlAttribute("MarksOfValor")]
        public int MarksOfValor
        {
            get { return marks_of_valor; }
            set
            {

                if (marks_of_valor != value)
                {
                    marks_of_valor = value;
                    NotifyPropertyChanged("MarksOfValor");

                };
            }
        }

        [XmlAttribute("GoldfingerTokens")]
        public int GoldfingerTokens {
            get { return goldfinger_tokens; }
            set {

                if (goldfinger_tokens!= value)
                {
                    goldfinger_tokens = value;
                    NotifyPropertyChanged("GoldfingerTokens");
                }
            } }

        [XmlAttribute("IsDirty")]
        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                if (isDirty!=value)
                {
                    isDirty = value;
                    NotifyPropertyChanged("IsDirty");
                }
            }
        }
        //Methods
        public Character(uint _index, string _name, string _class, string _laurel, uint _lvl, uint _guildId, uint _locationId, long _lastOnline)
        {
            name = _name;
            charClass = _class;
            laurel = _laurel;
            lvl = _lvl;
            position = _index;

            credits = 0;
            dailies = 8;
            weekly = 0;
            marks_of_valor = 0;
            goldfinger_tokens = 0;
            locationId = _locationId;
            guildId = _guildId;
            lastOnline = _lastOnline;

            
        }
        public Character() { }
   
        public void printAll()
        {
            Console.WriteLine("             Name: {0}", name);
            Console.WriteLine("            Class: {0}", charClass);
            Console.WriteLine("          Credits: {0}", credits);
            Console.WriteLine("          Dailies: {0}", dailies);
            Console.WriteLine("   Marks of Valor: {0}", marks_of_valor);
            Console.WriteLine("Goldfinger Tokens: {0}", goldfinger_tokens);
            Console.WriteLine("          Laurel : {0}", laurel);
        }
        public void setField(int _field, int _value)
        {
            if(_field == 2)
            {
                credits = _value;
            }

            if (_field == 5)
            {
                dailies = _value;
            }
            if(_field == 4)
            {
                marks_of_valor = _value;
            }
            if(_field == 3)
            {
                goldfinger_tokens = _value;
            }

        }
        public void setField(int _field, string _value)
        {
            if (_field == 0)
            {
                name = _value;
            }
            if (_field == 1)
            {
                charClass = _value;
            }
            if (_field == 6)
            {
                laurel = _value;
            }
        }
        public void setField2(string _field, string _value)
        {
            switch (_field)
            {
                case "name":
                    name = _value;
                    break;
                case "class":
                    charClass = _value;
                    break;
                case "Marks of Valor":
                    marks_of_valor = Convert.ToInt32(_value);
                    break;
                case "Goldfinger Tokens":
                    goldfinger_tokens = Convert.ToInt32(_value);
                    break;
                case "dailies":
                    dailies = Convert.ToInt32(_value);
                    break;
                case "credits":
                    credits = Convert.ToInt32(_value);
                    break;
                case "laurel":
                    laurel = _value;
                    break;
            }
        }

    }

}
