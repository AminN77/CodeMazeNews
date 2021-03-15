using Contracts;
using Entities.Context;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private  ICategoryRepository _categoryRepository;
        private  INewsRepository _newsRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public ICategoryRepository Category
        {
            get { return _categoryRepository ??= new CategoryRepository(_repositoryContext); }
        }
        public INewsRepository News
        {
            get { return _newsRepository ??= new NewsRepository(_repositoryContext); }
        }

        public void Save() => _repositoryContext.SaveChanges();
    }
}