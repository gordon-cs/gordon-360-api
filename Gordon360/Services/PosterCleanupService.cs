using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Gordon360.Services;
using System.Linq;



public class PosterCleanupService(IServiceProvider _services) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var posterService = scope.ServiceProvider.GetRequiredService<IPosterService>();

            var posters = posterService.GetPosters()
                .Where(p => p.ExpirationDate <= DateTime.Now.AddDays(-14))
                .ToList();

            foreach (var poster in posters)
            {
                await posterService.DeletePosterAsync(poster.ID);
            }

            // Wait 24 hours
            await Task.Delay(TimeSpan.FromDays(1));
        }
    }
}
