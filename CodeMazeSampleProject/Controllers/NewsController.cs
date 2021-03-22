﻿using System;
using System.Collections.Generic;
using AutoMapper;
using Contracts;
using Entities;
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

        [HttpGet("{id}", Name = "GetNewsForCategory")]
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

        [HttpPost]
        public IActionResult CreateNewsForCategory(Guid categoryId, [FromBody] NewsForCreationDto newsForCreationDto)
        {
            if (newsForCreationDto is null)
            {
                _logger.LogError("newsForCreationDto object sent from client is null");
                return BadRequest("newsForCreationDto object is null");
            }

            var category = _repository.Category.GetCategory(categoryId, trackChanges: false);
            if (category is null)
            {
                _logger.LogInfo($"Category with id:{categoryId} doesn't exist in the database");
                return NotFound();
            }

            var news = _mapper.Map<News>(newsForCreationDto);
            _repository.News.CreateNewsForCategory(categoryId, news);
            _repository.Save();

            var newsToReturn = _mapper.Map<NewsDto>(news);
            return CreatedAtRoute("GetNewsForCategory", new {categoryId, id = newsToReturn}, newsToReturn);
        }
    }
}