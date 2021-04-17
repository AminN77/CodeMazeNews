using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface INewsRepository
    {
        Task<IEnumerable<News>> GetNewsAsync(Guid categoryId, bool trackChanges);
        Task<News> GetNewsAsync(Guid categoryId, Guid id, bool trackChanges);
        void CreateNewsForCategory(Guid categoryId, News news);
        void DeleteNews(News news);
    }
}