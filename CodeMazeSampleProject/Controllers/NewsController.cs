using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CodeMazeSampleProject.Controllers
{
    [Route("api/categories/{categoryId}/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public NewsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsForCategory(Guid categoryId)
        {
            var category = await _repository.Category.GetCategoryAsync(categoryId, trackChanges: false);
            if (category is null)
            {
                _logger.LogInfo($"Category with id:{categoryId} doesn't exist in the database");
                return NotFound();
            }

            var newsFromDb = await _repository.News.GetNewsAsync(categoryId, trackChanges: false);
            var newsDto = _mapper.Map<IEnumerable<NewsDto>>(newsFromDb);
            return Ok(newsDto);
        }

        [HttpGet("{id}", Name = "GetNewsForCategory")]
        public async Task<IActionResult> GetNewsForCategory(Guid categoryId, Guid id)
        {
            var category = await _repository.Category.GetCategoryAsync(categoryId, trackChanges: false);
            if (category is null)
            {
                _logger.LogInfo($"Category with id:{categoryId} doesn't exist in the database");
                return NotFound();
            }

            var newsFromDb = await _repository.News.GetNewsAsync(categoryId, id, trackChanges: false);
            if (newsFromDb is null)
            {
                _logger.LogInfo($"News with id:{id} doesn't exist in the database");
                return NotFound();
            }

            var news = _mapper.Map<NewsDto>(newsFromDb);
            return Ok(news);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewsForCategory(Guid categoryId, [FromBody] NewsForCreationDto newsForCreationDto)
        {
            if (newsForCreationDto is null)
            {
                _logger.LogError("newsForCreationDto object sent from client is null");
                return BadRequest("newsForCreationDto object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the NewsForCreationDto object");
                return UnprocessableEntity(ModelState);
            }

            var category = await _repository.Category.GetCategoryAsync(categoryId, trackChanges: false);
            if (category is null)
            {
                _logger.LogInfo($"Category with id:{categoryId} doesn't exist in the database");
                return NotFound();
            }

            var news = _mapper.Map<News>(newsForCreationDto);
            _repository.News.CreateNewsForCategory(categoryId, news);
            await _repository.SaveAsync();

            var newsToReturn = _mapper.Map<NewsDto>(news);
            return CreatedAtRoute("GetNewsForCategory", new {categoryId, id = newsToReturn}, newsToReturn);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsForCategory(Guid categoryId, Guid id)
        {
            var category = await _repository.Category.GetCategoryAsync(categoryId, trackChanges: false);
            if (category is null)
            {
                _logger.LogInfo($"Category with id:{categoryId} doesn't exist in the database");
                return NotFound();
            }

            var newsForCategory = await _repository.News.GetNewsAsync(categoryId, id, trackChanges: false);
            if (newsForCategory is null)
            {
                _logger.LogError($"News with id:{id} doesn't exist in the database");
                return NotFound();
            }
            
            _repository.News.DeleteNews(newsForCategory);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNewsForCategory(Guid categoryId, Guid id,
            [FromBody] NewsForUpdateDto newsForUpdateDto)
        {
            if (newsForUpdateDto is null)
            {
                _logger.LogError("NewsForUpdateDto object sent from client is null");
                return BadRequest("NewsForUpdateDto object sent from client is null");
            }
            
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the NewsForUpdateDto object");
                return UnprocessableEntity(ModelState);
            }

            var category = await _repository.Category.GetCategoryAsync(categoryId, trackChanges: false);
            if (category is null)
            {
                _logger.LogInfo($"Category with id:{categoryId} doesn't exist in database");
                return NotFound();
            }

            var news = await _repository.News.GetNewsAsync(categoryId, id, trackChanges: true);
            if (news is null)
            {
                _logger.LogInfo($"News with id: {id} doesn't exist in the database");
                return NotFound();
            }

            _mapper.Map(newsForUpdateDto, news);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateNewsForCategory(Guid categoryId, Guid id,
            [FromBody] JsonPatchDocument<NewsForUpdateDto> patchDocument)
        {
            if (patchDocument is null)
            {
                _logger.LogError("patchDocument object sent from client is null");
                return BadRequest("patchDocument object is null");
            }

            var category = await _repository.Category.GetCategoryAsync(categoryId, trackChanges: false);
            if (category is null)
            {
                _logger.LogInfo($"Category with id:{categoryId} doesn't exist in the database");
                return NotFound();
            }

            var news = await _repository.News.GetNewsAsync(categoryId, id, trackChanges: true);
            if (news is null)
            {
                _logger.LogInfo($"News with id:{id} doesn't exist in the database");
                return NotFound();
            }

            var newsToPatch = _mapper.Map<NewsForUpdateDto>(news);
            patchDocument.ApplyTo(newsToPatch, ModelState);
            TryValidateModel(newsToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }
            
            _mapper.Map(newsToPatch, news);
            await _repository.SaveAsync();
            return NoContent();
        }
    }
}