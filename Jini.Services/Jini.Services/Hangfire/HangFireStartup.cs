using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Hangfire.Storage.Monitoring;

namespace Gyldendal.Jini.Services.Hangfire
{
    /// <summary>
    /// 
    /// </summary>
    public class HangFireStartup
    {
        /// <summary>
        /// 
        /// </summary>
        public static void PreProcessing()
        {
            DeleteHangFirePendingInstancesForJob();
        }
        /// <summary>
        /// Delete all consumer jobs which are in pending/processing state
        /// </summary>
        private static void DeleteHangFirePendingInstancesForJob()
        {
            RemoveAllJobs();
        }

        private static void RemoveAllJobs()
        {
            var monitor = JobStorage.Current.GetMonitoringApi();

            var jobsScheduled = monitor.ScheduledJobs(0, int.MaxValue);
            foreach (var j in jobsScheduled)
            {
                BackgroundJob.Delete(j.Key);
            }
        }
    }
}