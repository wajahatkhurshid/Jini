using System.Collections.Generic;
using Gyldendal.Jini.Web.Common;

namespace Gyldendal.Jini.Web.Facade.V2
{
    public class JiniFacadeV2
    {
        public List<T> GetMediaAndMaterialTypes<T>() where T : class
        {
            var mediaTypes = Utils.GetAsync<List<T>>(Utils.JiniServiceUrl, "v2/JiniV2", "GetMediaAndMaterialTypes");
            return mediaTypes;
        }

        public List<T> GetDepartmentsAndEditorials<T>() where T : class
        {
            var mediaTypes = Utils.GetAsync<List<T>>(Utils.JiniServiceUrl, "v2/JiniV2", "GetDepartmentsAndEditorial");
            return mediaTypes;
        }
    }
}