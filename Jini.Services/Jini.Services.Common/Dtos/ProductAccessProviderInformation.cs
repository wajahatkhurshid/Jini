using System.Collections.Generic;
using Gyldendal.Api.ShopServices.Contracts.License.Access;


namespace Gyldendal.Jini.Services.Common.Dtos
{
   public class ProductAccessProviders 
    {
        public string Isbn { get; set; }
        public bool IsExternalLogin { get; set; }
        public List<EnumAccessProvider> AccessProvider { get; set; }
    }
}
