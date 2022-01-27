using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RedditAPI.Entities;
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
        private readonly RedditDbContext _context;
        public RedditService(IConfiguration configuration, RedditDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<string> GetImage(int limit, string sort)
        {
            string Subreddit = _configuration["Subreddit"];
            List<int> Numbers = new List<int>();
            InitList(Numbers, limit);
            string Image = null;

            RestClient Client = new RestClient("http://www.reddit.com/r/" + Subreddit + ".json");
            RestRequest Request = new RestRequest();

            Request.AddParameter("limit", limit.ToString());
            Request.AddParameter("sort", sort);
            Request.AddParameter("t", "all");
            Request.AddParameter("raw_json", 1);

            while(true)
            {
                var response = await Client.ExecuteAsync(Request);

                if (!response.IsSuccessful) throw new InternalServerErrorException("Couldn't fetch reddit API");

                dynamic body = JsonConvert.DeserializeObject(response.Content);
                string after = body.data.after;

                if (after is null)
                {
                    throw new NotFoundException("Pics not found on this subreddit");
                }

                Request.AddParameter("after", after);

                for (int i = 0; i < limit; i++)
                {
                    int selectedIdx = SelectIdx(Numbers);
                    Console.WriteLine(selectedIdx.ToString());
                    if (body.data.children[selectedIdx].data.preview is not null)
                    {
                        Image = body.data.children[selectedIdx].data.preview.images[0].source.url;
                        break;
                    }
                }

                if (Image is not null)
                    break;

                InitList(Numbers, limit);
            }

            return Image;
        }

        public void GetHistory()
        {

        }

        private void InitList(List<int> Numbers, int limit)
        {
            for(int i = 0; i < limit; i++)
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
