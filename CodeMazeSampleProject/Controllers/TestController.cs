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
        
        public Test(ILoggerManager logger)
        {
            _logger = logger;
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