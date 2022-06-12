using System;
using System.Reflection;

namespace Gyldendal.Jini.SalesConfigurationServices.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}