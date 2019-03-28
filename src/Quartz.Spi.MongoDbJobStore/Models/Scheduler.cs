using System;
using MongoDB.Bson.Serialization.Attributes;
using Quartz.Spi.MongoDbJobStore.Models.Id;

namespace Quartz.Spi.MongoDbJobStore.Models
{
    internal class Scheduler
    {
        [BsonId]
        public SchedulerId Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime LastCheckIn { get; set; }
        
        public TimeSpan CheckInInterval { get; set; }
    }
}