using System.Collections.Generic;
using Gyldendal.Jini.SalesConfigurationServices.Models;
using Gyldendal.Jini.SalesConfigurationServices.Models.Request;

namespace Gyldendal.Jini.SalesConfigurationServices.BusinessLayer.Interfaces
{
    public interface IPriceCalculator
    {
        PriceRequest GetPrice(string isbn, string institutionNumber, string uniCServiceUrl, PriceRequest salesConfiguration = null );
        AccessForm WholeSchool(string institutionNumber, AccessForm accessForm);
        AccessForm NoOfStudents(string institutionNumber, AccessForm accessForm, string uniCServiceUrl);
        AccessForm PercentageOfStudents(string institutionNumber, AccessForm accessForm, int percentage, string uniCServiceUrl);
        AccessForm NoOfStudentsInReleventClasses(string institutionNumber, AccessForm accessForm, string gradeLevels, string uniCServiceUrl);
        AccessForm NoOfReleventClasses(string institutionNumber, AccessForm accessForm, string gradeLevels, string uniCServiceUrl);
        AccessForm NoOfStudentsInClasses(string institutionNumber, AccessForm accessForm, List<Class> selectedClasses, string uniCServiceUrl);
        AccessForm NoOfClasses(string institutionNumber, AccessForm accessForm, List<Class> selectedClasses);
        AccessForm SingleUser(string instituionNumber, AccessForm accessForm);
    }
}
