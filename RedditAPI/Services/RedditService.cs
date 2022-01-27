using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RedditAPI.Exceptions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditAPI.Services
{
    public interface IRedditService
    {
        void GetHistory();
        Task<string> GetImage(int limit, string sort);
    }

    public class RedditService : IRedditService
    {
        private readonly IConfiguration _configuration;
        public RedditService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> GetImage(int limit, string sort)
        {
            string Subreddit = _configuration["Subreddit"];
            List<int> Numbers = new List<int>();
            InitList(Numbers, limit);
            string Image = "";

            RestClient Client = new RestClient("http://www.reddit.com/r/" + Subreddit + ".json");
            RestRequest Request = new RestRequest();

            Request.AddParameter("limit", limit.ToString());
            Request.AddParameter("sort", sort);
            Request.AddParameter("raw_json", 1);

            var response = await Client.ExecuteAsync(Request);

            if (!response.IsSuccessful) throw new InternalServerErrorException("Couldn't fetch reddit API");

            dynamic body = JsonConvert.DeserializeObject(response.Content);

            for(int i = 1; i <= limit; i++)
            {
                int selectedIdx = SelectIdx(Numbers);

                if (body.data.children[selectedIdx].data.preview is not null)
                    Image = body.data.children[selectedIdx].data.preview.images[0].source.url;
            }

            if (Image is null) throw new NotFoundException("No post with an image found within specified limit");

            return Image;
        }

        public void GetHistory()
        {

        }

        private void InitList(List<int> Numbers, int limit)
        {
            for(int i = 1; i <= limit; i++)
            {
                Numbers.Add(i);
            }
        }

        private int SelectIdx(List<int> Numbers)
        {
            Random rnd = new Random();
            int idx = rnd.Next(0, Numbers.Count);
            int res = Numbers[idx];

            Numbers.RemoveAt(idx);

            return res;
        }
    }
}
