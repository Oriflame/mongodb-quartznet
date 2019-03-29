using System.Collections.Generic;
using System.Linq;
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
            var groups = await Collection.Find(x => x.Id.InstanceName == InstanceName && x.Id.Type == Type)
                .Project(x => x.Id).ToListAsync(); // For unknown reason .Project(x => x.Id.Group) returns null :(
            return groups.Select(x => x.Group).Distinct().ToList();
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