using System;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CodeMazeSampleProject.ActionFilters
{
    public class ValidateNewsForCategoryExistAttribute : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public ValidateNewsForCategoryExistAttribute(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var trackChanges = (method.Equals("PUT")) || method.Equals("PATCH") ? true : false;
            var categoryId = (Guid) context.ActionArguments["categoryId"];
            var category = await _repository.Category.GetCategoryAsync(categoryId, false);
            if (category is null)
            {
                _logger.LogInfo($"Category with id :{categoryId} doesn't exist in the database");
                context.Result = new NotFoundResult();
                return;
            }

            var id = (Guid) context.ActionArguments["id"];
            var news = await _repository.News.GetNewsAsync(categoryId, id, trackChanges);

            if (news is null)
            {
                _logger.LogInfo($"News with id:{id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
                return;
            }
            
            context.HttpContext.Items.Add("news", news);
            await next();
        }
    }
}