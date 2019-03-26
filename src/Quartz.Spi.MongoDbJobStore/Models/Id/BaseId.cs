namespace Quartz.Spi.MongoDbJobStore.Models.Id
{
    internal abstract class BaseId
    {
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }        
        
        public string InstanceName { get; set; }
    }
}