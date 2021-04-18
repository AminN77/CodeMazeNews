using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Context;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;

namespace Repository
{
    public class NewsRepository : RepositoryBase<News>, INewsRepository
    {
        public NewsRepository(RepositoryContext repositoryContext) 
            : base(repositoryContext)
        {
        }

        public async Task<PagedList<News>> GetNewsAsync(Guid categoryId, NewsParameters newsParameters,
            bool trackChanges)
        {
          var newsList =  await FindByCondition(n => n.CategoryId.Equals(categoryId), trackChanges)
                .Search(newsParameters.SearchTerm)
                .OrderBy(n => n.Title)
                .ToListAsync();

          return PagedList<News>
              .ToPagedList(newsList, newsParameters.PageNumber, newsParameters.PageSize);
        }
           

        public async Task<News> GetNewsAsync(Guid categoryId, Guid id, bool trackChanges) =>
            await FindByCondition(n => n.CategoryId.Equals(categoryId) && n.Id.Equals(id), trackChanges)
                .SingleOrDefaultAsync();

        public void CreateNewsForCategory(Guid categoryId, News news)
        {
            news.CategoryId = categoryId;
            Create(news);
        }

        public void DeleteNews(News news)
        {
            Delete(news);
        }
    }
}