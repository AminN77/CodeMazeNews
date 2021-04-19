using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;

namespace Contracts
{
    public interface INewsRepository
    {
        Task<PagedList<News>> GetNewsAsync(Guid categoryId, NewsParameters newsParameters, bool trackChanges);
        Task<News> GetNewsAsync(Guid categoryId, Guid id, bool trackChanges);
        void CreateNewsForCategory(Guid categoryId, News news);
        void DeleteNews(News news);
    }
}