using Gordon360.Models.Gordon360.Context;
using Gordon360.Exceptions;
using Gordon360.Models.ViewModels;
using Gordon360.Static_Classes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Gordon360.Models.Gordon360;

// <summary>
// We use this service to pull data from 25Live as well as parsing it
// The data is retrieved from the cache maintained by Startup.cs
// </summary>
namespace Gordon360.Services;

/// <summary>
/// Service that allows for event control
/// </summary>
public class EventService(Gordon360Context context, IMemoryCache cache, IAccountService accountService) : IEventService
{

    /**
     * URL to retrieve events from the 25Live API. 
     * event_type_id parameter fetches only events of type 14 (Calendar Announcement) and 57 (Event).
     * All other event types are not appropiate for the 360 events feed.
     * end_after parameter  limits the request to events from the current academic year.
     * state parameter fetches only confirmed events
     */
    private static readonly string AllEventsURL = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&event_type_id=14+57&state=2&end_after=" + GetFirstEventDate() + "&scope=extended";
    
    private IEnumerable<EventViewModel> Events => cache.Get<IEnumerable<EventViewModel>>(CacheKeys.Events);

    /// <summary>
    /// Access the memory stream created by the cached task and parse it into events
    /// Splits events with multiple repeated occurrences into individual records for each occurrence
    /// </summary>
    /// <returns>All events for the current academic year.</returns>
    public IEnumerable<EventViewModel> GetAllEvents()
    {
        return Events;
    }

    /// <summary>
    /// Select only events that are marked for Public promotion
    /// </summary>
    /// <returns>All Public Events</returns>
    public IEnumerable<EventViewModel> GetPublicEvents()
    {
        return Events.Where(e => e.IsPublic);
    }

    /// <summary>
    /// Select only events that are Approved to give CLAW credit
    /// </summary>
    /// <returns>All CLAW Events</returns>
    public IEnumerable<EventViewModel> GetCLAWEvents()
    {
        return Events.Where(e => e.HasCLAWCredit);
    }

    /// <summary>
    /// Returns all attended events for a student in a specific term
    /// </summary>
    /// <param name="username"> The student's AD Username</param>
    /// <param name="term"> The current term</param>
    /// <returns></returns>
    public IEnumerable<AttendedEventViewModel> GetEventsForStudentByTerm(string username, string term)
    {
        var account = accountService.GetAccountByUsername(username);

        var result = context.ChapelEvent
                             .Where(c => c.CHBarcode == account.Barcode && c.CHTermCD == term)
                             .Select<ChapelEvent, ChapelEventViewModel>(c => c);

        if (result is not IEnumerable<ChapelEventViewModel> chapelEvents)
        {
            return Enumerable.Empty<AttendedEventViewModel>();
        }

        // Left join to 25Live Events for extra event data when matching 25Live event is found
        var attendedEvents = from chapelEvent in chapelEvents
                       join event25Live in Events on chapelEvent.LiveID equals event25Live.Event_ID?.Split('_')?.FirstOrDefault() into liveEvents
                       from liveEvent in liveEvents.DefaultIfEmpty()
                       select new AttendedEventViewModel(liveEvent, chapelEvent);

        return attendedEvents;
    }

    public static async Task<IEnumerable<EventViewModel>> FetchEventsAsync()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

        var response = await client.GetAsync(AllEventsURL);
        if (response != null && response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
            var eventsXML = XDocument.Parse(content);
            return eventsXML
                .Descendants(EventViewModel.r25 + "event")
                .SelectMany(
                    // Select occurrences of each events
                    elem => elem.Element(EventViewModel.r25 + "profile")?.Descendants(EventViewModel.r25 + "reservation"),
                    // Map the event e with it's occurrence details o to a new EventViewModel
                    (e, o) => new EventViewModel(e, o)
                );
            }
            catch (Exception)
            {

                throw;
            }
        }
        else
        {
            throw new ResourceNotFoundException();
        }
    }

    /// <summary>
    ///  Helper function to determine the current academic year
    /// </summary>
    /// <returns></returns>
    private static string GetFirstEventDate()
    {
        //Beginning date of fall semester (MM/DD)
        var fallDate = "0815";
        //Beginning date of summer (MM/DD)
        var summerDate = "0515";

        // We need to determine what the current date is
        DateTime today = DateTime.Today;
        if (today.Month < 06)
        {
            return (today.Year - 1).ToString() + fallDate;
        }
        else if (today.Month > 07)
        {
            return today.Year.ToString() + fallDate;
        }
        return today.Year.ToString() + summerDate;
    }
}
