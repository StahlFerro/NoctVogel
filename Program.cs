using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Tweetinvi;

namespace NoctVogel
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting NoctVogel v0.0.1...");
            string jsonstring = File.ReadAllText("credentials.json");
            var cr = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonstring);
            Console.WriteLine("Authenticating to Twitter...");
            Auth.SetUserCredentials(cr["consumerKey"], cr["consumerSecret"], cr["accessToken"], cr["accessSecret"]);
            var noct = User.GetAuthenticatedUser();
            var timeline = Timeline.GetHomeTimeline();
            var creator = User.GetUserFromScreenName("StahlFerroMLG");
            Console.WriteLine(noct.ScreenName);
            Console.WriteLine(creator.ScreenName);
        }
    }
}
