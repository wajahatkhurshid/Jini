using Gyldendal.Common.WebUtils.Exceptions;
using Gyldendal.Common.WebUtils.Models;
using Gyldendal.Jini.Services.Common.ErrorHandling;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Data.DataAccess;
using System.Collections.Generic;

namespace Gyldendal.Jini.Services.Core.Seller
{
    public class SellerFacade : ISellerFacade
    {
        private readonly ILookUpsDbController _lookUpsDbController;

        public SellerFacade(ILookUpsDbController lookUpsDbController)
        {
            _lookUpsDbController = lookUpsDbController;
        }

        public int GetSellerWebShopId(string sellerName)
        {
            var result = _lookUpsDbController.GetSellerWebShopId(sellerName);
            if (result != null)
                return (int)result;

            //var errors = new Dictionary<string, string> { { "SellerName", $"The Seller Name {sellerName} is invalid" } };
            var errors = new List<ErrorDetailEx> { new ErrorDetailEx { Description = $"The Seller Name {sellerName} is invalid" } };
            throw new ValidationException((ulong)ErrorCodes.InvalidSellerName, ErrorCodes.InvalidSellerName.GetDescription(), Common.Constants.OrigionatingSystemName, errors);
        }

        public int GetSellerId(string sellerName)
        {
            var result = _lookUpsDbController.GetSellerId(sellerName);
            if (result != null)
                return (int)result;

            //var errors = new Dictionary<string, string> { { "SellerName", $"The Seller Name {sellerName} is invalid" } };
            var errors = new List<ErrorDetailEx> { new ErrorDetailEx { Description = $"The Seller Name {sellerName} is invalid" } };
            throw new ValidationException((ulong)ErrorCodes.InvalidSellerName, ErrorCodes.InvalidSellerName.GetDescription(), Common.Constants.OrigionatingSystemName, errors);
        }

    }
}