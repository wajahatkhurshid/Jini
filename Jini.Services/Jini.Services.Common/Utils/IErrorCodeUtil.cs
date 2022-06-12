using System.Collections.Generic;
using Gyldendal.Common.WebUtils.Models;

namespace Gyldendal.Jini.Services.Common.Utils
{
    public interface IErrorCodeUtil
    {
        IEnumerable<ErrorDetail> GetAllErrorCodes();

        ErrorDetail GetErrorDetail(ulong errorCode);
    }
}