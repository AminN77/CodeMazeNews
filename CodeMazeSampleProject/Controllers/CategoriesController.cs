using System;
using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CodeMazeSampleProject.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;

        public CategoriesController(ILoggerManager logger, IRepositoryManager repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            try
            {
                var categories = _repository.Category.GetAllCategories(trackChanges: false);
                return Ok(categories);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Something went wrong in the {nameof(GetCategories)} action {exception}");
            return StatusCode(500, "Internal server error");
            }
        }
    }
}