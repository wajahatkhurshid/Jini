using System.Collections.Generic;
using Gyldendal.Jini.Services.Common.Dtos;

namespace Gyldendal.Jini.Services.Data.DataAccess
{
    public interface IProductAccessProviderDbController
    {
        List<ProductAccessProvider> GetProductAccessProviders(List<string> isbns);

        bool IsProductAccessProviderExists(string isbn, int accessProviderCode);

        bool SaveProductAccessProvider(string isbn, int accessProviderCode);
        ProductAccessProviders GetProductAccessProviders(string isbn);
        bool SaveProductAccessProviders(ProductAccessProviders productInfo);
    }
}