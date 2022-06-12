using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Gyldendal.Common.WebUtils.Exceptions;
using Gyldendal.Common.WebUtils.Models;
using Gyldendal.Jini.Services.Common;
using Gyldendal.Jini.Services.Utils;

namespace Gyldendal.Jini.Services.Filters
{
    /// <summary>
    /// Catch Exception and handle it
    /// </summary>
    public class ExceptionFilter : ExceptionFilterAttribute, IExceptionFilter
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor of work Controller
        /// </summary>
        /// <param name="logger"></param>
        public ExceptionFilter(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            _logger.LogError(exception.Message, exception);
            if (exception is NotFoundException)
            {
                var notFoundException = (NotFoundException)exception;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(notFoundException.StatusCode, notFoundException.ValidationErrors);
            }
            else if (exception is ProcessException)
            {
                var processException = (ProcessException)exception;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorDetail
                    {
                        Code = processException.ErrorCode,
                        Description = processException.Description,
                        OriginatingSystem = processException.OriginatingSystem
                    });
            }
            else if (exception is ValidationException)
            {
                var modelValidatoinException = (ValidationException)exception;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ValidationErrorDetail
                    {
                        Code = modelValidatoinException.ErrorCode,
                        Description = modelValidatoinException.Description,
                        OriginatingSystem = modelValidatoinException.OriginatingSystem,
                        ObjectValidationDetail = modelValidatoinException.ObjectValidations
                    });
            }
            else
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError, exception.Message);
            }
        }
    }
}