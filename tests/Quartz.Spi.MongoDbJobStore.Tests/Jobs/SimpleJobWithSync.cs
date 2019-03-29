using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.Spi.MongoDbJobStore.Tests.Jobs
{
    public class SimpleJobWithSync : IJob
    {
        public static readonly Dictionary<string,object> Context = new Dictionary<string, object>();
        
        
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var jobExecTimestamps = (List<DateTime>) Context[BaseStoreTests.DateStamps];
                var barrier = (Barrier) Context[BaseStoreTests.Barrier];

                jobExecTimestamps.Add(DateTime.UtcNow);

                barrier.SignalAndWait(BaseStoreTests.TestTimeout);
            }
            catch (Exception e)
            {
                Console.Write(e);
                throw e;
            }

            return Task.FromResult(0);
        }
    }
}