using System;
using System.Xml.Serialization;

namespace DataMigration
{
    [Serializable]
    public class ItemDTO
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlArray("Tables")]
        [XmlArrayItem("Table", typeof(TableDTO))]
        public TableDTO[] Tables { get; set; }

        [XmlArray("Filters")]
        [XmlArrayItem("Column", typeof(ColumnDTO))]
        public ColumnDTO[] Filters { get; set; }
    }
}
