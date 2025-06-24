using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Gordon360.Services;
using System.Linq;

public class MarketplaceCleanupService : BackgroundService
{
    private readonly CCTContext _context;

    public MarketplaceCleanupService(CCTContext context)
    {
        _context = context;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var expired = _context.PostedItem
                .Where(x =>
                    (x.StatusID == 1 && x.PostedAt.AddDays(90) <= now) ||
                    (x.StatusID == 2 && x.PostedAt.AddDays(30) <= now) ||
                    (x.StatusID == 3 && x.PostedAt.AddDays(14) <= now)
                )
                .ToList();

            if (expired.Any())
            {
                _context.PostedItem.RemoveRange(expired);
                await _context.SaveChangesAsync();
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Run every hour
        }
    }
}