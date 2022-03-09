using System;

namespace Gordon360.Models.ViewModels
{
    public class PushNotificationViewModel
    {
        public String to { get; set; }

        public NotificationDataViewModel data { get; set; }

        public String title { get; set; }

        public String body { get; set; }

        public String sound = "default";

    }
}