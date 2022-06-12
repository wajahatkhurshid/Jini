using System.ComponentModel;

namespace Gyldendal.Jini.Services.Common.ErrorHandling
{
    /// <summary>
    /// Start Error codes by 900__
    /// </summary>
    public enum ErrorCodes : ulong
    {
        [Description("The error code provided is not valid")]
        InvalidErrorCode = 1,

        [Description("The AccessForm provided is not valid")]
        InvalidAccessForm = 90001,

        [Description("The SalesForm provided is not valid")]
        InvalidSalesForm = 90002,

        [Description("Parameter value is null")]
        NullValue = 90003,

        [Description("The Seller Name is Invalid")]
        InvalidSellerName = 90004,

        [Description("Unable to find grade levels for product.")]
        GradesNotFound = 90005,

        [Description("Provided container type: {0} is not supported")]
        UnsupportedContainerType = 0002,
    }
}