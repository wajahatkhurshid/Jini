using System.Collections.Generic;

namespace Gyldendal.Jini.SalesConfigurationServices.Models
{
    public class GradeLevel
    {
        public string GradeLevelId { get; set; }
        public string GradeLevelName { get; set; }

        public List<Class> Classes { get; set; }
    }
}