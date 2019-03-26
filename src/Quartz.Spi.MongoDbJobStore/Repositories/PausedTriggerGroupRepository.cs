using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Quartz.Impl.Matchers;
using Quartz.Spi.MongoDbJobStore.Extensions;
using Quartz.Spi.MongoDbJobStore.Models;
using Quartz.Spi.MongoDbJobStore.Models.Id;

namespace Quartz.Spi.MongoDbJobStore.Repositories
{
    internal class PausedTriggerGroupRepository : BaseRepository<PausedTriggerGroup>
    {
        public PausedTriggerGroupRepository(IMongoDatabase database, string instanceName, string collectionName)
            : base(database, instanceName, PausedTriggerGroupId.PausedTriggerGroupType, collectionName)
        {
        }

        public async Task<List<string>> GetPausedTriggerGroups()
        {
            return await Collection.Find(group => group.Id.InstanceName == InstanceName && group.Id.Type == Type)
                .Project(group => group.Id.Group)
                .ToListAsync();
        }

        public async Task<bool> IsTriggerGroupPaused(string group)
        {
            return await Collection.Find(g => g.Id == new PausedTriggerGroupId(group, InstanceName)).AnyAsync();
        }

        public async Task AddPausedTriggerGroup(string group)
        {
            await Collection.InsertOneAsync(new PausedTriggerGroup
            {
                Id = new PausedTriggerGroupId(group, InstanceName)
            });
        }

        public async Task DeletePausedTriggerGroup(GroupMatcher<TriggerKey> matcher)
        {
            var regex = matcher.ToBsonRegularExpression().ToRegex();
            await Collection.DeleteManyAsync(group => group.Id.InstanceName == InstanceName && regex.IsMatch(group.Id.Group) && group.Id.Type == Type);
        }

        public async Task DeletePausedTriggerGroup(string groupName)
        {
            await Collection.DeleteOneAsync(group => group.Id == new PausedTriggerGroupId(groupName, InstanceName));
        }
    }
}