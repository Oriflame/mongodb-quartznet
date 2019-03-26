namespace Quartz.Spi.MongoDbJobStore.Models.Id
{
    internal class TriggerId : BaseKeyId
    {
        public const string TriggerType = "Trigger";

        public TriggerId()
        {
            Type = TriggerType;
        }

        public TriggerId(TriggerKey triggerKey, string instanceName)
        {
            InstanceName = instanceName;
            Name = triggerKey.Name;
            Group = triggerKey.Group;
            Type = TriggerType;
        }

        public TriggerKey GetTriggerKey()
        {
            return new TriggerKey(Name, Group);
        }
    }
}