namespace Gyldendal.Jini.Services.Core.Seller
{
    public interface ISellerFacade
    {
        /// <summary>
        /// Get ID for Seller Name
        /// </summary>
        /// <param name="seller"></param>
        /// <returns></returns>
        int GetSellerId(string seller);

        int GetSellerWebShopId(string sellerName);

    }
}
