using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Gyldendal.Common.WebUtils.Attributes;
using Gyldendal.Common.WebUtils.Exceptions;
using Gyldendal.Jini.Services.Common.ErrorHandling;
using Gyldendal.Jini.Services.Common.Utils;

namespace Gyldendal.Jini.Services.Filters
{
    /// <summary>
    ///
    /// </summary>
    public class NullValueFilter : ActionFilterAttribute, INullValueFilter
    {
        /// <inheritdoc />
        /// Generate error if action arguments are null
        /// ignore nullable parameters
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var optionalParameters = actionContext.ActionDescriptor.GetCustomAttributes<OptionalParameterAttribute>().FirstOrDefault();
            var parameters = actionContext.ActionDescriptor.GetParameters();
            foreach (var param in parameters)
            {
                // ignore null value validation if its optional (works for primitive types)
                if (param.IsOptional)
                    continue;

                // ignore null value validation if it is define OptionalParameter attribute (works for non-primitive types)
                if (optionalParameters != null && optionalParameters.OptionalParameters.Contains(param.ParameterName))
                    continue;

                object value = null;

                if (actionContext.ActionArguments.ContainsKey(param.ParameterName))
                    value = actionContext.ActionArguments[param.ParameterName];

                if (value == null)
                    throw new ProcessException((ulong)ErrorCodes.NullValue, ErrorCodes.NullValue.GetDescription(), Extensions.JiniServiceName);
            }
        }
    }
}