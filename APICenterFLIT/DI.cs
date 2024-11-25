using APICenterFlit.Repositories.Portal;
using APICenterFlit.Repositories.Users;

namespace APICenterFlit
{
	public class DI
	{
		public static void DI_Portal(IServiceCollection services)
		{
			services.AddScoped<INewsService, NewsService>();
			services.AddScoped<INewsTypeService, NewsTypeService>();
			services.AddScoped<ICommentService, CommentService>();
			services.AddScoped<IImageService, ImageService>();
			services.AddHostedService<PublishAutoService>();
		}

		public static void DI_Users(IServiceCollection services)
		{
			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped<IAccountTypeService, AccountTypeService>();
			services.AddScoped<IAccessLogService, AccessLogService>();
		}
	}
}
