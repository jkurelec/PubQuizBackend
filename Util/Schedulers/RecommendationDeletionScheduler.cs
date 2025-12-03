using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Util.Schedulers
{
    public class KappaAndDeviationScheduler : BackgroundService
    {
        private readonly IServiceProvider _services;

        public KappaAndDeviationScheduler(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = now.Date.AddDays(1).AddHours(3);

                if (now.Hour < 3)
                    nextRun = now.Date.AddHours(3);

                var delay = nextRun - now;

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                using var scope = _services.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IRecommendationService>();

                try
                {
                    await service.DeleteRecommendationsForPrevoiusEditions(stoppingToken);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
