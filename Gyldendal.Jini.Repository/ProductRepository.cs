using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gyldendal.Jini.Repository.Contracts;
using Gyldendal.Jini.Services.Common;
using Gyldendal.Jini.Services.Common.ConfigurationManager;
using Gyldendal.Jini.Services.Common.Dtos;
using Gyldendal.Jini.Services.Common.RequestResponse;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Services.Data;
using Kendo.Mvc;
using LinqKit;

namespace Gyldendal.Jini.Repository
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly string _connectionString;
        public ProductRepository(Jini_Entities context, IJiniConfigurationManager jiniConfigurationManager) : base(context)
        {
            _connectionString = jiniConfigurationManager.ConnectionString;
        }
        public async Task<DigitalProductsResponse> GetProductsAsync(List<FilterDescriptor> request, int page, int pageSize, string sortOrder, string sortField)
        {
            return await GetDigitalProducts(request, page, pageSize, sortOrder, sortField);
        }

        public async Task<DigitalProduct> GetDigitalProductByIsbnAsync(string isbn)
        {
            return await Context.Products.Where(x => x.Isbn == isbn).Select(x => new DigitalProduct()
            {
                Isbn = x.Isbn,
                Title = x.Title,
                MaterialTypeCode = x.GpmMediaTypeId.ToString(),
                MaterialTypeName = x.MaterialType.Name,
                MediaTypeCode = x.GpmMaterialTypeId.ToString(),
                MediaTypeName = x.MediaType.Name,
                PublishDate = x.ReleaseDate,
                DepartmentCode = x.DepartmentCode,
                DepartmentName = x.DepartmentName,
                SectionCode = x.SectionCode,
                SectionName = x.SectionName

            }).FirstOrDefaultAsync();
        }

        public async Task<string> GetGradeLevelsByIsbnAsync(string isbn)
        {
            return await Context.Products.Where(x => x.Isbn == isbn).Select(x => x.GradeLevels).FirstOrDefaultAsync();
        }

        public async Task<bool> UpsertProductAsync(Product product)
        {
            using (var context = new Jini_Entities(_connectionString))
            {
                var existingEntity = await context.Products.Where(x => x.Isbn == product.Isbn).FirstOrDefaultAsync();
                if (existingEntity != null)
                    product.Id = existingEntity.Id;

                context.Products.AddOrUpdate(product);
                return await context.SaveChangesAsync() > 0;
            }
        }
        public async Task<DeflatedSalesConfigurationV2Response> GetSalesConfigurations(DeflatedSalesConfigurationRequest request)
        {
            if (!request.IsAllMediaTypesChecked && request.MediaMaterialTypes.Length == 0)
            {
                return new DeflatedSalesConfigurationV2Response();
            }
            var materialTypeIds = new List<string>();
            var departmentsIds = new List<string>();
            var salesConfigurations = GetSalesConfigurationsQueryable(request);
            if (request.DepartmentsSections?.Length > 0)
            {
                departmentsIds = request.DepartmentsSections.Select(x => x.SectionCode).ToList();
                salesConfigurations = salesConfigurations.Where(
                    x => x.DepartmentCode == null || (x.DepartmentCode != null && departmentsIds.Contains(x.DepartmentCode)));
            }
            if (request.MediaMaterialTypes?.Length > 0)
            {
                materialTypeIds = request.MediaMaterialTypes.Select(x => x.MaterialTypeCode).ToList();
                salesConfigurations = salesConfigurations.Where(
                    x => x.GpmMediaTypeId != null && materialTypeIds.Contains(x.GpmMaterialTypeId));
            }
            if (request.SalesConfigurationStates != null && request.SalesConfigurationStates.Any())
            {
                if (request.SalesConfigurationStates.Contains("1") && !request.SalesConfigurationStates.Contains("2"))
                {
                    salesConfigurations = salesConfigurations
                        .Where(x => x.IsExternalLogin == 1 || x.ProductAccessProvider == 1);
                }
                else if (request.SalesConfigurationStates.Contains("2") && !request.SalesConfigurationStates.Contains("1"))
                {
                    salesConfigurations = salesConfigurations
                        .Where(x => x.IsExternalLogin == 0 && x.ProductAccessProvider == 0);
                }
            }
            var totalCount = salesConfigurations.Count();
            var totalProducts = salesConfigurations.GroupBy(x => x.Isbn).Count();
            salesConfigurations = SortData(salesConfigurations, request.SortField, request.SortOrder);
            salesConfigurations = salesConfigurations
                .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);
            var objectToReturn = new DeflatedSalesConfigurationV2Response
            {
                Data = await salesConfigurations.ToListAsync(),
                Total = totalCount,
                ProductCount = totalProducts
            };
            return objectToReturn;
        }
        private IQueryable<DeflatedSalesConfigurationV2Dto> GetSalesConfigurationsQueryable(DeflatedSalesConfigurationRequest request)
        {
            var tzi = TimeZoneInfo.Local;//FindSystemTimeZoneById("Central European Standard Time");
            var releaseStartDate = !string.IsNullOrEmpty(request.ReleaseStartDate)
                ? TimeZoneInfo.ConvertTime(Convert.ToDateTime(request.ReleaseStartDate), tzi)
                : (DateTime?)null;

            var releaseEndDate = !string.IsNullOrEmpty(request.ReleaseEndDate)
                ? TimeZoneInfo.ConvertTime(Convert.ToDateTime(request.ReleaseEndDate), tzi)
                : (DateTime?)null;
            var salesConfiguration = (from prodct in Context.Products
                                      join deflatedSalesConfigurationView in Context.DeflatedSalesConfigurationViews on prodct.Isbn equals deflatedSalesConfigurationView.Isbn
                                      where (request.Isbn == "" || prodct.Isbn.Contains(request.Isbn))
                                            &&
                                            (request.Title == "" || (!string.IsNullOrEmpty(prodct.Title) &&
                                                                     prodct.Title.Trim().ToLower()
                                                                         .Contains(request.Title.Trim()
                                                                             .ToLower()))) &&
                                            (!releaseStartDate.HasValue ||
                                             (prodct.ReleaseDate.HasValue &&
                                              prodct.ReleaseDate.Value >= releaseStartDate.Value &&
                                              prodct.ReleaseDate.Value <= releaseEndDate.Value)

                                            )
                                      select new DeflatedSalesConfigurationV2Dto()
                                      {
                                          Id = deflatedSalesConfigurationView.Id,
                                          Isbn = deflatedSalesConfigurationView.Isbn,
                                          LastModified = deflatedSalesConfigurationView.CreatedDate,
                                          RefAccessFormCode = deflatedSalesConfigurationView.RefAccessFormCode,
                                          RefAccessFormDisplayName = deflatedSalesConfigurationView.RefAccessFormDisplayName,
                                          RefPeriodTypeCode = deflatedSalesConfigurationView.RefPeriodTypeCode,
                                          RefPeriodTypeDisplayName = deflatedSalesConfigurationView.RefPeriodTypeDisplayName,
                                          RefPriceModelCode = deflatedSalesConfigurationView.RefPriceModelCode,
                                          RefPriceModelDisplayName = deflatedSalesConfigurationView.RefPriceModelDisplayName,
                                          RefSalesCode = deflatedSalesConfigurationView.RefSalesCode,
                                          RefSalesDisplayName = deflatedSalesConfigurationView.RefSalesDisplayName,
                                          RefSalesConfigTypeCode = deflatedSalesConfigurationView.RefSalesConfigTypeCode,
                                          RevisionNumber = deflatedSalesConfigurationView.RevisionNumber,
                                          SalesChannel = deflatedSalesConfigurationView.SalesChannel,
                                          SalesFormId = deflatedSalesConfigurationView.SalesFormId,
                                          SellerId = deflatedSalesConfigurationView.SellerId,
                                          State = deflatedSalesConfigurationView.State,
                                          TrialLicenseId = deflatedSalesConfigurationView.TrialLicenseId,
                                          UnitPrice = deflatedSalesConfigurationView.UnitPrice,
                                          UnitPriceVat = deflatedSalesConfigurationView.UnitPriceVat,
                                          UnitValue = deflatedSalesConfigurationView.UnitValue,
                                          CreatedBy = deflatedSalesConfigurationView.CreatedBy,
                                          Title = prodct.Title,
                                          ReleaseDate = prodct.ReleaseDate,
                                          GpmMediaTypeId = prodct.GpmMediaTypeId.Value,
                                          GpmMaterialTypeId = prodct.GpmMaterialTypeId.ToString(),
                                          IsExternalLogin = deflatedSalesConfigurationView.IsExternalLogin,
                                          ProductAccessProvider = deflatedSalesConfigurationView.ProductAccessProvider,
                                          DepartmentCode = deflatedSalesConfigurationView.DepartmentCode
                                      });
            return salesConfiguration;
        }

        private static Expression<Func<T, bool>> MakeFalse<T>(IQueryable<T> ignored)
        {
            return PredicateBuilder.False<T>();
        }
        private IQueryable<Products> GetProductsQueryable()
        {
            var allProducts =
                (from prodct in Context.Products
                 join pap in Context.ProductAccessProviders on prodct.Id equals pap.ProductId into prodctJoined
                 from papD in prodctJoined.DefaultIfEmpty()
                 join cc in Context.SalesConfigurations on prodct.Isbn equals cc.Isbn into joined
                 from c in joined.DefaultIfEmpty()
                 where c.RefSalesConfigTypeCode == null || c.RefSalesConfigTypeCode == 1001
                 select new Products()
                 {
                     Isbn = prodct.Isbn ?? (c != null ? c.Isbn : ""),
                     CreatedBy = (c != null ? c.CreatedBy : null),
                     RefSalesConfigTypeCode = (c != null ? c.RefSalesConfigTypeCode : null),
                     CreatedDate = c.CreatedDate,
                     RevisionNumber = (c != null ? c.RevisionNumber : 0),
                     SalesChannel = (c != null ? c.SalesChannel : null),
                     SalesFormId = (c != null ? c.SalesFormId : null),
                     SellerId = (c != null ? c.SellerId : 0),
                     State = (c != null ? c.State : (int?)null),
                     TrialLicenseId = (c != null ? c.TrialLicenseId : null),
                     ProductAccessId = (papD != null ? papD.AccessProviderId : 0),
                     HasProductAccess = ((papD != null ? papD.AccessProviderId : 0) > 0 || prodct.IsExternalLogin),
                     Title = prodct.Title,
                     Subtitle = prodct.UnderTitle,
                     MediaTypeId = prodct.GpmMediaTypeId.Value,
                     MediaTypeName = prodct.MediaType.Name,
                     MaterialTypeId = prodct.GpmMaterialTypeId.Value,
                     MaterialTypeName = prodct.MaterialType.Name,
                     PublishDate = prodct.ReleaseDate,
                     DepartmentCode = prodct.DepartmentCode,
                     DepartmentName = prodct.DepartmentName,
                     SectionCode = prodct.SectionCode,
                     SectionName = prodct.SectionName

                 }).GroupBy(x => x.Isbn).Select(g => g.FirstOrDefault());
            return allProducts;
        }

        private async Task<DigitalProductsResponse> GetDigitalProducts(List<FilterDescriptor> filters, int page, int pageSize, string sortOrder, string sortField)
        {
            var digitalProductResponse = new DigitalProductsResponse();
            var extractMaterialTypeIds = filters.Where(y => y.Member == "MaterialTypeCode").GroupBy(x => x.Member,
                (k, c) => new
                {
                    id = c.Select(z => z.Value.ToString()).ToList()
                }
            ).ToList();
            var materialTypeIds = extractMaterialTypeIds.SelectMany(x => x.id).ToList();
            var departments= filters.Where(y => y.Member == "SectionCode").GroupBy(x => x.Member,
                (k, c) => new
                {
                    id = c.Select(z => z.Value.ToString()).ToList()
                }
            ).ToList();
            
            var departmentCodes = departments.SelectMany(r => r.id).ToList();
            var releaseDateFilter = filters.Any(x => x.Member.Contains(ProductFilterOptions.PublishDate.ToString()));
            var lastUpdatedDateFilter = filters.Any(x => x.Member.Contains(ProductFilterOptions.LatestChangeDateInJini.ToString()));
            var releaseDateStart = DateTime.Now;
            var releaseDateEnd = DateTime.Now;
            var lastUpdatedStartDate = DateTime.Now;
            var lastUpdatedEndDate = DateTime.Now;
            if (lastUpdatedDateFilter)
            {
                lastUpdatedStartDate = Convert.ToDateTime(filters.Where(x => x.Operator.ToString() == "IsGreaterThanOrEqualTo" && x.Member.ToString() == ProductFilterOptions.LatestChangeDateInJini.ToString())
                    .Select(y => y.Value).FirstOrDefault());
                lastUpdatedEndDate = Convert.ToDateTime(filters.Where(x => x.Operator.ToString() == "IsLessThanOrEqualTo" && x.Member.ToString() == ProductFilterOptions.LatestChangeDateInJini.ToString())
                    .Select(y => y.Value).FirstOrDefault());
            }
            if (releaseDateFilter)
            {
                releaseDateStart = Convert.ToDateTime(filters.Where(x => x.Operator.ToString() == "IsGreaterThanOrEqualTo" && x.Member.ToString() == ProductFilterOptions.PublishDate.ToString())
                    .Select(y => y.Value).FirstOrDefault());
                releaseDateEnd = Convert.ToDateTime(filters.Where(x => x.Operator.ToString() == "IsLessThanOrEqualTo" && x.Member.ToString() == ProductFilterOptions.PublishDate.ToString())
                    .Select(y => y.Value).FirstOrDefault());
            }
            var allProducts = GetProductsQueryable();
            var predicate = MakeFalse(allProducts);
            if (filters.Any())
            {
                var configurationTextFilter = filters.Where(x => x.Member == ProductFilterOptions.ConfigurationText.ToString()).Select(x => x).ToList();
                filters = filters.Where(x => x.Member != ProductFilterOptions.ConfigurationText.ToString()).GroupBy(x => x.Member).Select(x => x.First()).ToList();
                foreach (var filter in filters)
                {
                    if (filter.Member == ProductFilterOptions.MaterialTypeCode.ToString())
                    {
                        allProducts = allProducts.Where(
                            x => x.MaterialTypeId != null && !materialTypeIds.Contains(x.MaterialTypeId.ToString()));
                    }
                    else if (filter.Member == ProductFilterOptions.SectionCode.ToString())
                    {
                        allProducts = allProducts.Where(
                            x => x.DepartmentCode == null || (x.DepartmentCode != null && !departmentCodes.Contains(x.DepartmentCode.ToString())));
                    }
                    else if (filter.Member == ProductFilterOptions.Title.ToString())
                    {
                        var value = filter.Value.ToString();
                        allProducts = allProducts.Where(
                            x => x.Title.Contains(value));
                    }
                    else if (filter.Member == ProductFilterOptions.Isbn.ToString())
                    {
                        var value = filter.Value.ToString();
                        allProducts = allProducts.Where(
                            x => x.Isbn.Contains(value));
                    }
                    else if (filter.Member == ProductFilterOptions.PublishDate.ToString() && filter.Operator.ToString() == "IsGreaterThanOrEqualTo")
                    {
                        allProducts = allProducts.Where(
                               x => x.PublishDate >= releaseDateStart && x.PublishDate <= releaseDateEnd);
                    }
                    if (filter.Member == ProductFilterOptions.LatestChangeDateInJini.ToString() && filter.Operator.ToString() == "IsGreaterThanOrEqualTo")
                    {
                        allProducts = allProducts.Where(
                            x => x.CreatedDate >= lastUpdatedStartDate && x.CreatedDate <= lastUpdatedEndDate);
                    }
                }

                foreach (var filter in configurationTextFilter)
                {

                    if (filter.Member == ProductFilterOptions.ConfigurationText.ToString() && filter.Value.ToString() == ProductStatus.Configured.GetDescription())
                    {
                        predicate = predicate.Or(p =>
                            p.State == (int)Enums.EnumState.Approved && p.HasProductAccess == true);
                    }

                    else if (filter.Member == ProductFilterOptions.ConfigurationText.ToString() && filter.Value.ToString() == ProductStatus.PendingLoginSetup.GetDescription())
                    {
                        predicate = predicate.Or(p =>
                            p.State == (int)Enums.EnumState.Approved && p.HasProductAccess == false);
                    }
                    else if (filter.Member == ProductFilterOptions.ConfigurationText.ToString() && filter.Value.ToString() == ProductStatus.AwaitingSalesSetup.GetDescription())
                    {
                        predicate = predicate.Or(p =>
                            (p.State == null || p.State == (int)Enums.EnumState.Draft) && p.HasProductAccess == true);
                    }
                    else if (filter.Member == ProductFilterOptions.ConfigurationText.ToString() && filter.Value.ToString() == ProductStatus.AwaitingSetup.GetDescription())
                    {
                        predicate = predicate.Or(p =>
                            p.State == null && !p.HasProductAccess);
                    }

                }
            }
            if (predicate.Body.NodeType != (ExpressionType)9)
            {
                allProducts = allProducts.Where(predicate);
            }
            digitalProductResponse.Total = allProducts.Count();
            allProducts = SortData(allProducts, sortField, sortOrder);
            allProducts = allProducts.Skip((page - 1) * pageSize).Take(pageSize);
            var listOfProducts = await GetDigiltalProductsList(allProducts);
            digitalProductResponse.Data = listOfProducts;
            return digitalProductResponse;
        }

        private async Task<List<DigitalProduct>> GetDigiltalProductsList(IQueryable<Products> products)
        {
            var listOfProducts = await products.Select(product => new DigitalProduct()
            {
                Title = product.Title,
                SubTitle = product.Subtitle,
                Isbn = product.Isbn,
                MaterialTypeCode = product.MaterialTypeId.ToString(),
                MaterialTypeName = product.MaterialTypeName,
                MediaTypeCode = product.MediaTypeId.ToString(),
                MediaTypeName = product.MediaTypeName,
                PublishDate = product.PublishDate,
                LatestChangeDateInJini = product.CreatedDate,
                HasDraft = product.State == (int)Enums.EnumState.Draft ? true : false,
                HasTrailAccess = (product.TrialLicenseId != null && product.State == (int)Enums.EnumState.Approved) ? true : false,
                HasConfiguration = product.State == (int)Enums.EnumState.Approved ? true : false,
                HasProductAccess = product.HasProductAccess,
                State = product.State,
                DepartmentCode = product.DepartmentCode,
                DepartmentName = product.DepartmentName,
                SectionCode = product.SectionCode,
                SectionName = product.SectionName
            }).ToListAsync();
            foreach (var item in listOfProducts)
            {
                item.ConfigurationText = GetConfigurationText(item.State, item.HasProductAccess);
            }

            return listOfProducts;
        }

        private string GetConfigurationText(int? state, bool hasProductAccess)
        {
            var result = ProductStatus.AwaitingSetup.GetDescription();
            if (state == null && !hasProductAccess)
            {
                result = ProductStatus.AwaitingSetup.GetDescription();
            }
            else if (state == (int)Enums.EnumState.Approved && hasProductAccess)
            {
                result = ProductStatus.Configured.GetDescription();
            }
            else if (state == (int)Enums.EnumState.Approved && !hasProductAccess)
            {
                result = ProductStatus.PendingLoginSetup.GetDescription();
            }
            else if ((state == null || state == (int)Enums.EnumState.Draft) && hasProductAccess)
            {
                result = ProductStatus.AwaitingSalesSetup.GetDescription();
            }

            return result;
        }
        private IQueryable<Products> SortData(IQueryable<Products> data, string sortName, string order)
        {
            if (!string.IsNullOrEmpty(sortName) && !string.IsNullOrEmpty(order))
            {
                switch (order)
                {
                    case "Ascending":
                        switch (sortName)
                        {
                            case "Isbn":
                                data = data.OrderBy(x => x.Isbn);
                                break;
                            case "Title":
                                data = data.OrderBy(x => x.Title);
                                break;
                            case "MaterialTypeCode":
                                data = data.OrderBy(x => x.MaterialTypeId);
                                break;
                            case "MediaMaterialName":
                                data = data.OrderBy(x => x.MaterialTypeId);
                                break;
                            case "PublishDate":
                                data = data.OrderBy(x => x.PublishDate);
                                break;
                            case "HasConfiguration":
                                data = data.OrderBy(x => x.PublishDate);
                                break;
                            case "LatestChangeDateInJini":
                                data = data.OrderBy(x => x.CreatedDate);
                                break;
                            case "ConfigurationText":
                                data = data.OrderBy(x => x.State);
                                break;

                        }

                        break;
                    case "Descending":
                        switch (sortName)
                        {
                            case "Isbn":
                                data = data.OrderByDescending(x => x.Isbn);
                                break;
                            case "Title":
                                data = data.OrderByDescending(x => x.Title);
                                break;
                            case "MaterialTypeCode":
                                data = data.OrderByDescending(x => x.MaterialTypeId);
                                break;
                            case "MediaMaterialName":
                                data = data.OrderByDescending(x => x.MaterialTypeId);
                                break;
                            case "PublishDate":
                                data = data.OrderByDescending(x => x.PublishDate);
                                break;
                            case "HasConfiguration":
                                data = data.OrderByDescending(x => x.PublishDate);
                                break;
                            case "LatestChangeDateInJini":
                                data = data.OrderByDescending(x => x.CreatedDate);
                                break;
                            case "ConfigurationText":
                                data = data.OrderByDescending(x => x.State);
                                break;
                        }

                        break;
                }
            }
            else
            {
                data = data.OrderByDescending(x => x.PublishDate);
            }

            return data;
        }

        private IQueryable<DeflatedSalesConfigurationV2Dto> SortData(IQueryable<DeflatedSalesConfigurationV2Dto> data, string sortName, string order)
        {
            if (!string.IsNullOrEmpty(sortName) && !string.IsNullOrEmpty(order))
            {
                if (order == "asc")
                {
                    switch (sortName)
                    {
                        case "Isbn":
                            data = data.OrderBy(x => x.Isbn);
                            break;
                        case "Title":
                            data = data.OrderBy(x => x.Title);
                            break;
                        case "RefSalesDisplayName":
                            data = data.OrderBy(x => x.RefSalesDisplayName);
                            break;
                        case "RefPeriodTypeDisplayName":
                            data = data.OrderBy(x => x.RefPeriodTypeDisplayName);
                            break;
                        case "ReleaseDate":
                            data = data.OrderBy(x => x.ReleaseDate);
                            break;
                        case "LastModified":
                            data = data.OrderBy(x => x.LastModified);
                            break;

                        case "UnitPriceVat":
                            data = data.OrderBy(x => x.UnitPriceVat);
                            break;
                        case "UnitPrice":
                            data = data.OrderBy(x => x.UnitPrice);
                            break;

                    }
                }

                if (order == "desc")
                {
                    switch (sortName)
                    {
                        case "Isbn":
                            data = data.OrderByDescending(x => x.Isbn);
                            break;
                        case "Title":
                            data = data.OrderByDescending(x => x.Title);
                            break;
                        case "RefSalesDisplayName":
                            data = data.OrderByDescending(x => x.RefSalesDisplayName);
                            break;
                        case "RefPeriodTypeDisplayName":
                            data = data.OrderByDescending(x => x.RefPeriodTypeDisplayName);
                            break;
                        case "ReleaseDate":
                            data = data.OrderByDescending(x => x.ReleaseDate);
                            break;
                        case "LastModified":
                            data = data.OrderByDescending(x => x.LastModified);
                            break;
                        case "UnitPrice":
                            data = data.OrderByDescending(x => x.UnitPrice);
                            break;
                        case "UnitPriceVat":
                            data = data.OrderByDescending(x => x.UnitPriceVat);
                            break;
                    }
                }
            }
            else
            {
                data = data.OrderByDescending(x => x.LastModified);
            }

            return data;
        }

    }
}
