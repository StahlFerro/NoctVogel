using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Tweetinvi;
using Tweetinvi.Models;

namespace NoctVogel.Modules
{
    public class TwitterCommands : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient discordclient;
        private readonly IAuthenticatedUser tweetuser;

        public TwitterCommands(IServiceProvider ISP)
        {
            discordclient = ISP.GetService<DiscordSocketClient>();
            tweetuser = ISP.GetService<IAuthenticatedUser>();
        }

        [Command("tweet", RunMode = RunMode.Async)]
        [RequireOwner]
        public async Task TweetMsg([Remainder] string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length > 140)
                await ReplyAsync("Tweet message cannot be more than 140 letters");
            tweetuser.PublishTweet(text);
            await ReplyAsync("Tweet successfully sent");
        }
    }
}
