using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CodeMazeSampleProject.ModelBinders;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace CodeMazeSampleProject.Controllers
{
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

        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _repository.Category.GetAllCategories(trackChanges: false);
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(categoriesDto);
        }

        [HttpGet("{id}", Name = "CategoryById")]
        public IActionResult GetCategory(Guid id)
        {
            var category = _repository.Category.GetCategory(id, trackChanges: false);
            if (category is null)
            {
              _logger.LogInfo($"Category with id:{id} doesn't exist in the database");
              return NotFound();
            }
            
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpPost]
        public IActionResult CreateCategory([FromBody] CategoryForCreationDto categoryForCreationDto)
        {
            if (categoryForCreationDto is null)
            {
                _logger.LogError("categoryForCreationDto object sent from client is null");
                return BadRequest("CategoryForCreation object is null");
            }

            var category = _mapper.Map<Category>(categoryForCreationDto);
            _repository.Category.CreateCategory(category);
            _repository.Save();
            
            var categoryToReturn = _mapper.Map<CategoryDto>(category);
            return CreatedAtRoute("CategoryById", new {id = categoryToReturn.Id}, categoryToReturn);
        }

        [HttpGet("collection/({ids})", Name = "CategoryCollection")]
        public IActionResult GetCategoryCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids is null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }

            var categories = _repository.Category.GetByIds(ids, trackChanges: false);
            if (ids.Count() != categories.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }

            var categoriesToReturn = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(categoriesToReturn);
        }

        [HttpPost("collection")]
        public IActionResult CreateCategoryCollection([FromBody] IEnumerable<CategoryForCreationDto> categoryCollection)
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
            
            _repository.Save();

            var categoryCollectionToReturn = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            var ids = string.Join(",", categoryCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CategoryCollection", new {ids}, categoryCollectionToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(Guid id)
        {
            var category = _repository.Category.GetCategory(id, trackChanges: false);
            if(category == null)
            {
                _logger.LogInfo($"Category with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            
            _repository.Category.DeleteCategory(category);
            _repository.Save();
            return NoContent();

        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(Guid id, [FromBody] CategoryForUpdateDto categoryForUpdateDto)
        {
            if (categoryForUpdateDto is null)
            {
                _logger.LogError("CategoryForUpdateDto object sent from client is null");
                return BadRequest("CategoryForUpdateDto object sent from client is null");
            }

            var category = _repository.Category.GetCategory(id, trackChanges: true);
            if (category is null)
            {
                _logger.LogError($"Category with id:{id} doesn't exist in the database");
                return NotFound();
            }

            _mapper.Map(categoryForUpdateDto, category);
            _repository.Save();
            return NoContent();
        }
    }
}