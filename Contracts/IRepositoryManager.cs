using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryManager
    {
        ICategoryRepository Category { get; }
        INewsRepository News { get; }
        Task SaveAsync();
    }
}