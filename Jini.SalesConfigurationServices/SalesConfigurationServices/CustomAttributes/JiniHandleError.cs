using System.Web.Http.Filters;
using LoggingManager;

namespace Gyldendal.Jini.SalesConfigurationServices.CustomAttributes
{
    public class JiniHandleError : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (Logger != null)
            {
                Logger.LogError(context.Exception.Message, context.Exception);
            }
            
        }

        private Logger Logger => Logger.Instance;
    }
}