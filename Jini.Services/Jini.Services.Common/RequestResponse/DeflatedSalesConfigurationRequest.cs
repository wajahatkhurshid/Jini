using System;

namespace Gyldendal.Jini.Services.Common.RequestResponse
{
    public class DeflatedSalesConfigurationRequest
    {
        public string Isbn { get; set; }
        public string Title { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string ReleaseStartDate { get; set; }//for ReleaseDate filter
        public string ReleaseEndDate { get; set; }  //for ReleaseDate filter
        public DateTime? ModifiedDate { get; set; } //How to filter this as we dont have modifiedDate in SalesConfiguration
        public string ModifiedStartDate { get; set; }//for ModifiedDate filter
        public string ModifiedEndDate { get; set; }  //for ModifiedDate filter
        public string[] DepartmentCodes { get; set; }
        public string[] SectionCodes { get; set; }
        public string[] MaterialTypeCodes { get; set; }
        public string[] MediaTypeCodes { get; set; }
        public string[] SalesConfigurationStates { get; set; }
        public MediaMaterialTypeRequest[] MediaMaterialTypes { get; set; }
        public DepartmentsSectionsRequest[] DepartmentsSections { get; set; }
        public bool IsAllMediaTypesChecked { get; set; }
        public bool IsAllDepartmentChecked { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
