using System;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CodeMazeSampleProject.ActionFilters
{
    public class ValidateCategoryExistsAttribute : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public ValidateCategoryExistsAttribute(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var trackChanges = context.HttpContext.Request.Method.Equals("PUT");
            var id = (Guid) context.ActionArguments["id"];
            var category = await _repository.Category.GetCategoryAsync(id, trackChanges);
            if (category is null)
            {
                _logger.LogInfo($"Category with id : {id} doesn't exist in the database");
                context.Result = new NotFoundResult();
                return;
            }
            
            context.HttpContext.Items.Add("category", category);
            await next();
        }
    }
}