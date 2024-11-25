namespace WebTinTucFLIC.Models
{
    public class Comment
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
