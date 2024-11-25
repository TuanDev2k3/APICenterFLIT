using APICenterFlit.Entities;
using APICenterFlit.Models;
using AutoMapper;

namespace APICenterFlit.Helper
{
	public class MyAutoMapper : Profile
	{
		public MyAutoMapper()
		{
			CreateMap<News, NewsDTO>().ReverseMap();
			CreateMap<NewsType, NewsTypeDTO>().ReverseMap();
			CreateMap<Account, AccountDTO>().ReverseMap();
			CreateMap<AccountType, AccountTypeDTO>().ReverseMap();
			CreateMap<Comment, CommentDTO>().ReverseMap();
			CreateMap<Image, ImageDTO>().ReverseMap();
			CreateMap<AccessLog, AccessLogDTO>().ReverseMap();
		}
	}
}
