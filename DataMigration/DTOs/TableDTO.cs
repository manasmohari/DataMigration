using System;
using System.Xml.Serialization;

namespace DataMigration
{
    [Serializable]
    public class TableDTO
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("Join")]
        public JoinDTO Join { get; set; }

        [XmlArray("Columns")]
        [XmlArrayItem("Column", typeof(ColumnDTO))]
        public ColumnDTO[] Columns { get; set; }
    }
}
