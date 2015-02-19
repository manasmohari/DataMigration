namespace DataMigration
{
    public class ExtensionPointDTO : IBaseDTO
    {
        public string Description
        {
            get;
            set;
        }
        
        public string DisplayName
        {
            get;
            set;
        }

        public string ExtensionPointType
        {
            get;
            set;
        }

        public string ExtensionPointTypeDisplay
        {
            get;
            set;
        }

        public string GroupName
        {
            get;
            set;
        }

        public string GroupDisplayName
        {
            get;
            set;
        }

        public int LocaleId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}
