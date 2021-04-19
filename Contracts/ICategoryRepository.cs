using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Entities.Models;

namespace Contracts
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges);
        Task<Category> GetCategoryAsync(Guid categoryId, bool trackChanges);
        void CreateCategory(Category category);
        Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
        void DeleteCategory(Category category);
    }
}