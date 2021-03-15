namespace Contracts
{
    public interface IRepositoryManager
    {
        ICategoryRepository Category { get; }
        INewsRepository News { get; }
        void Save();
    }
}