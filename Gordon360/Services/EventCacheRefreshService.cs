using Gordon360.Models.ViewModels;
using Gordon360.Static_Classes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Gordon360.Services;

public sealed class EventCacheRefreshService(IServiceScopeFactory scopeFactory) : IHostedService, IDisposable
{
    private Timer? _timer = null;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(UpdateEventsCacheAsync, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(10));

        return Task.CompletedTask;
    }

    private async void UpdateEventsCacheAsync(object? state)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();

            var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
            var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

            var events = await eventService.FetchEventsAsync();
            cache.Set(CacheKeys.Events, events);

        }
        catch (Exception ex)
        {
            Debug.WriteLine("UpdateEventsCacheAsync error: " + ex.Message);
        }
    }


    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
