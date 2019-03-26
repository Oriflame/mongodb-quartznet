namespace Quartz.Spi.MongoDbJobStore.Models.Id
{
    internal class PausedTriggerGroupId : BaseId
    {
        public const string PausedTriggerGroupType = "PausedTriggerGroup";
        
        public PausedTriggerGroupId()
        {
        }

        public PausedTriggerGroupId(string group, string instanceName)
        {
            InstanceName = instanceName;
            Group = group;
            Type = PausedTriggerGroupType;
        }

        public string Group { get; set; }
    }
}