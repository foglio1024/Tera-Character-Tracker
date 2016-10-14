using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tera
{
    [Serializable]
    public class CharDungeon : INotifyPropertyChanged
    {
        private int runs;
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string _prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(_prop));
            }
        }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Runs")]
        public int Runs { get { return runs; } set
            {
                if (runs != value)
                {
                    runs = value;
                    NotifyPropertyChanged("Runs");
                }
            }
        }

      
        public CharDungeon() { }
        public CharDungeon(string _name, int _runs)
        {
            Name = _name;
            Runs = _runs;
        }
    }
}
