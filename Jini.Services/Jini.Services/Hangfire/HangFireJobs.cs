using System;
using Gyldendal.Jini.Services.Core.GpmSubscription;
using Gyldendal.Jini.Services.Core.MediaMaterialType;
using Hangfire;

namespace Gyldendal.Jini.Services.Hangfire
{
    /// <summary>
    /// 
    /// </summary>
    public class HangFireJobs
    {
        /// <summary>
        /// 
        /// </summary>
        public static void ConfigureHangFireJobs()
        {
            RecurringJob.AddOrUpdate<IMediaMaterialTypeService>(x => x.UpsertMediaMaterialTypesAsync(), System.Configuration.ConfigurationManager.AppSettings["HangfireJobCron"],
                TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));
            BackgroundJob.Enqueue<IGpmMessageConsumerService>(c => c.ConsumeGpmEvents(null));

        }
    }
}