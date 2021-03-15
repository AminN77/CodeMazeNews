using System.Collections.Generic;
using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CodeMazeSampleProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Test : ControllerBase
    {
        private ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        public Test(ILoggerManager logger, IRepositoryManager repository)
        {
            _logger = logger;
            _repository = repository;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInfo("Info");
            _logger.LogError("Error");
            _logger.LogDebug("Debug");
            _logger.LogWarn("Warn");

            return Ok();
        }
    }
}