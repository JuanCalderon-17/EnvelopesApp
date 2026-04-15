using cdpTracker_Api.Data;
using Microsoft.EntityFrameworkCore;

namespace cdpTracker_Api.Services
{
    public class CleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CleanupService> _logger;

        public CleanupService(IServiceScopeFactory scopeFactory, ILogger<CleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Wait until next UTC midnight before running
                var now = DateTime.UtcNow;
                var nextMidnight = now.Date.AddDays(1);
                var delay = nextMidnight - now;
                await Task.Delay(delay, stoppingToken);

                if (stoppingToken.IsCancellationRequested) break;

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var cutoff = DateTime.UtcNow.AddMonths(-3);
                var old = db.Envelopes.Where(e => e.RecordedAt < cutoff);
                db.Envelopes.RemoveRange(old);
                var deleted = await db.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("CleanupService: deleted {Count} envelopes older than 3 months.", deleted);
            }
        }
    }
}
