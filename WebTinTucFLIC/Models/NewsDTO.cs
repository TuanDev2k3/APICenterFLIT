using System.ComponentModel.DataAnnotations.Schema;

namespace WebTinTucFLIC.Models
{
    public class NewsDTO
    {
        public int Id { get; set; }

        public int NewTypeId { get; set; }

        public string? NewsTypeName { get; set; }

        public string Title { get; set; } = null!;

        public string? TitleSlug { get; set; }

        public string Description { get; set; } = null!;

        public string Detail { get; set; } = null!;

        public int ImageId { get; set; }

        public int? HomePage { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime PublishAt { get; set; }

        public int? IsHot { get; set; }

        public int Status { get; set; }
    }
}
