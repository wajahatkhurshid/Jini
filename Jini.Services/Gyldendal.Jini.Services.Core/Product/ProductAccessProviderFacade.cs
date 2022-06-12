using Gyldendal.Api.ShopServices.Contracts.License.Access;
using Gyldendal.Jini.Services.Data.DataAccess;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Jini.Services.Common.Dtos;
using AccessProvider = Gyldendal.Api.ShopServices.Contracts.License.Access.AccessProvider;

namespace Gyldendal.Jini.Services.Core.Product
{
    public class ProductAccessProviderFacade : IProductAccessProviderFacade
    {
        private readonly IProductAccessProviderDbController _accessProviderDbController;

        public ProductAccessProviderFacade(IProductAccessProviderDbController accessProviderDbController)
        {
            _accessProviderDbController = accessProviderDbController;
        }

        public List<ProductAccessProvider> GetProductAccessProviders(List<string> isbns)
        {
            return _accessProviderDbController.GetProductAccessProviders(isbns).GroupBy(
                p => p.Product?.Isbn,
                p => p.RefAccessProvider,
                (key, g) => new ProductAccessProvider
                {
                    Isbn = key,
                    AccessProviders = g.Select(x => new AccessProvider()
                    {
                        Code = (EnumAccessProvider)x.Code,
                        Identifier = x.Identifier,
                        DisplayName = x.DisplayName
                    }).ToList()
                }).ToList();
        }

        public ProductAccessProviders GetProductAccessProviders(string isbn)
        {
            return _accessProviderDbController.GetProductAccessProviders(isbn);
        }

        public bool SaveProductAccessProviders(ProductAccessProviders productAccessProviders)
        {
            return _accessProviderDbController.SaveProductAccessProviders(productAccessProviders);
        }
        public bool IsProductAccessProviderExists(string isbn, int accessProviderCode)
        {
            return _accessProviderDbController.IsProductAccessProviderExists(isbn, accessProviderCode);
        }

        public bool SaveProductAccessProvider(string isbn, int accessProviderCode)
        {
            return _accessProviderDbController.SaveProductAccessProvider(isbn, accessProviderCode);
        }
    }
}