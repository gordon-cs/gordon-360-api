using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Gordon360.Models.CCT.Context;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

public class MarketplaceCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public MarketplaceCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CCTContext>();
                var now = DateTime.Now;
                var expired = context.PostedItem
                    .Where(x =>
                        (x.StatusId == 1 && x.PostedAt.AddDays(90) <= now) ||
                        (x.StatusId == 2 && x.PostedAt.AddDays(30) <= now) ||
                        (x.StatusId == 3 && x.PostedAt.AddDays(14) <= now)
                    )
                    .ToList();

                if (expired.Any())
                {
                    context.PostedItem.RemoveRange(expired);
                    await context.SaveChangesAsync();
                }
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Run every hour
        }
    }
}