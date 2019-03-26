namespace Quartz.Spi.MongoDbJobStore.Models.Id
{
    internal class CalendarId : BaseId
    {
        public const string CalendarType = "Calendar";
        
        public CalendarId()
        {
        }

        public CalendarId(string calendarName, string instanceName)
        {
            InstanceName = instanceName;
            CalendarName = calendarName;
            Type = CalendarType;
        }

        public string CalendarName { get; set; }
    }
}