using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICenterFlit.Models
{
    public class CommentDTO
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public int NewId { get; set; }

        public string Detail { get; set; } = null!;

        public string? AccountName { get; set; }

        public string? ImageURL { get; set; }

        public DateTime CommentAt { get; set; }

    }
}