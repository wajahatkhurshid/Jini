using Gyldendal.Api.ShopServices.Contracts.License.Access;
using Gyldendal.Jini.Services.Common.Dtos;
using Gyldendal.Jini.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Jini.Services.Data.DataAccess
{
    public class ProductAccessProviderDbController : IProductAccessProviderDbController
    {
        private readonly ILogger _logger;

        private readonly Jini_Entities _dbContext;
        private readonly ILookUpsDbController _lookUpsDbController;

        public ProductAccessProviderDbController(ILogger logger, ILookUpsDbController lookUpsDbController, Jini_Entities dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _lookUpsDbController = lookUpsDbController;
        }

        public List<ProductAccessProvider> GetProductAccessProviders(List<string> isbns)
        {
            return _dbContext.ProductAccessProviders.Include("RefAccessProvider").Include("Product").Where(x => isbns.Contains(x.Product.Isbn)).ToList();
        }

        public ProductAccessProviders GetProductAccessProviders(string isbn)
        {
            var product = _dbContext.Products.Include("ProductAccessProviders").FirstOrDefault(x => isbn.Equals(x.Isbn));
            return new ProductAccessProviders()
            {
                Isbn = isbn,
                IsExternalLogin = product?.IsExternalLogin ?? false,
                AccessProvider = product?.ProductAccessProviders.Select(x => (EnumAccessProvider)x.AccessProviderId)
                    .ToList()
            };
        }

        public bool SaveProductAccessProviders(ProductAccessProviders productAccessProvider)
        {
            var product = _dbContext.Products.FirstOrDefault(x => x.Isbn.Equals(productAccessProvider.Isbn));
            //  var accessProvider = _dbContext.ProductAccessProviders.Where(x => x.ISBN.Equals(productInfo.Isbn));
            if (product == null)
            {
                _dbContext.Products.Add(new Product()
                {
                    Isbn = productAccessProvider.Isbn,
                    IsExternalLogin = productAccessProvider.IsExternalLogin
                });
                _dbContext.SaveChanges();
                product = _dbContext.Products.FirstOrDefault(x => x.Isbn.Equals(productAccessProvider.Isbn));
            }

            if (product != null)
            {
                product.IsExternalLogin = productAccessProvider.IsExternalLogin;

                if (product.ProductAccessProviders.Any())
                {
                    _dbContext.ProductAccessProviders.RemoveRange(product.ProductAccessProviders);
                }

                if (productAccessProvider.IsExternalLogin == false &&
                    (productAccessProvider.AccessProvider?.Any() ?? false))
                {
                    foreach (var accessProvider in productAccessProvider.AccessProvider)
                    {
                        var isAccessProvider = _lookUpsDbController.GetAccessProvider((int)accessProvider);
                        if (isAccessProvider != null)
                        {
                            _dbContext.ProductAccessProviders.Add(new ProductAccessProvider()
                            {
                                AccessProviderId = (int)accessProvider,
                                ProductId = product.Id
                            });
                        }
                    }
                }
            }

            _dbContext.SaveChanges();

            return true;
        }

        public bool IsProductAccessProviderExists(string isbn, int accessProviderCode)
        {
            return _dbContext.ProductAccessProviders.Include("Product").FirstOrDefault(x => x.Product.Isbn == isbn && x.AccessProviderId == accessProviderCode) != null;
        }

        public bool SaveProductAccessProvider(string isbn, int accessProviderCode)
        {
            bool result = false;
            var accessProvider = _lookUpsDbController.GetAccessProvider(accessProviderCode);
            if (accessProvider == null)
            {
                _logger.LogError("Invalid AccessProviderCode is given.",
                    new ArgumentException("Invalid AccessProviderCode is given."));
                return false;
            }

            // To avoid adding duplicate rows.
            var product = _dbContext.Products.FirstOrDefault(x => x.Isbn.Equals(isbn));
            if (product != null)
            {
                if (product.ProductAccessProviders.Any(x => x.AccessProviderId.Equals(accessProviderCode)))
                {
                    result = true;
                }
                else
                {
                    var productAccessProvider = new ProductAccessProvider()
                    {
                        AccessProviderId = accessProviderCode,
                        ProductId = product.Id
                    };
                    _dbContext.ProductAccessProviders.Add(productAccessProvider);
                }
            }
            else
            {
                var productNew = new Product()
                {
                    Isbn = isbn,
                    IsExternalLogin = false
                };
                var productAccessProvider = new ProductAccessProvider()
                {
                    AccessProviderId = accessProviderCode,
                    ProductId = product.Id
                };
                _dbContext.Products.Add(productNew);
                _dbContext.ProductAccessProviders.Add(productAccessProvider);
            }

            _dbContext.SaveChanges();
            return result;
        }
    }
}