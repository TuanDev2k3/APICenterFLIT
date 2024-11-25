using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICenterFlit.Models
{
    public class ImageDTO
    {
        public int Id { get; set; }

        public string Description { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        //public string? ImageList { get; set; }

    }
}