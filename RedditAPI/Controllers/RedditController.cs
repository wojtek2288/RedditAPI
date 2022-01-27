using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RedditAPI.Models;
using RedditAPI.Services;
using RestSharp;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RedditAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedditController : ControllerBase
    {
        private readonly IRedditService _redditService;
        private readonly IMapper _mapper;

        public RedditController(IRedditService redditService, IMapper mapper)
        {
            _redditService = redditService;
            _mapper = mapper;
        }

        [HttpGet("random")]
        [SwaggerResponse(200, Type = typeof(ImageDto))]
        [SwaggerResponse(404, Type = typeof(string))]
        public async Task<ActionResult<ImageDto>> GetImage()
        {
            var image = await _redditService.GetImage(25, "top");
            var imageDto = _mapper.Map<ImageDto>(image);

            return Ok(imageDto);
        }

        [HttpGet("history")]
        [SwaggerResponse(200, Type = typeof(IEnumerable<HistoryDto>))]
        public ActionResult<IEnumerable<HistoryDto>> GetHistory()
        {
            var history = _redditService.GetHistory();
            var historyDtos = _mapper.Map<IEnumerable<HistoryDto>>(history);

            return Ok(historyDtos);
        }
    }
}
