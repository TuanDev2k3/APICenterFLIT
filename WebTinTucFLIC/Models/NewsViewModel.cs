using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTinTucFLIC.Models
{
    public class NewsViewModel
    {
        public List<NewsDTO> NewsList { get; set; } = new();
        public NewsDTO? SelectedNews { get; set; }
        public List<NewsDTO> RelatedNews { get; set; } = new();
    }
}