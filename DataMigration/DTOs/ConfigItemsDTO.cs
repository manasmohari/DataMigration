using System;
using System.Xml.Serialization;

namespace DataMigration
{
    [Serializable]
    [XmlRoot("ConfigItems")]
    public class ConfigItemsDTO
    {
        [XmlArray("Items")]
        [XmlArrayItem("Item", typeof(ItemDTO))]
        public ItemDTO[] Items { get; set; }
    }
}
