namespace APICenterFlit.Models
{
    public class AccessLogDTO
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string? AccountName { get; set; }

        public string Description { get; set; } = null!;

        public string DeviceName { get; set; } = null!;

        public string BrowersName { get; set; } = null!;

        public DateTime Timer { get; set; }

        public string IpAddress { get; set; } = null!;
    }
}
