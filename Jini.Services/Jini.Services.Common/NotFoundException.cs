using System;
using System.Collections.Generic;
using System.Net;
using Gyldendal.Common.WebUtils.Models;

namespace Gyldendal.Jini.Services.Common
{
    public class NotFoundException : Exception
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.NotFound;

        public List<ErrorDetail> ValidationErrors
        {
            get;
            private set;
        }

        public NotFoundException(List<ErrorDetail> validationErrors)
        {
            ValidationErrors = validationErrors;
        }
    }
}