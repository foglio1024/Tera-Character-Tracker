using System.Collections.Generic;
using System.Xml.Serialization;

namespace Tera
{
    [System.Xml.Serialization.XmlType("Account", IncludeInSchema = true)]
    public class Account
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }
        [XmlAttribute("TeraClub")]
        public long TeraClub { get; set; }
        [XmlAttribute("Veteran")]
        public bool Veteran { get; set; }
    }
}