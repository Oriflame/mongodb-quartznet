namespace Quartz.Spi.MongoDbJobStore.Models.Id
{
    internal class SchedulerId : BaseId
    {
        public const string SchedulerType = "Scheduler";
        
        public SchedulerId() { }

        public SchedulerId(string id, string instanceName)
        {
            Id = id;
            InstanceName = instanceName;
            Type = SchedulerType;
        }

        public string Id { get; set; }

        public override string ToString()
        {
            return $"{Id}/{InstanceName}";
        }
    }
}