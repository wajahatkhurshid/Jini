using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Jini.Services.Contracts
{
    public class Department
    {
        /// <summary>
        /// Afdeling Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Afdeling Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of Redaktions against Afdeling Id of this object
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public List<Section> Sections { get; set; }

        public bool HasChild => Sections.Any();
    }
}
