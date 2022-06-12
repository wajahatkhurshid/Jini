using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Common.WebUtils.Models;
using Gyldendal.Jini.Services.Common.ErrorHandling;

namespace Gyldendal.Jini.Services.Common.Utils
{
    public class ErrorCodeUtil : IErrorCodeUtil

    {
        /// <summary>
        /// Get All ErrorCodes Information
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ErrorDetail> GetAllErrorCodes()
        {
            var errorsInfo = new List<ErrorDetail>();
            errorsInfo.AddRange(Extensions.GetValues<ErrorCodes>().Select(x => x.GetErrorDetail()));
            return errorsInfo;
        }

        /// <summary>
        /// Get ErrorCode Details
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public ErrorDetail GetErrorDetail(ulong errorCode)
        {
            var enumVal = Extensions.GetEnum<ErrorCodes>(errorCode);

            if (enumVal != default(ErrorCodes))
            {
                return new ErrorDetail
                {
                    Code = errorCode,
                    Description = enumVal.GetDescription(),
                    OriginatingSystem = Extensions.JiniServiceName
                };
            }
            return null;
        }
    }
}