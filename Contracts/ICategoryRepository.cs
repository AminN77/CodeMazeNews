using System;
using System.Collections.Generic;
using Entities;

namespace Contracts
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories(bool trackChanges);
        Category GetCategory(Guid categoryId, bool trackChanges);
        void CreateCategory(Category category);
        IEnumerable<Category> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
        void DeleteCategory(Category category);
    }
}