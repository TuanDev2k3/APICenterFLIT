namespace APICenterFlit.Models
{
	public class AccountDTO
	{
		public int Id { get; set; }

		public string Fullname { get; set; } = null!;

		public string UserName { get; set; } = null!;

		public string Password { get; set; } = null!;

		public string Email { get; set; } = null!;

		public int AccountType { get; set; }

		public string? AccountCode { get; set; }

		public string? AccountTypeName { get; set; }

		public string? Phone { get; set; }

		public int AddressId { get; set; }

		public int ImageId { get; set; }

		public string? ImageUrl { get; set; }

		public int Status { get; set; }
	}
}
