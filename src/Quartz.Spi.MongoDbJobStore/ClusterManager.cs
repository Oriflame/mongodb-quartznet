using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Quartz.Spi.MongoDbJobStore.Util;

namespace Quartz.Spi.MongoDbJobStore
{
    internal class ClusterManager
    {
        /// <summary>
        /// From SchedulerConstants
        /// A special date time to check against when signaling scheduling change when the signaled fire date suggestion is actually irrelevant.
        /// We only want to signal the change.
        /// </summary>
        internal static DateTimeOffset? SchedulingSignalDateTime = new DateTimeOffset(1982, 6, 28, 0, 0, 0, TimeSpan.FromSeconds(0));
        
        private static readonly ILog log = LogManager.GetLogger<ClusterManager>();

        // keep constant lock requestor id for manager's lifetime
        private readonly string requestorId = Guid.NewGuid().ToString();

        private readonly MongoDbJobStore _jobStore;

        private QueuedTaskScheduler taskScheduler;
        private readonly CancellationTokenSource cancellationTokenSource;
        private Task task;

        private int numFails;

        internal ClusterManager(MongoDbJobStore jobStore)
        {
            this._jobStore = jobStore;
            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Initialize()
        {
            await Manage().ConfigureAwait(false);
            string threadName = $"QuartzScheduler_{_jobStore.InstanceName}-{_jobStore.InstanceId}_ClusterManager";

            taskScheduler = new QueuedTaskScheduler(threadCount: 1, threadPriority: ThreadPriority.AboveNormal, threadName: threadName, useForegroundThreads: !_jobStore.MakeThreadsDaemons);
            task = Task.Factory.StartNew(() => Run(cancellationTokenSource.Token), cancellationTokenSource.Token, TaskCreationOptions.HideScheduler, taskScheduler).Unwrap();
        }

        public async Task Shutdown()
        {
            cancellationTokenSource.Cancel();
            try
            {
                taskScheduler.Dispose();
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task<bool> Manage()
        {
            bool res = false;
            try
            {
                res = await _jobStore.DoCheckin(requestorId).ConfigureAwait(false);

                numFails = 0;
                log.Debug("Check-in complete.");
            }
            catch (Exception e)
            {
                if (numFails % _jobStore.RetryableActionErrorLogThreshold == 0)
                {
                    log.Error("Error managing cluster: " + e.Message, e);
                }
                numFails++;
            }
            return res;
        }

        private async Task Run(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                TimeSpan timeToSleep = _jobStore.ClusterCheckinInterval;
                TimeSpan transpiredTime = SystemTime.UtcNow() - _jobStore.LastCheckin;
                timeToSleep = timeToSleep - transpiredTime;
                if (timeToSleep <= TimeSpan.Zero)
                {
                    timeToSleep = TimeSpan.FromMilliseconds(100);
                }

                if (numFails > 0)
                {
                    timeToSleep = _jobStore.DbRetryInterval > timeToSleep ? _jobStore.DbRetryInterval : timeToSleep;
                }

                await Task.Delay(timeToSleep, token).ConfigureAwait(false);

                token.ThrowIfCancellationRequested();

                if (await Manage().ConfigureAwait(false))
                {
                    _jobStore.SignalSchedulingChangeImmediately(SchedulingSignalDateTime);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}