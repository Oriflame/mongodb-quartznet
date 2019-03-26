namespace Quartz.Spi.MongoDbJobStore.Models.Id
{
    internal class FiredTriggerId : BaseId
    {
        public const string FiredTriggerType = "FiredTrigger";
        
        public FiredTriggerId()
        {
        }

        public FiredTriggerId(string firedInstanceId, string instanceName)
        {
            InstanceName = instanceName;
            FiredInstanceId = firedInstanceId;
            Type = FiredTriggerType;
        }

        public string FiredInstanceId { get; set; }
    }
}