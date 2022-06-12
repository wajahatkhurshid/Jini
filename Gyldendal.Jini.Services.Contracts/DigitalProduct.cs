
using System;
using System.Collections.Generic;

namespace Gyldendal.Jini.Services.Contracts
{
    public class DigitalProduct
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Isbn { get; set; }
        public string MediaTypeName { get; set; }
        public string MediaTypeCode { get; set; }
        public string MaterialTypeName { get; set; }
        public string MaterialTypeCode { get; set; }
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }
        public string DepartmentSectionName => $"{DepartmentName} - {SectionName}";
        public string MediaMaterialName => $"{MediaTypeName} - {MaterialTypeName}";
        public DateTime? PublishDate { get; set; }
        public DateTime? LatestChangeDateInJini { get; set; }
        public DateTime? DeletedDate { get; set; }
        public List<DigitalProductAuthor> Authors { get; set; }
        public string DepartmentCode { get; set; }
        public string SectionCode { get; set; }
        public bool HasConfiguration { get; set; }
        public bool HasDraft { get; set; }
        public bool HasTrailAccess { get; set; }
        public string ConfigurationText { get; set; } = "Afventer";
        public bool HasProductAccess { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public int? State { get; set; }
    }
}
