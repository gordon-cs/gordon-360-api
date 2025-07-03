using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Gordon360.Services;
using System.Linq;




public class PosterCleanupService : BackgroundService
{
    //private readonly ILogger<PosterCleanupService> _logger;
    private int _executionCount;

    private readonly IServiceProvider _services;

    public PosterCleanupService(IServiceProvider services)
    {
        _services = services;
    }

    // public PosterCleanupService(ILogger<PosterCleanupService> logger)
    // {
    //     _logger = logger;
    // }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // _logger.LogInformation("Poster Cleanup Service running.");

        // When the timer should have no due-time, then do the work once now.
        await PosterCleanup();

        using PeriodicTimer timer = new(TimeSpan.FromDays(1));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await PosterCleanup();
            }
        }
        catch (OperationCanceledException)
        {
            // _logger.LogInformation("Poster Cleanup Service is stopping.");
        }
    }

    private async Task PosterCleanup()
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
    }
}
