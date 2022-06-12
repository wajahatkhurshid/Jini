using System.Collections.Generic;
using Gyldendal.Api.ShopServices.Contracts.License.Access;
using Gyldendal.Jini.Services.Common.Dtos;

namespace Gyldendal.Jini.Services.Core.Product
{
    public interface IProductAccessProviderFacade
    {
        List<ProductAccessProvider> GetProductAccessProviders(List<string> isbns);
        bool IsProductAccessProviderExists(string isbn, int accessProviderCode);
        bool SaveProductAccessProvider(string isbn, int accessProviderCode);
        ProductAccessProviders GetProductAccessProviders(string isbns);
        bool SaveProductAccessProviders(ProductAccessProviders productAccessProvider);
    }
}