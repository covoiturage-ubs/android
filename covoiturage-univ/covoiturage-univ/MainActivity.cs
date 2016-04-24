using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;
using Gcm.Client;
using Java.Net;
using Javax.Crypto;
using Javax.Crypto.Spec;
using System;
using System.IO;
using System.Net;
using System.Text;
using WindowsAzure.Messaging;

namespace covoiturage_univ
{
    [Activity(Label = "Covoiturage-univ", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public static MainActivity instance;
        
        private void RegisterWithGCM()
        {
            // Check to ensure everything's set up right
            GcmClient.CheckDevice(this);
            GcmClient.CheckManifest(this);

            // Register for push notifications
            Log.Info("MainActivity", "Registering...");
            GcmClient.Register(this, notification.Constants.SenderID);
        }
        
        public void SendNotification()
        {
            Log.Info("SendNotification","SendNotification");
            EditText notificationText = FindViewById<EditText>(Resource.Id.editText1);
            string json = "{\"data\":{\"message\":\"" + notificationText.Text + "\"}}";

            notificationText.Text = "";

            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            webClient.UploadStringCompleted += (s, e) => 
            {
                Log.Info("SendNotification", "SendNotification "+ e.Result);
            };
            webClient.UploadStringAsync(new Uri("http://covoiturage-univ.azurewebsites.net/notification"), "POST", json);

            Log.Info("SendNotification", "FIN SendNotification ");
        }

       

        protected override void OnCreate(Bundle bundle)
        {
            instance = this;

            base.OnCreate(bundle);

            // Set your view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get your button from the layout resource,
            // and attach an event to it
            var button = FindViewById<Button>(Resource.Id.button1);

            button.Click += delegate {
                SendNotification();
            };

            if (!notification.PushHandlerService.running)
            {
                RegisterWithGCM();
            }
            
        }

        protected override void OnStop()
        {
            base.OnStop();
            Log.Info("OnStop", "FIN OnStop ");
        }

    }
}

