using Gordon360.Static_Classes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Diagnostics;
using System.Collections;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;

namespace Gordon360.Services;

public class EventCacheRefreshService : IHostedService, IDisposable
{
    private readonly IMemoryCache _cache;
    private Timer? _timer = null;

    public EventCacheRefreshService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(UpdateEventsCacheAsync, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(10));

        return Task.CompletedTask;
    }

    private async void UpdateEventsCacheAsync(object? state)
    {
        IEnumerable<EventViewModel>? events = null;
        try
        {
            events = await EventService.FetchEventsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        _cache.Set(CacheKeys.Events, events);
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
