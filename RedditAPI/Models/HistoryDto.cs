using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditAPI.Models
{
    public class HistoryDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime DrawDate { get; set; }
    }
}
