using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CodeMazeSampleProject.ActionFilters;
using CodeMazeSampleProject.ModelBinders;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMazeSampleProject.Controllers
{
    [ApiVersion("1.0")]
    // For Deprecated versions : [ApiVersion("1.0", Deprecated = true)]
    // For URI versioning : [Route("api/{v:apiversion}/categories")]
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CategoriesController(ILoggerManager logger, IRepositoryManager repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all categories
        /// </summary>
        /// <returns>The categories list</returns>
        [HttpGet(Name = "GetCategories"), Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _repository.Category.GetAllCategoriesAsync(trackChanges: false);
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(categoriesDto);
        }

        [HttpGet("{id}", Name = "CategoryById")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            var category = await _repository.Category.GetCategoryAsync(id, trackChanges: false);
            if (category is null)
            {
              _logger.LogInfo($"Category with id:{id} doesn't exist in the database");
              return NotFound();
            }
            
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="categoryForCreationDto"></param>
        /// <returns code="201">Returns the newly created item</returns>
        /// <returns code="400">If the item is null</returns>
        /// <returns code="422">If the model is invalid</returns>
        [HttpPost(Name = "CreateCategory")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryForCreationDto categoryForCreationDto)
        {
            var category = _mapper.Map<Category>(categoryForCreationDto);
            _repository.Category.CreateCategory(category);
            await _repository.SaveAsync();
            
            var categoryToReturn = _mapper.Map<CategoryDto>(category);
            return CreatedAtRoute("CategoryById", new {id = categoryToReturn.Id}, categoryToReturn);
        }

        [HttpGet("collection/({ids})", Name = "CategoryCollection")]
        public async Task<IActionResult> GetCategoryCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids is null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }

            var categories = await _repository.Category.GetByIdsAsync(ids, trackChanges: false);
            if (ids.Count() != categories.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }

            var categoriesToReturn = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(categoriesToReturn);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCategoryCollection([FromBody] IEnumerable<CategoryForCreationDto> categoryCollection)
        {
            if (categoryCollection is null)
            {
                _logger.LogError("Category collection sent from client is null");
                return BadRequest("Category collection is null");
            }

            var categories = _mapper.Map<IEnumerable<Category>>(categoryCollection);
            foreach (var category in categories)
            {
                _repository.Category.CreateCategory(category);
            }
            
            await _repository.SaveAsync();

            var categoryCollectionToReturn = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            var ids = string.Join(",", categoryCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CategoryCollection", new {ids}, categoryCollectionToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCategoryExistsAttribute))]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = HttpContext.Items["category"] as Category;
            _repository.Category.DeleteCategory(category);
            await _repository.SaveAsync();
            return NoContent();

        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateCategoryExistsAttribute))]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryForUpdateDto categoryForUpdateDto)
        {
            var category = HttpContext.Items["category"] as Category;
            _mapper.Map(categoryForUpdateDto, category);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCategoriesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();
        }
    }
}