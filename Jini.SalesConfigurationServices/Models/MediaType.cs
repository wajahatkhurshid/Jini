namespace Gyldendal.Jini.SalesConfigurationServices.Models
{
    /// <summary>
    /// Media Type of Digital Titles
    /// </summary>
    public class MediaType
    {
        /// <summary>
        /// Media Type Id
        /// </summary>
        public int MediaTypeId { get; set; }

        /// <summary>
        /// Media Type Name
        /// </summary>
        public string MediaTypeName { get; set; }

        /// <summary>
        /// List of Material Types for MediaTypeId of current object.
        /// </summary>
        public MaterialType[] ListOfMaterialTypes { get; set; }
    }
}
