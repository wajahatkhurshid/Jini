using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Jini.SalesConfigurationServices.Models
{
    public class Department
    {
        /// <summary>
        /// Afdeling Id
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Afdeling Name
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// List of Redaktions against Afdeling Id of this object
        /// </summary>
        public IEnumerable<Section> ListOfSections { get; set; }

        public bool HasChild => ListOfSections.Any();
    }
}
