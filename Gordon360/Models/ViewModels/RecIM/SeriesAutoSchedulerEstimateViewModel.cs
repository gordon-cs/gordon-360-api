using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;
using Gordon360.Extensions.System;


namespace Gordon360.Models.ViewModels.RecIM
{
    public class SeriesAutoSchedulerEstimateViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime EndDate { get; set; }
        public int GamesCreated { get; set; }

    }
}