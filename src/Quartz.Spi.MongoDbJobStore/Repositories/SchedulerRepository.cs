using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Quartz.Spi.MongoDbJobStore.Models;
using Quartz.Spi.MongoDbJobStore.Models.Id;

namespace Quartz.Spi.MongoDbJobStore.Repositories
{
    internal class SchedulerRepository : BaseRepository<Scheduler>
    {
        public SchedulerRepository(IMongoDatabase database, string instanceName, string collectionName)
            : base(database, instanceName, SchedulerId.SchedulerType, collectionName)
        {
        }

        public async Task AddScheduler(Scheduler scheduler)
        {
            await Collection.ReplaceOneAsync(sch => sch.Id == scheduler.Id,
                scheduler, new UpdateOptions
                {
                    IsUpsert = true
                });
        }

        public async Task DeleteScheduler(string id)
        {
            await Collection.DeleteOneAsync(sch => sch.Id == new SchedulerId(id, InstanceName));
        }
        
        public async Task<long> UpdateLastCheckin(string id, DateTime lastCheckin)
        {
            var updateResult = await Collection.UpdateOneAsync(sch => sch.Id == new SchedulerId(id, InstanceName),
                UpdateBuilder.Set(sch => sch.LastCheckIn, lastCheckin));

            return updateResult.MatchedCount;
        }

        public Task<List<Scheduler>> SelectSchedulerStateRecords(string instanceId, CancellationToken cancellationToken)
        {
            return instanceId == null ? 
                Collection.Find(s => s.Id.InstanceName == InstanceName && s.Id.Type == Type).ToListAsync(cancellationToken: cancellationToken) : 
                Collection.Find(s => s.Id == new SchedulerId(instanceId, InstanceName)).ToListAsync(cancellationToken: cancellationToken);
        }
    }
}