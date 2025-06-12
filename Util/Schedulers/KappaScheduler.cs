using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Util.Schedulers
{
    public class KappaScheduler : BackgroundService
    {
        private readonly IServiceProvider _services;

        public KappaScheduler(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = GetNextMidnightMonday(now);
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
                var service = scope.ServiceProvider.GetRequiredService<IEloCalculatorService>();

                try
                {
                    await service.CalculateKappa(stoppingToken);
                }
                catch (Exception)
                {
                }
            }
        }

        private static DateTime GetNextMidnightMonday(DateTime from)
        {
            var daysUntilMonday = ((int)DayOfWeek.Monday - (int)from.DayOfWeek + 7) % 7;
            var nextMonday = from.Date.AddDays(daysUntilMonday);

            if (daysUntilMonday == 0 && from.TimeOfDay < TimeSpan.FromHours(24))
                return from.Date.AddDays(1);

            return nextMonday;
        }
    }
}
