using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using Entities;
using Repository.Extensions.Utility;

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

        public static IQueryable<News> Sort(this IQueryable<News> newsList, string
            orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return newsList.OrderBy(n => n.Title);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<News>(orderByQueryString);
            if (string.IsNullOrWhiteSpace(orderQuery))
                return newsList.OrderBy(n => n.Title);

            return newsList.OrderBy(orderQuery);
        }
    }
}