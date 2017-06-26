using System;
using System.Net;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;

namespace Gordon360.Models.ViewModels
{
    public class EventViewModel
    {
        public int ROWID { get; set; }
        public string CHBarEventID { get; set; }
        public string CHBarcode { get; set; }
        public string CHEventID { get; set; }
        public string CHCheckerID { get; set; }
        public DateTime CHDate { get; set; }
        public DateTime CHTime { get; set; }
        public string CHTermCD { get; set; }

        public static implicit operator EventViewModel(string EventID)
        {
            var requestUrl = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&otransform=json.xsl&event_id=" + EventID + "&scope=extended";

            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    var json = new WebClient().DownloadString(requestUrl);

                    JObject joResponse = JObject.Parse(json);
                    JObject ojObject = (JObject)joResponse["response"];
                  
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            return requestUrl;
        }
    }


}