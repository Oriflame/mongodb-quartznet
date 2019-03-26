namespace Quartz.Spi.MongoDbJobStore.Models.Id
{
    internal class JobDetailId : BaseKeyId
    {
        public const string JobDetailType = "JobDetail";
        
        public JobDetailId()
        {
        }

        public JobDetailId(JobKey jobKey, string instanceName)
        {
            InstanceName = instanceName;
            Name = jobKey.Name;
            Group = jobKey.Group;
            Type = JobDetailType;
        }

        public JobKey GetJobKey()
        {
            return new JobKey(Name, Group);
        }
    }
}