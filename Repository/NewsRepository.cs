using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using Entities;
using Entities.Context;

namespace Repository
{
    public class NewsRepository : RepositoryBase<News>, INewsRepository
    {
        public NewsRepository(RepositoryContext repositoryContext) 
            : base(repositoryContext)
        {
        }

        public IEnumerable<News> GetNews(Guid categoryId, bool trackChanges) =>
            FindByCondition(n => n.CategoryId.Equals(categoryId), trackChanges)
                .OrderBy(n => n.Title);

        public News GetNews(Guid categoryId, Guid id, bool trackChanges) =>
            FindByCondition(n => n.CategoryId.Equals(categoryId) && n.Id.Equals(id), trackChanges)
                .SingleOrDefault();

    }
}