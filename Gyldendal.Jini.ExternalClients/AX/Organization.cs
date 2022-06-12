using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gyldendal.Jini.ExternalClients.AX
{
    public class Organization
    {
        [JsonProperty("departmentKey")]
        public int DepartmentKey { get; set; }
        [JsonProperty("departmentCode")]
        public string DepartmentCode { get; set; }
        [JsonProperty("departmentCompany")]
        public string DepartmentCompany { get; set; }
        [JsonProperty("departmentBusinessArea")]
        public string DepartmentBusinessArea { get; set; }
        [JsonProperty("departmentGroup")]
        public string DepartmentGroup { get; set; }
        [JsonProperty("departmentSubgroup")]
        public string DepartmentSubgroup { get; set; }
        [JsonProperty("departmentReportDepartment")]
        public string DepartmentReportDepartment { get; set; }
        [JsonProperty("departmentName")]
        public string DepartmentName { get; set; }
        [JsonProperty("validFrom")]
        public DateTime ValidFrom { get; set; }
        [JsonProperty("validTo")]
        public DateTime ValidTo { get; set; }
        [JsonProperty("isCurrentRecord")]
        public bool? IsCurrentRecord { get; set; }
    }

    public class Organizations
    {
        [JsonProperty("organisations")]
        public List<Organization> OrganizationList { get; set; }
    }
}
