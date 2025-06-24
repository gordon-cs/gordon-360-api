using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
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
                    (x.StatusId == 1 && x.PostedAt.AddDays(90) <= now) ||
                    (x.StatusId == 2 && x.PostedAt.AddDays(30) <= now) ||
                    (x.StatusId == 3 && x.PostedAt.AddDays(14) <= now)
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