using System.Collections.Generic;
using Gyldendal.Jini.Web.Common;

namespace Gyldendal.Jini.Web.Facade
{
    public class JiniFacade
    {
        public List<T> GetDepartmentsAndEditorials<T>() where T : class
        {
            var departments = Utils.GetAsync<List<T>>(Utils.JiniServiceUrl, "v1/Jini","GetDepartmentsAndEditorial");
            return departments;
        }
        public List<T> GetMediaAndMaterialeTypes<T>() where T : class
        {
            var mediaTypes = Utils.GetAsync<List<T>>(Utils.JiniServiceUrl, "v1/Jini", "GetMediaAndMaterialeTypes");
            return mediaTypes;
        }
    }
}