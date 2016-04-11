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
using WindowsAzure.Messaging;

namespace covoiturage_univ
{
    [Activity(Label = "Covoiturage-univ", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public static MainActivity instance;

        private string HubEndpoint = null;
        private string HubSasKeyName = null;
        private string HubSasKeyValue = null;

        private void RegisterWithGCM()
        {
            // Check to ensure everything's set up right
            GcmClient.CheckDevice(this);
            GcmClient.CheckManifest(this);

            // Register for push notifications
            Log.Info("MainActivity", "Registering...");
            GcmClient.Register(this, notification.Constants.SenderID);
        }

        private void ParseConnectionString(string connectionString)
        {
            string[] parts = connectionString.Split(';');
            if (parts.Length != 3)
                throw new Exception("Error parsing connection string: "
                        + connectionString);

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("Endpoint"))
                {
                    this.HubEndpoint = "https" + parts[i].Substring(11);
                }
                else if (parts[i].StartsWith("SharedAccessKeyName"))
                {
                    this.HubSasKeyName = parts[i].Substring(20);
                }
                else if (parts[i].StartsWith("SharedAccessKey"))
                {
                    this.HubSasKeyValue = parts[i].Substring(16);
                }
            }
        }

        private String generateSasToken(String uri)
        {

            String targetUri;
            try
            {
                targetUri = URLEncoder
                        .Encode(uri.ToString().ToLower(), "UTF-8")
                        .ToLower();

                long expiresOnDate = DateTime.Now.Ticks;
                int expiresInMins = 60; // 1 hour
                expiresOnDate += expiresInMins * 60 * 1000;
                long expires = expiresOnDate / 1000;
                String toSign = targetUri + "\n" + expires;

                // Get an hmac_sha1 key from the raw key bytes
                byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(HubSasKeyValue);
                SecretKeySpec signingKey = new SecretKeySpec(keyBytes, "HmacSHA256");

                // Get an hmac_sha1 Mac instance and initialize with the signing key
                Mac mac = Mac.GetInstance("HmacSHA256");
                mac.Init(signingKey);

                // Compute the hmac on input data bytes
                byte[] rawHmac = mac.DoFinal(System.Text.Encoding.UTF8.GetBytes(toSign));

                // Using android.util.Base64 for Android Studio instead of
                // Apache commons codec
                String signature = URLEncoder.Encode(
                        Base64.EncodeToString(rawHmac, Base64Flags.NoWrap).ToString(), "UTF-8");

                // Construct authorization string
                String token = "SharedAccessSignature sr=" + targetUri + "&sig="
                        + signature + "&se=" + expires + "&skn=" + HubSasKeyName;
                return token;
            }
            catch (Exception e)
            {
                Log.Info("Token", e.Message);
            }

            return null;
        }

        public async void SendNotification()
        {
            Log.Info("SendNotification","SendNotification");
            EditText notificationText = FindViewById<EditText>(Resource.Id.editText1);
            string json = "{\"data\":{\"message\":\"" + notificationText.Text + "\"}}";

            ParseConnectionString(notification.Constants.FullConectionString);
            var request = HttpWebRequest.Create("http://covoiturage-univ.azurewebsites.net/notification");
            request.ContentType = "application/json";
            request.Method = "POST";
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(json);
            }

           /* HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
                Log.Info("SendNotification", response.StatusCode.ToString());*/
            
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

           
            RegisterWithGCM();
        }

        protected override void OnStop()
        {
            base.OnStop();

            //notification.PushHandlerService.Hub.UnregisterAll(notification.PushHandlerService.RegistrationID);
        }

    }
}

