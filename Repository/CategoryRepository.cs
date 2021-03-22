using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<Category> GetAllCategories(bool trackChanges) =>
            FindAll(trackChanges)
                .OrderBy(c => c.Title)
                .ToList();

        public Category GetCategory(Guid categoryId, bool trackChanges) =>
            FindByCondition(c => c.Id.Equals(categoryId), trackChanges)
                .SingleOrDefault();

        public void CreateCategory(Category category) => Create(category);

        public IEnumerable<Category> GetByIds(IEnumerable<Guid> ids, bool trackChanges) =>
            FindByCondition(x => ids.Contains(x.Id), trackChanges)
                .ToList();
    }
}