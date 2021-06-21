using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public class RideViewModel
    {
        public string rideID { get; set; }
        public string destination { get; set; } //should this really be a string? or an int?
        public string meetingPoint { get; set; }
        public Nullable<System.DateTime> startTime { get; set; }
        public Nullable<System.DateTime> endTime { get; set; }
        public int capacity { get; set; }
        public string notes { get; set; }
        public byte isCanceled { get; set; }


        public static implicit operator RideViewModel(Save_Rides ride)
        {
            RideViewModel vm = new RideViewModel
            {
                rideID = ride.rideID,
                destination = ride.destination,
                meetingPoint = ride.meetingPoint,
                startTime = ride.startTime,
                endTime = ride.endTime,
                capacity = ride.capacity,
                notes = ride.notes ?? "",
                isCanceled = ride.canceled,

            };

            return vm;
        }
    }
}
