using System;
using System.Collections.Generic;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
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
        public IActionResult GetNewsForCategory(Guid categoryId)
        {
            var category = _repository.Category.GetCategory(categoryId, trackChanges: false);
            if (category is null)
            {
                _logger.LogInfo($"Category with id:{categoryId} doesn't exist in the database");
                return NotFound();
            }

            var newsFromDb = _repository.News.GetNews(categoryId, trackChanges: false);
            var newsDto = _mapper.Map<IEnumerable<NewsDto>>(newsFromDb);
            return Ok(newsDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetNewsForCategory(Guid categoryId, Guid id)
        {
            var category = _repository.Category.GetCategory(categoryId, trackChanges: false);
            if (category is null)
            {
                _logger.LogInfo($"Category with id:{categoryId} doesn't exist in the database");
                return NotFound();
            }

            var newsFromDb = _repository.News.GetNews(categoryId, id, trackChanges: false);
            if (newsFromDb is null)
            {
                _logger.LogInfo($"News with id:{id} doesn't exist in the database");
                return NotFound();
            }

            var news = _mapper.Map<NewsDto>(newsFromDb);
            return Ok(news);
        }
    }
}