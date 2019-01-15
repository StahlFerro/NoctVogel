using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using Tweetinvi;
using Tweetinvi.Models;
using NoctVogel.Services;

namespace NoctVogel
{
    public class Program
    {
        private DiscordSocketClient discordclient;
        private IAuthenticatedUser tweetuser;
        private CommandHandler handler;
        private LogService LS;
        private EventService ES;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            LS = new LogService();
            await LS.Write("Starting NoctVogel", ConsoleColor.DarkGreen);
            discordclient = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = true,
                DefaultRetryMode = RetryMode.AlwaysRetry
            });
            ES = new EventService(discordclient, LS);
            discordclient.Log += ES.Log;
            
            // Load credentials
            string jsonstring = File.ReadAllText("credentials.json");
            var tokens = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonstring);

            // Connect to Discord
            await LS.Write("Connecting to Discord...", ConsoleColor.DarkGreen);
            var time = Stopwatch.StartNew();
            await discordclient.LoginAsync(TokenType.Bot, tokens["discordToken"]);
            await discordclient.StartAsync();
            time.Stop();
            await LS.Write($"Connected in {time.Elapsed.TotalSeconds.ToString("F3")} seconds", ConsoleColor.DarkGreen);

            // Subscribe to events
            discordclient.MessageReceived += ES.MessageReceived;
            discordclient.Connected += ES.AutoSetGame;

            // Connect to Twitter
            await LS.Write("Authenticating to Twitter...", ConsoleColor.DarkGreen);
            time = Stopwatch.StartNew();
            Auth.SetUserCredentials(tokens["consumerKey"], tokens["consumerSecret"], tokens["accessToken"], tokens["accessSecret"]);
            time.Stop();
            await LS.Write($"Connected in {time.Elapsed.TotalSeconds.ToString("F3")} seconds", ConsoleColor.DarkGreen);
            tweetuser = User.GetAuthenticatedUser();

            // Initialize the CommandHandler and ServiceProvider
            var services = BuildServiceProvider();
            handler = new CommandHandler(services);
            await handler.InitializeAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection()
                .AddSingleton(discordclient)
                .AddSingleton(new CommandService())
                .AddSingleton(LS)
                .AddSingleton(ES)
                .AddSingleton(tweetuser);
            var provider = services.BuildServiceProvider();
            return provider;
        }
    }
}
