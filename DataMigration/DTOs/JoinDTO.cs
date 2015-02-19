using System;
using System.Xml.Serialization;

namespace DataMigration
{
    [Serializable]
    public class JoinDTO
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlArray("Columns")]
        [XmlArrayItem("Column", typeof(ColumnDTO))]
        public ColumnDTO[] Columns { get; set; }
    }
}
