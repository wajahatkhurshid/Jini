using System.Collections.ObjectModel;

namespace Gyldendal.Jini.Services.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ParameterDescription
    {
        /// <summary>
        /// 
        /// </summary>
        public ParameterDescription()
        {
            Annotations = new Collection<ParameterAnnotation>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Collection<ParameterAnnotation> Annotations { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Documentation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ModelDescription TypeDescription { get; set; }
    }
}