using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

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
            if (string.IsNullOrWhiteSpace(text) || text.Length > 280)
                await ReplyAsync("Tweet message cannot be more than 280 letters");
            var tweet = tweetuser.PublishTweet(text);
            await ReplyAsync(tweet.Url);
        }

        [Command("userinfo", RunMode = RunMode.Async)]
        [Alias("uinfo")]
        [RequireOwner]
        public async Task UserInfo([Remainder] string username)
        {
            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
            var user = User.GetUserFromScreenName(username);
            if (user == null)
                await ReplyAsync($"User with username {username} not found");
            var timelineparam = new UserTimelineParameters()
            {
                MaximumNumberOfTweetsToRetrieve = int.MaxValue,
                IncludeRTS = true,
                IncludeEntities = true,
                IncludeContributorDetails = true
            };
            long tweetcount = (await user.GetUserTimelineAsync(1000)).Count();
            long followercount = user.FollowersCount;
            long followingcount = (await user.GetFriendIdsAsync(int.MaxValue -1)).Count();
            long likecount = user.FavouritesCount;
            long listcount = user.ListedCount;
            var statistics = $"Tweets: {tweetcount}\nFollowing: {followingcount}\nFollowers: {followercount}\n" +
                $"Likes: {likecount}\nLists: {listcount}";
            var E = new EmbedBuilder()
                .WithTitle($"{user.Name} @{user.ScreenName}")
                .WithThumbnailUrl(user.ProfileImageUrl)
                .WithDescription(user.Description)
                .AddField("Statistics", statistics, true)
                .AddField("Identifier", user.UserIdentifier, true);
            await ReplyAsync("", embed: E.Build());
        }
    }
}
