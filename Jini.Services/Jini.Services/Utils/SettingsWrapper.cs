using Gyldendal.Jini.Services.Properties;

namespace Gyldendal.Jini.Services.Utils
{
    public class SettingsWrapper : ISettingsWrapper
    {
        public string ActiveConfiguration => Settings.Default.ActiveConfiguration;
        public string InactiveConfiguration => Settings.Default.InactiveConfiguration;
        
    }
}