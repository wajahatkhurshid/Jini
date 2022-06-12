CREATE VIEW dbo.[DeflatedSalesConfigurationView]  
AS  
SELECT DISTINCT     
                         dbo.SalesConfiguration.Isbn, dbo.SalesConfiguration.SellerId, dbo.SalesConfiguration.SalesChannel, dbo.SalesConfiguration.State, dbo.SalesConfiguration.SalesFormId,     
                         dbo.SalesConfiguration.CreatedDate, dbo.SalesConfiguration.RevisionNumber, dbo.SalesConfiguration.CreatedBy, dbo.SalesConfiguration.TrialLicenseId, dbo.SalesConfiguration.RefSalesConfigTypeCode,     
                         dbo.RefSalesForm.Code AS RefSalesCode, dbo.RefSalesForm.DisplayName AS RefSalesDisplayName, dbo.RefAccessForm.Code AS RefAccessFormCode, dbo.RefAccessForm.DisplayName AS RefAccessFormDisplayName,     
                         dbo.PeriodPrice.UnitPrice, dbo.PeriodPrice.UnitPriceVat, dbo.PeriodPrice.UnitValue, dbo.PeriodPrice.RefPeriodTypeCode, dbo.RefPeriodUnitType.DisplayName AS RefPeriodTypeDisplayName,     
                         dbo.RefPriceModel.Code AS RefPriceModelCode, dbo.RefPriceModel.DisplayName AS RefPriceModelDisplayName,[dbo].[Product].DepartmentCode,
						coalesce ([dbo].[Product].[IsExternalLogin], 0) IsExternalLogin,
						 "ProductAccessProvider" = 
                              CASE 
                                 WHEN (select count(id) from [dbo].[ProductAccessProvider] where ProductId= [dbo].[Product].Id) =  0 THEN 0
                                 ELSE 1
                              END,
                          ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS Id  
FROM            dbo.SalesConfiguration LEFT OUTER JOIN    
                         dbo.SalesForm ON dbo.SalesConfiguration.SalesFormId = dbo.SalesForm.Id LEFT OUTER JOIN    
                         dbo.RefSalesForm ON dbo.SalesForm.RefSalesFormCode = dbo.RefSalesForm.Code LEFT OUTER JOIN    
                         dbo.AccessForm ON dbo.AccessForm.SalesConfigurationId = dbo.SalesConfiguration.Id LEFT OUTER JOIN    
                         dbo.RefAccessForm ON dbo.AccessForm.RefCode = dbo.RefAccessForm.Code LEFT OUTER JOIN    
                         dbo.PeriodPrice ON dbo.PeriodPrice.AccessFormId = dbo.AccessForm.Id LEFT OUTER JOIN    
                         dbo.RefPeriodUnitType ON dbo.RefPeriodUnitType.Code = dbo.PeriodPrice.RefPeriodTypeCode LEFT OUTER JOIN    
                         dbo.PriceModel ON dbo.PriceModel.AccessFormId = dbo.AccessForm.Id LEFT OUTER JOIN    
                         dbo.RefPriceModel ON dbo.RefPriceModel.Code = dbo.PriceModel.RefPriceModelCode  LEFT OUTER JOIN 
                         [dbo].[Product] ON dbo.SalesConfiguration.Isbn = [dbo].[Product].Isbn
						   
WHERE        (dbo.RefAccessForm.Code <> 1005) AND dbo.SalesConfiguration.State =1002
