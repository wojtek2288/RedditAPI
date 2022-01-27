using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RedditAPI.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedditController : ControllerBase
    {
        private readonly IRedditService _redditService;

        public RedditController(IRedditService redditService)
        {
            _redditService = redditService;
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetImage()
        {
            string image = await _redditService.GetImage(10, "top");

            return Ok(image);
        }

        [HttpGet("history")]
        public void GetHistory()
        {

        }
    }
}
