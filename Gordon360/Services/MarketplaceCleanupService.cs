using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Gordon360.Models.CCT.Context;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class MarketplaceCleanupService(IServiceProvider services) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromDays(1)); // Run every 24 hours

        try
        {
            do
            {
                await DoCleanupAsync(stoppingToken);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
            // Service is stopping
        }
    }

    private async Task DoCleanupAsync(CancellationToken stoppingToken)
    {
        try
        {
            using (var scope = services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CCTContext>();
                var now = DateTime.Now;
                // Find items whose images should be deleted (14 days after DeletedAt)
                var expired = await context.PostedItem
                    .Where(x => x.DeletedAt != null && x.DeletedAt.Value.AddDays(14) <= now)
                    .Include(x => x.PostImage)
                    .ToListAsync(stoppingToken);

                foreach (var item in expired)
                {
                    var images = context.PostImage.Where(img => img.PostedItemId == item.Id).ToList();
                    context.PostImage.RemoveRange(images);
                }

                var deleted = await context.PostedItem
                    .Where(x => x.DeletedAt != null && x.DeletedAt.Value <= now)
                    .ToListAsync(stoppingToken);

                foreach (var item in deleted)
                {
                    item.StatusId = 3;
                }

                if (expired.Any())
                {
                    await context.SaveChangesAsync(stoppingToken);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MarketplaceCleanupService error: {ex}");
        }
    }
}