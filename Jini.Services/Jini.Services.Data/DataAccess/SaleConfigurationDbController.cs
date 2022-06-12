using Gyldendal.AccessServices.Contracts.Enumerations;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.ShopServices.Contracts.Enumerations;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Common.WebUtils.Exceptions;
using Gyldendal.Jini.Services.Common;
using Gyldendal.Jini.Services.Common.Dtos;
using Gyldendal.Jini.Services.Common.ErrorHandling;
using Gyldendal.Jini.Services.Common.RequestResponse;
using Gyldendal.Jini.Services.Common.Services;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Data.MappingHelper;
using Gyldendal.Jini.Services.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Enums = Gyldendal.Jini.Services.Common.Enums;

namespace Gyldendal.Jini.Services.Data.DataAccess
{
    public class SaleConfigurationDbController : ISaleConfigurationDbController
    {
        private readonly ILogger _logger;

        private readonly Jini_Entities _dbContext;
        private IRapService _rapService;

        public SaleConfigurationDbController(Jini_Entities dbContext, IRapService rapService, ILogger logger)
        {
            _dbContext = dbContext;
            _rapService = rapService;
            _logger = logger;
        }

        private SalesConfiguration GetSalesConfiguration(string isbn, Enums.EnumState state)
        {
            return _dbContext.SalesConfigurations
                    .Include("TrialLicense")
                    .Include("TrialLicense.TrialCount").Include("TrialLicense.TrialCount.RefTrialCountUnitType")
                    .Include("TrialLicense.TrialPeriod").Include("TrialLicense.TrialPeriod.RefTrialPeriodUnitType")
                    .Include("TrialLicense.RefTrialAccessForm")
                    .Include("SalesForm").Include("Seller")
                    .Include("AccessForms")
                    .Include("AccessForms.PriceModels")
                    .Include("AccessForms.PriceModels.RefPriceModel")
                    .Include("AccessForms.RefAccessForm")
                    .Include("AccessForms.PeriodPrices").Include("AccessForms.PeriodPrices.RefPeriodUnitType")
                    .Include("SalesForm.RefSalesForm")
                    .FirstOrDefault(x => x.Isbn == isbn //&& x.Seller.WebShopId == (int)webShop
                                                        && x.State == (int)state
                                         && (x.RefSalesConfigTypeCode == null || x.RefSalesConfigTypeCode == (int)SalesConfigurationType.External)); // SalesConfigurationType check added to differentiate between Internal and External Sales Configuration
        }

        // This new function added for fetching sales configuration on the basis of SalesConfigType
        public SalesConfiguration GetSaleConfiguration(string isbn, Enums.EnumState state, WebShop webShop, SalesConfigurationType salesConfigType)
        {
            return _dbContext.SalesConfigurations
            .Include("TrialLicense")
            .Include("TrialLicense.TrialCount").Include("TrialLicense.TrialCount.RefTrialCountUnitType")
            .Include("TrialLicense.TrialPeriod").Include("TrialLicense.TrialPeriod.RefTrialPeriodUnitType")
            .Include("TrialLicense.RefTrialAccessForm")
            .Include("SalesForm").Include("Seller")
            .Include("AccessForms")
            .Include("AccessForms.PriceModels")
            .Include("AccessForms.PriceModels.RefPriceModel")
            .Include("AccessForms.RefAccessForm")
            .Include("AccessForms.PeriodPrices").Include("AccessForms.PeriodPrices.RefPeriodUnitType")
            .Include("SalesForm.RefSalesForm")
            .FirstOrDefault(x => x.Isbn == isbn //&& x.Seller.WebShopId == (int)webShop
                                                && x.State == (int)state
                                 && (x.RefSalesConfigTypeCode == (int)salesConfigType ||
                                     (x.RefSalesConfigTypeCode == null && (int)salesConfigType != (int)SalesConfigurationType.Internal))); // SalesConfigurationType check added to differentiate between Internal and External Sales Configuration
        }

        /// <summary>
        /// Get Sales Configuration for webshop
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="webShop"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public SalesConfiguration GetSaleConfigurationForWebShop(string isbn, WebShop webShop, Enums.EnumState state)
        {
            return GetSalesConfiguration(isbn, state);
        }

        /// <summary>
        /// Get Sales configuration Exists or not for Front End
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="seller"></param>
        /// <returns></returns>
        public List<int> SalesConfigurationExistBySeller(string isbn, string seller, int refSalesConfigTypeCode = (int)SalesConfigurationType.External)
        {
            if (refSalesConfigTypeCode == (int)SalesConfigurationType.Internal)
            {
                return _dbContext.SalesConfigurations.Where(sc => sc.Isbn.Equals(isbn)
                                    && (sc.RefSalesConfigTypeCode == refSalesConfigTypeCode 
                                    &&  sc.Seller.Name.Equals(seller,
                                        StringComparison.InvariantCultureIgnoreCase)))
                                    .Select(sc => sc.State).ToList();
            }
            else
            {

                return _dbContext.SalesConfigurations.Where(sc => sc.Isbn.Equals(isbn)
                                    && (sc.RefSalesConfigTypeCode == null || sc.RefSalesConfigTypeCode == (int)SalesConfigurationType.External)
                                    &&  sc.Seller.Name.Equals(seller,
                                        StringComparison.InvariantCultureIgnoreCase))
                                    .Select(sc => sc.State).ToList();
            }
        }

        /// <summary>
        /// Get whether SalesConfiguration exists or not for webshop
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="webShop"></param>
        /// <returns></returns>
        public List<int> SalesConfigurationExistByWebShop(string isbn, WebShop webShop)
        {
            return _dbContext.SalesConfigurations.Include("Seller").Where(sc => sc.Isbn.Equals(isbn)
                                                                  && (sc.RefSalesConfigTypeCode == null || sc.RefSalesConfigTypeCode == (int)SalesConfigurationType.External) // SalesConfigurationType check added to differentiate between Internal and External Sales Configuration
                                                                                                                                                                              //&& sc.Seller.WebShopId == (int)webShop
                                                                  ).Select(sc => sc.State).ToList();
        }

        public List<SalesConfiguration> GetSalesConfigurations(string isbn, Api.ShopServices.Contracts.Enumerations.Seller seller, int refSalesCode)
        {

            if ((int)SalesConfigurationType.Internal == refSalesCode)
            {
                return
               _dbContext.SalesConfigurations.Where(sc =>
                                                    sc.Isbn.Equals(isbn)
                                                    && (sc.RefSalesConfigTypeCode == refSalesCode))
                                                   .ToList();
            }
            else
            {
                return
               _dbContext.SalesConfigurations.Where(sc =>
                                                    sc.Isbn.Equals(isbn) && (sc.RefSalesConfigTypeCode == null ||
                                                    sc.RefSalesConfigTypeCode == (int)SalesConfigurationType.External))
                                                   .ToList();
            }


        }

        public SalesConfiguration GetSalesConfigurationByState(string isbn, int stateId, string seller)
        {
            return
                  _dbContext.SalesConfigurations.FirstOrDefault(
                      sc =>
                          sc.Isbn.Equals(isbn) && sc.State == stateId && (sc.RefSalesConfigTypeCode == null || sc.RefSalesConfigTypeCode == (int)SalesConfigurationType.External) && // SalesConfigurationType check added to differentiate between Internal and External Sales Configuration
                          sc.Seller.Name.Equals(seller, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool CreateSalesConfiguration(SalesConfiguration configuration)
        {
            try
            {
                var configInJson = string.Empty;
                if (configuration.State == Enums.ToInt(Enums.EnumState.Approved))
                {
                    configInJson = ConvertObjToJson(configuration);
                }
                _dbContext.SalesConfigurations.Add(configuration);
                if (configuration.State == Enums.ToInt(Enums.EnumState.Approved))
                {
                    var configHistory = new SalesConfigurationHistory
                    {
                        Value = configInJson,
                        CreatedDate = DateTime.Now.ToUniversalTime(),
                        CreatedBy = configuration.CreatedBy,
                        VersionNo = configuration.RevisionNumber,
                        Isbn = configuration.Isbn
                    };
                    _dbContext.SalesConfigurationHistories.Add(configHistory);
                }

                // Save changes and Commit Transaction
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInfo(_logger.GetPropertyNameAndValue(() => configuration), isGdprSafe: false);
                _logger.LogError(ex.Message, ex, isGdprSafe: false);
                return false;
            }
        }

        public bool UpdateSalesConfiguration(List<SalesConfiguration> oldConfigurations,
            SalesConfiguration newConfiguration)
        {
            try
            {
                using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
                {
                    // If new Configuration is in Approved State then remove all existing
                    if (newConfiguration.State == Enums.ToInt(Enums.EnumState.Approved))
                    {
                        var approvedConfig =
                            oldConfigurations.FirstOrDefault(c => c.State == Enums.ToInt(Enums.EnumState.Approved));
                        if (approvedConfig != null)
                        {
                            newConfiguration.RevisionNumber = ++approvedConfig.RevisionNumber;
                        }
                        foreach (var oldSalesConfiguration in oldConfigurations)
                        {
                            _dbContext.SalesConfigurations.Attach(oldSalesConfiguration);
                        }

                        // Remove previous Sales Configuration
                        _dbContext.SalesConfigurations.RemoveRange(oldConfigurations);

                        // Create History of New Configuration
                        var configInJson = ConvertObjToJson(newConfiguration);
                        var configHistory = new SalesConfigurationHistory
                        {
                            Value = configInJson,
                            CreatedDate = DateTime.Now.ToUniversalTime(),
                            CreatedBy = newConfiguration.CreatedBy,
                            VersionNo = newConfiguration.RevisionNumber,
                            Isbn = newConfiguration.Isbn
                        };
                        _dbContext.SalesConfigurationHistories.Add(configHistory);
                    }
                    else
                    {
                        // if new configuration is in Draft State then get Draft from list
                        var oldConfig = oldConfigurations.FirstOrDefault(c => c.State == newConfiguration.State);

                        // if draft exists then remove it
                        if (oldConfig != null)
                        {
                            _dbContext.SalesConfigurations.Attach(oldConfig);
                            _dbContext.SalesConfigurations.Remove(oldConfig);
                        }
                    }

                    // Add new Sales Configuration either Draft/Approved
                    _dbContext.SalesConfigurations.Add(newConfiguration);

                    // Save changes and Commit Transaction
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo(_logger.GetPropertyNameAndValue(() => oldConfigurations), isGdprSafe: false);
                _logger.LogInfo(_logger.GetPropertyNameAndValue(() => newConfiguration), isGdprSafe: false);
                _logger.LogError(ex.Message, ex, isGdprSafe: false);
                return false;
            }
        }

        public bool UpdateBulkSalesConfiguration(List<SalesConfiguration> oldConfigurations,
           SalesConfiguration newConfiguration)
        {
            try
            {

                if (newConfiguration.State == Enums.ToInt(Enums.EnumState.Approved))
                {
                    var approvedConfig =
                        oldConfigurations.FirstOrDefault(c => c.State == Enums.ToInt(Enums.EnumState.Approved));
                    if (approvedConfig != null)
                    {
                        newConfiguration.RevisionNumber = ++approvedConfig.RevisionNumber;
                    }
                    // Create History of New Configuration
                    var configInJson = ConvertObjToJson(newConfiguration);
                    var configHistory = new SalesConfigurationHistory
                    {
                        Value = configInJson,
                        CreatedDate = DateTime.Now.ToUniversalTime(),
                        CreatedBy = newConfiguration.CreatedBy,
                        VersionNo = newConfiguration.RevisionNumber,
                        Isbn = newConfiguration.Isbn
                    };
                    var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };

                    using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
                    {
                        // Remove previous Sales Configuration
                        _dbContext.SalesConfigurations.RemoveRange(oldConfigurations);

                        _dbContext.SaveChanges();
                        //Complete the scope
                        scope.Complete();
                    }
                    try
                    {
                        _dbContext.SalesConfigurationHistories.Add(configHistory);
                        _dbContext.SalesConfigurations.Add(newConfiguration);
                        _dbContext.SaveChanges();
                    }
                    catch (Exception)
                    {
                        // if exception occure we manaully add oldConfiguration which are delete in transaction 
                        foreach (var oldSalesConfiguration in oldConfigurations)
                        {
                            _dbContext.SalesConfigurations.Attach(oldSalesConfiguration);
                        }
                        _dbContext.SaveChanges();
                    }

                    // Save changes and Commit Transaction


                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInfo(_logger.GetPropertyNameAndValue(() => oldConfigurations), isGdprSafe: false);
                _logger.LogInfo(_logger.GetPropertyNameAndValue(() => newConfiguration), isGdprSafe: false);
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }

        public bool DeleteSalesConfiguration(string[] isbns)
        {
            try
            {
                _dbContext.SalesConfigurations.RemoveRange(_dbContext.SalesConfigurations.Where(x => isbns.Contains(x.Isbn)));
                _dbContext.SalesConfigurationHistories.RemoveRange(_dbContext.SalesConfigurationHistories.Where(x => isbns.Contains(x.Isbn)));
                // Save changes and Commit Transaction
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }

        public List<SalesConfigurationHistory> GetConfigurationVersionsHistory(string isbn)
        {
            _logger.LogInfo(_logger.GetPropertyNameAndValue(() => isbn));
            try
            {
                var history =
                    _dbContext.SalesConfigurationHistories.Where(x => x.Isbn.Equals(isbn))
                        .ToList()
                        .Select(e => new SalesConfigurationHistory
                        {
                            CreatedDate = e.CreatedDate,
                            CreatedBy = e.CreatedBy,
                            VersionNo = e.VersionNo
                        });
                return history.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        public List<SalesConfigurationRevisionHistoryDto> GetConfigurationVersionsRevisionHistory(string isbn)
        {
            _logger.LogInfo(_logger.GetPropertyNameAndValue(() => isbn));
            try
            {
                var history = _dbContext.SalesConfigurationHistories.Where(x => x.Isbn.Equals(isbn))
                    .ToList();
                var revisionHistory = history.Select(x => x.MapSalesConfigurationRevisionHistoryDt())
                    .Where(x => x.SalesConfiguration.RefSalesConfigTypeCode == null ||
                    x.SalesConfiguration.RefSalesConfigTypeCode == (int)SalesConfigurationType.External);
                return revisionHistory.OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        public List<SalesConfigurationRevisionHistoryDto> GetGUAConfigurationVersionsRevisionHistory(string isbn)
        {
            _logger.LogInfo(_logger.GetPropertyNameAndValue(() => isbn));
            try
            {
                var history = _dbContext.SalesConfigurationHistories.Where(x => x.Isbn.Equals(isbn))
                    .ToList();
                var revisionHistory = history.Select(x => x.MapSalesConfigurationRevisionHistoryDt())
                    .Where(x => x.SalesConfiguration.RefSalesConfigTypeCode != null &&
                    x.SalesConfiguration.RefSalesConfigTypeCode == (int)SalesConfigurationType.Internal);
                return revisionHistory.OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }
        private string ConvertObjToJson(object obj)
        {
            string json;
            try
            {
                json = JsonConvert.SerializeObject(obj,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
            }
            catch (Exception ex)
            {
                _logger.LogInfo(_logger.GetPropertyNameAndValue(() => obj), true, isGdprSafe: false);
                _logger.LogError(ex.Message, ex, isGdprSafe: false);
                json = obj.ToString();
            }
            return json;
        }
        public async Task<List<ProductConfigWithAccessProvider>> GetAllSalesConfigurations()
        {
            try
            {
                var q =
                    (from config in _dbContext.SalesConfigurations
                     join p in _dbContext.Products on config.Isbn equals p.Isbn into prod
                     from prodct in prod.DefaultIfEmpty()
                     join pap in _dbContext.ProductAccessProviders.Include("Product") on config.Isbn equals pap.Product.Isbn into prodAP
                     from prodctAccessProvider in prodAP.DefaultIfEmpty()
                     where config.RefSalesConfigTypeCode == null || config.RefSalesConfigTypeCode == (int)SalesConfigurationType.External

                     select new ProductConfigWithAccessProvider()
                     {
                         Isbn = config.Isbn,
                         CreatedBy = config.CreatedBy,
                         RefSalesConfigTypeCode = config.RefSalesConfigTypeCode,
                         CreatedDate = config.CreatedDate,
                         RevisionNumber = config.RevisionNumber,
                         SalesChannel = config.SalesChannel,
                         SalesFormId = config.SalesFormId,
                         SellerId = config.SellerId,
                         State = config.State,
                         TrialLicenseId = config.TrialLicenseId,
                         ProductAccessId = (prodctAccessProvider != null) ? prodctAccessProvider.AccessProviderId : 0,
                         HasProductAccess = (prodctAccessProvider != null && prodctAccessProvider.AccessProviderId != 0) || (prodct != null && prodct.IsExternalLogin),
                     }).ToList();

                var r =
                    (from prodct in _dbContext.Products
                     join pap in _dbContext.ProductAccessProviders on prodct.Id equals pap.ProductId into prodctJoined
                     from papD in prodctJoined.DefaultIfEmpty()
                     join cc in _dbContext.SalesConfigurations on prodct.Isbn equals cc.Isbn into joined
                     from c in joined.DefaultIfEmpty()
                     where c.RefSalesConfigTypeCode == null || c.RefSalesConfigTypeCode == 1001

                     select new ProductConfigWithAccessProvider()
                     {
                         Isbn = prodct.Isbn ?? (c != null ? c.Isbn : ""),
                         CreatedBy = (c != null ? c.CreatedBy : null),
                         RefSalesConfigTypeCode = (c != null ? c.RefSalesConfigTypeCode : null),
                         CreatedDate = (c != null ? c.CreatedDate : DateTime.MinValue),
                         RevisionNumber = (c != null ? c.RevisionNumber : 0),
                         SalesChannel = (c != null ? c.SalesChannel : null),
                         SalesFormId = (c != null ? c.SalesFormId : null),
                         SellerId = (c != null ? c.SellerId : 0),
                         State = (c != null ? c.State : (int?)null),
                         TrialLicenseId = (c != null ? c.TrialLicenseId : null),
                         ProductAccessId = (papD != null ? papD.AccessProviderId : 0),
                         HasProductAccess = ((papD != null ? papD.AccessProviderId : 0) > 0 || prodct.IsExternalLogin)
                     }).ToList();

                return q.Union(r).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Returns the desired length list of ISBNs, of the products for which the Sales Configuration has been changed, after the DateTime value passed in.
        /// </summary>
        /// <param name="updatedAfter"></param>
        /// <param name="webShop"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IEnumerable<UpdatedSalesConfigInfo> GetUpdatedSaleConfigsInfo(DateTime updatedAfter,
            WebShop webShop, int take)
        {
            var updatedIsbns = _dbContext.SalesConfigurations
                .Where(x => x.CreatedDate > updatedAfter //&& x.Seller.WebShopId == (int)webShop
                                                         && x.State == (int)Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumState.Approved
                                                         && (x.RefSalesConfigTypeCode == null || x.RefSalesConfigTypeCode == (int)SalesConfigurationType.External)) // SalesConfigurationType check added to differentiate between Internal and External Sales Configuration
                .OrderBy(x => x.CreatedDate)
                .Take(take)
                .Select(x => new { x.Isbn, x.CreatedDate })
                .ToArray()
                .Select(x => new UpdatedSalesConfigInfo { Isbn = x.Isbn, UpdatedAt = x.CreatedDate })
                .ToArray();

            return updatedIsbns;
        }

        public Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.SalesConfiguration GetSalesConfigurationForPriceUpates(PriceModelRequest request)
        {

            var salesConfig = new Api.ShopServices.Contracts.SalesConfiguration.SalesConfiguration();
            var UniqueIdentifer = DecodeUniqueId(request.DeflatedPrice.FirstOrDefault()?.Id);
            var state = Convert.ToInt32(UniqueIdentifer["state"]);
            int? salesConfigTypeCode = null;
            if (UniqueIdentifer["RefSalesConfigTypeCode"] != "null")
            {
                salesConfigTypeCode = Convert.ToInt32(UniqueIdentifer["RefSalesConfigTypeCode"]);
            }

            //_dbContext.Configuration.LazyLoadingEnabled = false;
            var salesObj = new SalesConfiguration();

            if (salesConfigTypeCode == (int)SalesConfigurationType.Internal)
            {


                salesObj = _dbContext.SalesConfigurations.AsNoTracking()
                    .Include("TrialLicense")
                    .Include("TrialLicense.TrialCount")
                    .Include("TrialLicense.TrialCount.RefTrialCountUnitType")
                    .Include("TrialLicense.TrialPeriod")
                    .Include("TrialLicense.TrialPeriod.RefTrialPeriodUnitType")
                    .Include("TrialLicense.RefTrialAccessForm")
                    .Include("SalesForm")
                    .Include("Seller")
                    .Include("AccessForms")
                    .Include("AccessForms.PriceModels")
                    .Include("AccessForms.PriceModels.RefPriceModel")
                    .Include("AccessForms.RefAccessForm")
                    .Include("AccessForms.PeriodPrices")
                    .Include("AccessForms.PeriodPrices.RefPeriodUnitType")
                    .Include("SalesForm.RefSalesForm")
                    .FirstOrDefault(x => x.Isbn == request.Isbn //&& x.Seller.WebShopId == (int)webShop
                                         && x.State == state
                                         && (x.RefSalesConfigTypeCode == salesConfigTypeCode));
            }
            else
            {
                salesObj = _dbContext.SalesConfigurations.AsNoTracking()
                   .Include("TrialLicense")
                   .Include("TrialLicense.TrialCount")
                   .Include("TrialLicense.TrialCount.RefTrialCountUnitType")
                   .Include("TrialLicense.TrialPeriod")
                   .Include("TrialLicense.TrialPeriod.RefTrialPeriodUnitType")
                   .Include("TrialLicense.RefTrialAccessForm")
                   .Include("SalesForm")
                   .Include("Seller")
                   .Include("AccessForms")
                   .Include("AccessForms.PriceModels")
                   .Include("AccessForms.PriceModels.RefPriceModel")
                   .Include("AccessForms.RefAccessForm")
                   .Include("AccessForms.PeriodPrices")
                   .Include("AccessForms.PeriodPrices.RefPeriodUnitType")
                   .Include("SalesForm.RefSalesForm")
                   .FirstOrDefault(x => x.Isbn == request.Isbn //&& x.Seller.WebShopId == (int)webShop
                                        && x.State == state
                                        && (x.RefSalesConfigTypeCode == salesConfigTypeCode ||
                                            (x.RefSalesConfigTypeCode == null && salesConfigTypeCode != (int)SalesConfigurationType.Internal)));
            }

            foreach (var ac in salesObj.AccessForms)
            {
                foreach (var deflatedSal in request.DeflatedPrice)
                {
                    var identifer = DecodeUniqueId(deflatedSal.Id);
                    int? accessFormType = null;
                    if (identifer["RefAccessFormCode"] != null)
                    {
                        accessFormType = Convert.ToInt32(identifer["RefAccessFormCode"]);
                    }
                    int? periodTypeCode = null;
                    if (identifer["RefPeriodTypeCode"] != null)
                    {
                        periodTypeCode = Convert.ToInt32(identifer["RefPeriodTypeCode"]);
                    }
                    int? unitValue = null;
                    if (identifer["UnitValue"] != null)
                    {
                        unitValue = Convert.ToInt32(identifer["UnitValue"]);
                    }

                    if (ac.RefCode == accessFormType)
                    {

                        foreach (var pp in ac.PeriodPrices)
                        {

                            if (pp.RefPeriodTypeCode == periodTypeCode && pp.UnitValue == unitValue)
                            {
                                pp.UnitPrice = deflatedSal.UnitPrice;
                                pp.UnitPriceVat = deflatedSal.UnitPriceVat;
                            }

                        }
                    }

                }

            }

            salesConfig.Seller = (Api.ShopServices.Contracts.Enumerations.Seller)salesObj.SellerId;
            salesConfig.Isbn = salesObj.Isbn;
            salesConfig.CreatedDate = (DateTime)request.DeflatedPrice.FirstOrDefault()?.CreatedDate;
            salesConfig.CreatedBy = salesObj.CreatedBy;
            salesConfig.SalesChannel = salesObj.SalesChannel;
            salesConfig.Approved = salesObj.TrialLicenseId > 0;
            salesConfig.HasTrialAccess = salesObj.TrialLicenseId != null && salesObj.State == (int)Enums.EnumState.Approved;
            salesConfig.TrialAccess = DbTrialLicenseToTrialLicense(salesObj);
            salesConfig.SalesForms = DbSaleFormToSaleForm(salesObj);
            salesConfig.AccessForms = DbAccessFormToAccessForm(salesObj);
            salesConfig.Approved = true;
            return salesConfig;
        }


        public TrialAccess GetTrialLicense(string isbn, string seller)
        {

            var trialLicense = GetTrialLicenseFromDb(isbn, seller);
            if (trialLicense == null)
                throw new Exception("Trial License Not Exists"); //todo : handle proper exception
            return TrialLicenseToTrialAccess(trialLicense);

        }

        private TrialAccess TrialLicenseToTrialAccess(Data.TrialLicense trialLicense)
        {
            return new TrialAccess
            {
                TrialAccessCount = new TrialAccessCount()
                {
                    UnitType = new TrialCountUnitType()
                    {
                        Code = (TrialCountUnitTypeCode)trialLicense.TrialCount.RefCountUnitTypeCode,
                        DisplayName = $"{trialLicense.TrialCount.UnitValue} {((TrialCountUnitTypeCode)trialLicense.TrialCount.RefCountUnitTypeCode).ToString()}"
                    },
                    UnitValue = trialLicense.TrialCount.UnitValue ?? 0
                },
                AccessForm = new TrialAccessForm
                {
                    Code = (EnumTrialAccessForm)trialLicense.RefTrialAccessForm.Code,
                    DisplayName = trialLicense.RefTrialAccessForm.DisplayName
                },
                MultipleTrialsPerUser = trialLicense.MultipleTrials,
                ContactSales = trialLicense.ContactSalesText,
                Period = new Api.ShopServices.Contracts.SalesConfiguration.TrialPeriod
                {
                    UnitValue = trialLicense.TrialPeriod.UnitValue,
                    UnitType = new TrialPeriodUnitType()
                    {
                        Code = (TrialPeriodUnitTypeCode)trialLicense.TrialPeriod.RefTrialPeriodTypeCode,
                        DisplayName = $"{trialLicense.TrialPeriod.UnitValue} {((TrialPeriodUnitTypeCode)trialLicense.TrialPeriod.RefTrialPeriodTypeCode).ToString()}"
                    },
                }
            };
        }

        private Data.TrialLicense GetTrialLicenseFromDb(string isbn, string seller)
        {
            var salesConfiguration =
                _dbContext.SalesConfigurations
                    .Include("TrialLicense")
                    .Include("TrialLicense.TrialPeriod")
                    .Include("TrialLicense.TrialCount")
                    .Include("TrialLicense.Seller")
                    .Include("TrialLicense.RefTrialAccessForm")
                    .FirstOrDefault(x => x.Isbn.Equals(isbn));
            return salesConfiguration?.TrialLicense;
        }


        private TrialAccess DbTrialLicenseToTrialLicense(SalesConfiguration dbSaleConfiguration)
        {
            if (dbSaleConfiguration.TrialLicense != null)
            {
                TrialAccess trialAccess = new TrialAccess()
                {
                    MultipleTrialsPerUser = dbSaleConfiguration.TrialLicense.MultipleTrials,
                    ContactSales = dbSaleConfiguration.TrialLicense.ContactSalesText
                };
                if (dbSaleConfiguration.TrialLicense.TrialAccessFormCode != null)
                {
                    trialAccess.AccessForm = new TrialAccessForm
                    {
                        Code = (EnumTrialAccessForm)dbSaleConfiguration.TrialLicense.RefTrialAccessForm.Code,
                        DisplayName = dbSaleConfiguration.TrialLicense.RefTrialAccessForm.DisplayName
                    };
                }
                if (dbSaleConfiguration.TrialLicense.TrialPeriodId != null)
                {
                    trialAccess.Period = new Api.ShopServices.Contracts.SalesConfiguration.TrialPeriod
                    {
                        UnitValue = dbSaleConfiguration.TrialLicense.TrialPeriod.UnitValue,
                        UnitType = new TrialPeriodUnitType()
                        {
                            Code = (TrialPeriodUnitTypeCode)dbSaleConfiguration.TrialLicense.TrialPeriod.RefTrialPeriodTypeCode,
                            DisplayName = dbSaleConfiguration.TrialLicense.TrialPeriod.RefTrialPeriodUnitType.DisplayName
                        }
                    };
                }
                if (dbSaleConfiguration.TrialLicense.TrialCountId != null)
                {
                    trialAccess.TrialAccessCount = new TrialAccessCount()
                    {
                        UnitType = new TrialCountUnitType()
                        {
                            Code =
                                (TrialCountUnitTypeCode)dbSaleConfiguration.TrialLicense.TrialCount.RefCountUnitTypeCode,
                            DisplayName = dbSaleConfiguration.TrialLicense.TrialCount.RefTrialCountUnitType.DisplayName
                        },
                        UnitValue = dbSaleConfiguration.TrialLicense.TrialCount.UnitValue ?? 0
                    };
                }
                return trialAccess;
            }

            return null;
        }

        private List<Api.ShopServices.Contracts.SalesConfiguration.SalesForm> DbSaleFormToSaleForm(SalesConfiguration dbSaleConfiguration)
        {
            if (dbSaleConfiguration.SalesFormId != null)
            {
                // setting sales configuration
                return new List<Api.ShopServices.Contracts.SalesConfiguration.SalesForm>
                {
                    new Api.ShopServices.Contracts.SalesConfiguration.SalesForm
                    {
                        Code = (EnumLicenseType) (dbSaleConfiguration.SalesForm?.RefSalesFormCode ?? 0),
                        DisplayName = dbSaleConfiguration.SalesForm?.RefSalesForm.DisplayName,
                    }
                };
            }

            return new List<Api.ShopServices.Contracts.SalesConfiguration.SalesForm>();
        }

        private List<Api.ShopServices.Contracts.SalesConfiguration.AccessForm> DbAccessFormToAccessForm(SalesConfiguration dbSaleConfiguration)
        {
            return dbSaleConfiguration.AccessForms.Select(af => new Api.ShopServices.Contracts.SalesConfiguration.AccessForm
            {
                DisplayName = af.RefAccessForm.DisplayName,
                Code = (EnumAccessForm)af.RefCode,
                WebText = af.WebText,
                PriceModels =
                    af.PriceModels.Select(pm => new Api.ShopServices.Contracts.SalesConfiguration.PriceModel
                    {
                        Code = (Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel)pm.RefPriceModelCode,

                        DisplayName = pm.RefPriceModel.DisplayName,
                        GradeLevels = GetGrades(dbSaleConfiguration.Isbn, (Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel)pm.RefPriceModelCode),
                        PercentageValue = pm.PercentValue ?? 0,
                        ShowPercentage = pm.PercentValue != null,
                        RefAccessFormCode = pm.AccessForm.RefCode
                    }).ToList(),
                BillingPeriods = af.PeriodPrices.Select(pp => new Period
                {
                    DisplayName =
                        $"{pp.UnitValue} {pp.RefPeriodUnitType.DisplayName} {dbSaleConfiguration.SalesForm.RefSalesForm.PeriodTypeName}",
                    Currency = pp.Currency,
                    IsCustomPeriod = pp.IsCustomPeriod,
                    UnitValue = pp.UnitValue,
                    RefPeriodUnitTypeCode = (EnumPeriodUnitType)pp.RefPeriodUnitType.Code,
                    Price = new Price
                    {
                        VatValue = pp.VatValue,
                        UnitPrice = pp.UnitPrice,
                        UnitPriceVat = pp.UnitPriceVat
                    }
                }).OrderByDescending(bp => bp.RefPeriodUnitTypeCode).ThenBy(bp => bp.UnitValue).ToList(),
                DescriptionText = af.DescriptionText
            }).ToList();
        }
        private string GetGrades(string isbn, Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel priceModel)
        {
            switch (priceModel)
            {
                case Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel.NoOfReleventClasses:
                case Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel.NoOfStudentsInReleventClasses:
                    try
                    {
                        var getGradeLevelsTask = Task.Run(() => _rapService.GetGradeLevels(isbn));
                        getGradeLevelsTask.Wait();

                        var gradeLevelList = getGradeLevelsTask.Result;
                        if (!gradeLevelList.Any())
                            return "";

                        return ProcessGradeLevels.SortGradeLevels(gradeLevelList);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                        throw new ProcessException((ulong)ErrorCodes.GradesNotFound, ErrorCodes.GradesNotFound.GetDescription(), Constants.OrigionatingSystemName);
                    }

                default:
                    return "";
            }
        }

        public Dictionary<string, string> DecodeUniqueId(string uniqueId)
        {

            string[] splitedValues;
            var valuesToReturn = new Dictionary<string, string>();

            splitedValues = uniqueId.Split('_');

            valuesToReturn.Add("Isbn", splitedValues[0]);
            valuesToReturn.Add("RefSalesConfigTypeCode", splitedValues[1] == "null" ? null : splitedValues[1]);
            valuesToReturn.Add("RefSalesCode", splitedValues[2] == "null" ? null : splitedValues[2]);
            valuesToReturn.Add("RefAccessFormCode", splitedValues[3] == "null" ? null : splitedValues[3]);
            valuesToReturn.Add("RefPeriodTypeCode", splitedValues[4] == "null" ? null : splitedValues[4]);
            valuesToReturn.Add("RefPriceModelCode", splitedValues[5] == "null" ? null : splitedValues[5]);
            valuesToReturn.Add("UnitValue", splitedValues[6] == "null" ? null : splitedValues[6]);
            valuesToReturn.Add("state", splitedValues[7] == "null" ? null : splitedValues[7]);

            return valuesToReturn;

        }
    }
}