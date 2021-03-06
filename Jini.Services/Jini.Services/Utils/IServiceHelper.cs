using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Utils
{
    public interface IServiceHelper
    {
        Task<T> GetAsync<T>(string serviceBaseAddress, string addressSuffix);
        Task<T> GetAsync<T>(string serviceBaseAddress, params string[] methodAddress);
    }
}