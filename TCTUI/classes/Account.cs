using System.Collections.Generic;
using System.Xml.Serialization;

namespace Tera
{
    [System.Xml.Serialization.XmlType("Account", IncludeInSchema = true)]
    public class Account
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }
        [XmlAttribute("TeraClub")]
        public bool TeraClub { get; set; }
        [XmlAttribute("Veteran")]
        public bool Veteran { get; set; }
        [XmlAttribute("TeraClubDate")]
        public long TeraClubDate { get; set; }

        public Account(string id, bool tc, bool vet, long tcl)
        {
            Id = id;
            TeraClub = tc;
            Veteran = vet;
            TeraClubDate = tcl;
        }
        public Account()
        {
            Id = "0";
            TeraClub = false;
            Veteran = false;
            TeraClubDate = 0;
        }
    }
}