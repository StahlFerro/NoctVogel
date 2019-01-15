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
            string jsonstring = File.ReadAllText("credentials.json");
            var cr = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonstring);
            Auth.SetUserCredentials(cr["consumerKey"], cr["consumerSecret"], cr["accessToken"], cr["accessSecret"]);
            var noct = User.GetAuthenticatedUser();
            var timeline = Timeline.GetHomeTimeline();
            var creator = User.GetUserFromScreenName("StahlFerroMLG");
            Console.WriteLine(noct.ScreenName);
            Console.WriteLine(creator.ScreenName);
        }
    }
}
