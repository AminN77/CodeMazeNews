using System;
using System.Collections.Generic;
using Entities;

namespace Contracts
{
    public interface INewsRepository
    {
        IEnumerable<News> GetNews(Guid categoryId, bool trackChanges);
        News GetNews(Guid categoryId, Guid id, bool trackChanges);
        void CreateNewsForCategory(Guid categoryId, News news);
        void DeleteNews(News news);
    }
}