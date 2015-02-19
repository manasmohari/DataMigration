using System;
using System.Xml.Serialization;

namespace DataMigration
{
    [Serializable]
    public class ColumnDTO
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("mappedtable")]
        public string MappedTable { get; set; }

        [XmlAttribute("mappedcolumn")]
        public string MappedColumn { get; set; }

        [XmlAttribute("table")]
        public string Table { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}
