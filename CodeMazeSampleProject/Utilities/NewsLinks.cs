using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Net.Http.Headers;
using Contracts;
using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CodeMazeSampleProject.Utilities
{
    public class NewsLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<NewsDto> _dataShaper;
        
        public NewsLinks(LinkGenerator linkGenerator, IDataShaper<NewsDto> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }

        public LinkResponse TryGenerateLinks(IEnumerable<NewsDto> newsDtos, string fields, Guid categoryId,
            HttpContext httpContext)
        {
            var shapedNewsList = ShapeData(newsDtos, fields);
            if (ShouldGenerateLinks(httpContext))
            {
                return ReturnLikedNewsList(newsDtos, fields, categoryId, httpContext, shapedNewsList);
            }

            return ReturnShapedNewsList(shapedNewsList);
        }

        private LinkResponse ReturnShapedNewsList(List<Entity> shapedNewsList) => new LinkResponse {ShapedEntities = shapedNewsList};

        private LinkResponse ReturnLikedNewsList(IEnumerable<NewsDto> newsDtos, string fields, Guid categoryId, HttpContext httpContext,
            List<Entity> shapedNewsList)
        {
            var newsDtoList = newsDtos.ToList();
            for (var index = 0; index < newsDtoList.Count(); index++)
            {
                var newsLinks = CreateLinksForNews(httpContext, categoryId,
                    newsDtoList[index].Id, fields);
                shapedNewsList[index].Add("Links", newsLinks);
            }

            var newsCollection = new LinkCollectionWrapper<Entity>(shapedNewsList);
            var linkedNewsList = CreateLinksForNewsList(httpContext, newsCollection);
            return new LinkResponse {HasLinks = true, LinkedEntities = linkedNewsList};
        }

        private LinkCollectionWrapper<Entity> CreateLinksForNewsList(HttpContext httpContext, LinkCollectionWrapper<Entity> newsWrapper)
        {
            newsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, "GetAllNewsForCategory", values : new {}),
                "self",
                "GET"));
            return newsWrapper;
        }

        private List<Link> CreateLinksForNews(HttpContext httpContext, Guid categoryId, Guid id, string fields)
        {
            var links = new List<Link>
            {
                new Link(
                    _linkGenerator.GetUriByAction(httpContext, "GetNewsForCategory",
                        values: new {categoryId, id, fields}),
                    "self",
                    "GET"),
                new Link(
                    _linkGenerator.GetUriByAction(httpContext, "DeleteNewsForCategory", values: new {categoryId, id}),
                    "delete_news",
                    "DELETE"),
                new Link(
                    _linkGenerator.GetUriByAction(httpContext, "UpdateNewsForCategory", values: new {categoryId, id}),
                    "update_news",
                    "PUT"),
                new Link(
                    _linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateNewsForCategory",
                        values: new {categoryId, id}),
                    "partially_update_news",
                    "PATCH")
            };

            return links;
        }
        

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue) httpContext.Items["AcceptHeaderMediaType"];

            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas",
                StringComparison.InvariantCultureIgnoreCase);
        }

        private List<Entity> ShapeData(IEnumerable<NewsDto> newsDtos, string fields) =>
            _dataShaper.ShapeData(newsDtos, fields)
                .Select(n => n.Entity)
                .ToList();
    }
}