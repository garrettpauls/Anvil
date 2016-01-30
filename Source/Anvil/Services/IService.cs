using System.Threading.Tasks;

namespace Anvil.Services
{
    public interface IService
    {
    }

    public interface IInitializableService : IService
    {
        Task InitializeAsync();
    }
}