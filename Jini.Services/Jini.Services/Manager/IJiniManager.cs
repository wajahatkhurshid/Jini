using System.Collections.Generic;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;

namespace Gyldendal.Jini.Services.Manager
{
    public interface IJiniManager
    {
        List<Data.SalesConfiguration> GetAllConfigurations();

        void UpdateConfigStatusOfCachedTitle(string isbn, string seller = "");

        List<Data.SalesConfiguration> PopulateSalesConfiguration(SalesConfiguration configuration);
    }
}