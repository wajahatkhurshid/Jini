namespace Gyldendal.Jini.Web.Models
{
    /// <summary>
    /// Media Type of Digital Titles
    /// </summary>
    public class MediaType
    {
        /// <summary>
        /// Media Type Code
        /// </summary>
        public string MediaTypeCode { get; set; }

        /// <summary>
        /// Media Type Name
        /// </summary>
        public string MediaTypeName { get; set; }

        /// <summary>
        /// List of Material Types for MediaTypeCode of current object.
        /// </summary>
        public MaterialType[] ListOfMaterialTypes { get; set; }
    }
}
