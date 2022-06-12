using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Core.Departments
{
    public  interface IDepartmentsService
    {
        Task<List<Contracts.Department>> GetDepartmentsAsync();
    }
}
