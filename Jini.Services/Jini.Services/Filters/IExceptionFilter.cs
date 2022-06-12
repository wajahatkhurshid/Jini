using System.Web.Http.Filters;

namespace Gyldendal.Jini.Services.Filters
{
    /// <summary>
    /// Interface for Exception Filter
    /// </summary>
    public interface IExceptionFilter
    {
        /// <summary>
        /// On Exception, This method will be executed
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        void OnException(HttpActionExecutedContext actionExecutedContext);
    }
}