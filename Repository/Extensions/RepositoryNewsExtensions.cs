using System.Linq;
using Entities;

namespace Repository.Extensions
{
    public static class RepositoryNewsExtensions
    {
        public static IQueryable<News> Search(this IQueryable<News> newsList, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return newsList;

            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return newsList.Where(n => n.Title.ToLower().Contains(lowerCaseTerm));
        }
    }
}