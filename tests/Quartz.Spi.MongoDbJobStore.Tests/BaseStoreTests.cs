using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Quartz.Impl;

namespace Quartz.Spi.MongoDbJobStore.Tests
{
    public abstract class BaseStoreTests
    {
        public const string Barrier = "BARRIER";
        public const string DateStamps = "DATE_STAMPS";
        public static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(125);

        protected async Task<IScheduler> CreateScheduler(string instanceName = "QUARTZ_TEST")
        {
            var properties = new NameValueCollection
            {
                ["quartz.serializer.type"] = "binary",
                [StdSchedulerFactory.PropertySchedulerInstanceName] = instanceName,
                ["Clustered"] = "true",
                ["ClusterCheckinInterval"] = "3000",
                ["ClusterCheckinMisfireThreshold"] = "3000",
                [StdSchedulerFactory.PropertySchedulerInstanceId] = $"{Environment.MachineName}-{Guid.NewGuid()}",
                [StdSchedulerFactory.PropertyJobStoreType] = typeof(MongoDbJobStore).AssemblyQualifiedName,
                [$"{StdSchedulerFactory.PropertyJobStorePrefix}.{StdSchedulerFactory.PropertyDataSourceConnectionString}"]
                    = "mongodb://localhost:32768/quartz",
                [$"{StdSchedulerFactory.PropertyJobStorePrefix}.collectionPrefix"] = "quartztest"
            };

            var scheduler = new StdSchedulerFactory(properties);
            return await scheduler.GetScheduler();
        }
        
        protected IScheduler[] CreateSchedulers(int n, string instanceName = "QUARTZ_TEST")
        {
            var schedulers = new IScheduler[n];
            
            for (var i = 0; i < n; i++)
            {
                schedulers[i] = CreateScheduler(instanceName).Result;
            }

            return schedulers;
        }
    }
}