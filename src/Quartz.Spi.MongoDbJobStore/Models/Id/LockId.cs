using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Quartz.Spi.MongoDbJobStore.Models.Id
{
    internal class LockId : BaseId
    {
        public const string LokType = "Lock";
        
        public LockId() { }

        public LockId(LockType lockType, string instanceName)
        {
            LockType = lockType;
            InstanceName = instanceName;
            Type = LokType;
        }

        [BsonRepresentation(BsonType.String)]
        public LockType LockType { get; set; }

        public override string ToString()
        {
            return $"{LockType}/{InstanceName}";
        }
    }
}