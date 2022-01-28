using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RedditAPI.Entities;
using RedditAPI.Exceptions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedditAPI.Services
{
    public interface IRedditService
    {
        IEnumerable<History> GetHistory();
        Task<string> GetImage(int limit, string sort);
    }

    public class RedditService : IRedditService
    {
        private readonly IConfiguration _configuration;
        private readonly RedditDbContext _context;
        private readonly HashSet<string> sortingOptions;
        public RedditService(IConfiguration configuration, RedditDbContext context)
        {
            _configuration = configuration;
            _context = context;
            sortingOptions = new HashSet<string>() { "relevance", "hot", "top", "new" };
        }

        /// <summary>
        /// Gets an url of random image from subreddit specified in configuration
        /// </summary>
        /// <param name="limit">Number of posts per request (from 1 to 100)</param>
        /// <param name="sort">Post sorting method. Possible values = {relevance, hot, top, new, comments}</param>
        /// <returns>Url of random image</returns>
        public async Task<string> GetImage(int limit, string sort)
        {
            if (limit < 1 || limit > 100 || !sortingOptions.Contains(sort)) 
                throw new BadRequestException("Limit must be between 1 and 100 and sort must be one of: relevance, hot, top, new");

            string subreddit = _configuration["Subreddit"];
            List<int> numbers = new List<int>();
            InitList(numbers, limit);
            string image = null;

            RestClient Client = new RestClient("http://www.reddit.com/r/" + subreddit + ".json");
            RestRequest Request = new RestRequest();

            Request.AddParameter("limit", limit.ToString());
            Request.AddParameter("sort", sort);
            Request.AddParameter("t", "all");
            Request.AddParameter("raw_json", 1);

            //Make requests until finding post with an image or exceed the limit of listing (around 1000 posts)
            while (true)
            {
                var response = await Client.ExecuteAsync(Request);

                if (!response.IsSuccessful) throw new InternalServerErrorException("Couldn't fetch reddit API");

                dynamic body = JsonConvert.DeserializeObject(response.Content);

                //Get after element for next pagination result
                string after = body.data.after;

                if (after is null)
                {
                    throw new NotFoundException("Pics not found on this subreddit");
                }

                Request.AddParameter("after", after);

                //Foreach post per request
                for (int i = 0; i < limit; i++)
                {
                    //Get random index
                    int selectedIdx = SelectIdx(numbers);

                    //Break when first image was found
                    if (body.data.children[selectedIdx].data.preview is not null)
                    {
                        image = body.data.children[selectedIdx].data.preview.images[0].source.url;
                        break;
                    }
                }

                if (image is not null)
                    break;

                InitList(numbers, limit);
            }

            History hist = new History()
            {
                Url = image,
                DrawDate = DateTime.Now
            };

            _context.DrawHistory.Add(hist);
            if(_context.SaveChanges() != 1) throw new InternalServerErrorException("Couldn't save to Database");
            
            return image;
        }

        /// <summary>
        /// Gets all history items from Database
        /// </summary>
        /// <returns>History items</returns>
        public IEnumerable<History> GetHistory()
        {
            foreach(var historyItem in _context.DrawHistory)
            {
                yield return historyItem;
            }
        }

        /// <summary>
        /// Initializes List with values from 0 to limit
        /// </summary>
        /// <param name="numbers">List to be initialized</param>
        /// <param name="limit">Upper limit of initialization</param>
        private void InitList(List<int> numbers, int limit)
        {
            for(int i = 0; i < limit; i++)
            {
                numbers.Add(i);
            }
        }

        /// <summary>
        /// Draws an index and removes element at this index from List
        /// </summary>
        /// <param name="Numbers">List of possible numbers</param>
        /// <returns>Index of the post drawn</returns>
        private int SelectIdx(List<int> numbers)
        {
            Random rnd = new Random();
            int idx = rnd.Next(0, numbers.Count);
            int res = numbers[idx];

            numbers.RemoveAt(idx);

            return res;
        }
    }
}
