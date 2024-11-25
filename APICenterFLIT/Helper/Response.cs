namespace APICenterFlit.Helper
{
	public class Response
	{
		public int? Result {  get; set; }
		public int Status { get; set; }
		public string? Message { get; set; }
		public dynamic? Data { get; set; }
	}
}
