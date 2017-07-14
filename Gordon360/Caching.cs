using System.Web;
using System.Diagnostics;
using System;
using System.Net.Mail;
using System.Web.Caching;
using System.Net;
using Gordon360.Static.Data;
using Gordon360.Static.Methods;
using Gordon360.Static.Names;

/// <summary>
/// Caching task method created using the article written by Omar Al Zabir
/// Article: https://www.codeproject.com/Articles/12117/Simulate-a-Windows-Service-using-ASP-NET-to-run-sc
/// </summary>
namespace Gordon360
{
    public class StoredJobs
    {
        // Create a new dummy cache entry. We don't want to store anything here, because it will be gone on restart of application
        // Thus, all we need is the frequent callback from this item
        private const string DummyCacheItemKey = "DeloresMichaelLindsay";

        // Create a dummy URL that never works
        private const string DummyPageURL = "http://localhost/TestCacheTimeout/WebForm1.aspx";

        // On the application start, register the cache entry. Pretty straightforward.
        protected void Application_Start(Object sender, EventArgs e)
        {
            RegisterCacheEntry();
        }


        // Register the entry in the cache
        private bool RegisterCacheEntry()
        {
            // Check and see if the dummy entry is already in the cache
            if (null != HttpRuntime.Cache[DummyCacheItemKey])
            {
                return false;
            }

            // Otherwise, we add it to the cache
            HttpRuntime.Cache.Add(DummyCacheItemKey, "Test", null,
                DateTime.MaxValue, TimeSpan.FromMinutes(1),
                CacheItemPriority.Normal,
                new CacheItemRemovedCallback(CacheItemRemovedCallback));
            return true;
        }


        // Perform service
        private void DoWork()
        {
            Debug.WriteLine("DoWork(): " + DateTime.Now.ToString());
        }

        // Inside the callback we do all the service work
        public void CacheItemRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            Debug.WriteLine("Cache item callback: " + DateTime.Now.ToString());
            DoWork();
            RegisterCacheEntry();
        }



    }
}
