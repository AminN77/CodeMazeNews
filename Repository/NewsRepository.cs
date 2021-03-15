using Contracts;
using Entities;
using Entities.Context;

namespace Repository
{
    public class NewsRepository : RepositoryBase<News>, INewsRepository
    {
        public NewsRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}