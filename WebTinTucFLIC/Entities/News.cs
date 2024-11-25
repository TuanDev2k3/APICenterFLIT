namespace WebTinTucFLIC.Entities
{
    public class News
    {
        public int Id { get; set; }

        public int NewTypeId { get; set; }

        public string? NewsTypeName { get; set; }

        public string Title { get; set; } = null!;

        public string? TitleSlug { get; set; }

        public string Description { get; set; } = null!;

        public string Detail { get; set; } = null!;

        public int ImageId { get; set; }

        public DateTime PublishAt { get; set; }

        public int IsHot { get; set; }

        public int Status { get; set; }

        public DateTime CreateAt { get; set; }

        public int CreateBy { get; set; }

        public DateTime UpdateAt { get; set; }

        public int UpdateBy { get; set; }

        public string? ImageList { get; set; }
    }
}
