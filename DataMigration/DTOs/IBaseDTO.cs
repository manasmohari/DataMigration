namespace DataMigration
{
    public interface IBaseDTO
    {
        string Description { get; set; }

        string DisplayName { get; set; }

        string GroupName { get; set; }

        string GroupDisplayName { get; set; }

        int LocaleId { get; set; }

        string Name { get; set; }
    }
}
