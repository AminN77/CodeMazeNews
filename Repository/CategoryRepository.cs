using Contracts;
using Entities;
using Entities.Context;

namespace Repository
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(RepositoryContext repositoryContext) 
            : base(repositoryContext)
        {
        }
    }
}