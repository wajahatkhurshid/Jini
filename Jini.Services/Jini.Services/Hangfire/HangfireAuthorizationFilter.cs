using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Dashboard;

namespace Gyldendal.Jini.Services.Hangfire
{
    /// <summary>
    /// Hangfire Authorization Filter
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        /// <summary>
        /// Hangfire Dashboard Context for Authorization
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}