using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditAPI.Entities
{
    public class History
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime DrawDate { get; set; }
    }
}
