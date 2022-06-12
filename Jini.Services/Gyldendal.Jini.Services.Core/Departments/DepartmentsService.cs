using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gyldendal.Jini.ExternalClients.AX;
using Gyldendal.Jini.ExternalClients.Interfaces;
using Gyldendal.Jini.Services.Contracts;


namespace Gyldendal.Jini.Services.Core.Departments
{
    public class DepartmentsService : IDepartmentsService
    {
        private readonly IAxApiClient _axApiClient;


        public DepartmentsService(IAxApiClient axApiClient)
        {
            _axApiClient = axApiClient;
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            var organizations = await _axApiClient.GetOrganizationsAsync(new CancellationToken(false));
            var departments = new List<Department>
            {
                GetSectionsByDepartment("GU - Grundskolen", organizations),
                GetSectionsByDepartment("GU - Videregående", organizations)
            };
            return departments;
        }

        private Department GetSectionsByDepartment(string department, IEnumerable<Organization> organizations)
        {
            var sectionList = organizations.Where(r => !string.IsNullOrEmpty(r.DepartmentSubgroup) && r.DepartmentSubgroup.Equals(department)).ToList();
            var sections = new List<Contracts.Section>();
            sectionList.ForEach(r =>
                sections.Add(
                    new Section()
                    {
                        SectionCode = r.DepartmentCode,
                        SectionName = r.DepartmentName
                    }
                )
            );
            return new Department()
            {
                Code = "",
                Name = department,
                Sections = sections
            };
        }
    }
}
