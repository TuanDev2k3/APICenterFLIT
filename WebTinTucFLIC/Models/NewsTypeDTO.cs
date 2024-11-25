namespace WebTinTucFLIC.Models
{
    public class NewsTypeDTO
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string? ParentName { get; set; }

        public string Title { get; set; } = null!;

        public string? TitleSlug { get; set; }

        public int ImageId { get; set; }

        public string? ImageUrl { get; set; }

        public int? HomePage { get; set; }

    }
}
