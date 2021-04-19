using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CodeMazeSampleProject.ActionFilters;
using CodeMazeSampleProject.Utilities;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodeMazeSampleProject.Controllers
{
    [Route("api/categories/{categoryId}/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IDataShaper<NewsDto> _dataShaper;
        private readonly NewsLinks _newsLinks;

        public NewsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IDataShaper<NewsDto> dataShaper, NewsLinks newsLinks)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _dataShaper = dataShaper;
            _newsLinks = newsLinks;
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetAllNewsForCategory(Guid categoryId, [FromQuery] NewsParameters newsParameters)
        {
            var category = await _repository.Category.GetCategoryAsync(categoryId, trackChanges: false);
            if (category is null)
            {
                _logger.LogInfo($"Category with id:{categoryId} doesn't exist in the database");
                return NotFound();
            }

            var newsFromDb = await _repository.News.GetNewsAsync(categoryId, 
               newsParameters , trackChanges: false);
            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(newsFromDb.MetaData));
            var newsDto = _mapper.Map<IEnumerable<NewsDto>>(newsFromDb);
            var links = _newsLinks.TryGenerateLinks(newsDto, newsParameters.Fields, categoryId, HttpContext);
            return links.HasLinks ? Ok(links.LinkedEntities) : Ok(links.ShapedEntities);
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
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateNewsForCategory(Guid categoryId, [FromBody] NewsForCreationDto newsForCreationDto)
        {
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
        [ServiceFilter(typeof(ValidateNewsForCategoryExistAttribute))]
        public async Task<IActionResult> DeleteNewsForCategory(Guid categoryId, Guid id)
        {
           var newsForCategory = HttpContext.Items["news"] as News;
            _repository.News.DeleteNews(newsForCategory);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateNewsForCategoryExistAttribute))]
        public async Task<IActionResult> UpdateNewsForCategory(Guid categoryId, Guid id,
            [FromBody] NewsForUpdateDto newsForUpdateDto)
        {
            var news = HttpContext.Items["news"] as News;
            _mapper.Map(newsForUpdateDto, news);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValidateNewsForCategoryExistAttribute))]
        public async Task<IActionResult> PartiallyUpdateNewsForCategory(Guid categoryId, Guid id,
            [FromBody] JsonPatchDocument<NewsForUpdateDto> patchDocument)
        {
            if (patchDocument is null)
            {
                _logger.LogError("patchDocument object sent from client is null");
                return BadRequest("patchDocument object is null");
            }

            var news = HttpContext.Items["news"] as News;
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