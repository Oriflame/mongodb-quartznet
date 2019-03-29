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


        protected static IScheduler[] CreateSchedulers(int n, string instanceName = "QUARTZ_TEST")
        {
            var schedulers = new IScheduler[n];
            
            for (var i = 0; i < n; i++)
            {
                var properties = new NameValueCollection
                {
                    ["quartz.serializer.type"] = "binary",
                    [StdSchedulerFactory.PropertySchedulerInstanceName] = instanceName,
                    ["quartz.jobStore.clustered"] = n == 1 ? "False" : "True",
                    [StdSchedulerFactory.PropertySchedulerInstanceId] = $"{Environment.MachineName}-{Guid.NewGuid()}",
                    [StdSchedulerFactory.PropertyJobStoreType] = typeof(MongoDbJobStore).AssemblyQualifiedName,
                    [$"{StdSchedulerFactory.PropertyJobStorePrefix}.{StdSchedulerFactory.PropertyDataSourceConnectionString}"]
                        = "mongodb://localhost:32768/quartz",
                    [$"{StdSchedulerFactory.PropertyJobStorePrefix}.collectionPrefix"] = "quartztest"
                };
                var factory = new HackedStdSchedulerFactory(properties);
                var schedulerTask = factory.GetScheduler();
                schedulers[i] = schedulerTask.Result;
            }

            return schedulers;
        }
    }
}