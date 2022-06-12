namespace Gyldendal.Jini.Services.Utils
{
    public interface ISettingsWrapper
    {
        string ActiveConfiguration { get; }
        string InactiveConfiguration { get; }
    }
}