using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace covoiturage_univ
{
    class Constants
    {
        public const string SenderID = "1088202902008"; // "covoiturage-univ"; // Google API Project Number
        public const string ListenConnectionString = "Endpoint=sb://covoiturage-univ.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=w23Kcl9MbtQxfg7ycA7epocJIisvGNBAmGcPFzONnys=";
        public const string NotificationHubName = "covoiturage-univ_notificationhub";
    }
}