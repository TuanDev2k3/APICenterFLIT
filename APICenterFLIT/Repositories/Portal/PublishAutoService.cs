
using APICenterFlit.Entities;
using System;

namespace APICenterFlit.Repositories.Portal
{
    public class PublishAutoService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public PublishAutoService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _db = scope.ServiceProvider.GetRequiredService<DbnewsContext>();

                    var today = DateTime.Now;
                    var articlesToPublish = _db.News
                        .Where(a => a.Status == 3 && a.PublishAt <= today)
                        .ToList();

                    foreach (var article in articlesToPublish)
                    {
                        article.Status = 1;
                        _db.Update(article);
                    }

                    await _db.SaveChangesAsync(); 
                }

                // Chờ một khoảng thời gian trước khi chạy lại
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
