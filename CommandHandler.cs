using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using NoctVogel.Services;
using System.Diagnostics;

namespace NoctVogel
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient discordclient;
        private readonly CommandService CS;
        private readonly LogService LS;
        private readonly IServiceProvider ISP;

        Stopwatch T = new Stopwatch();

        void SWatchStart()
            => T = Stopwatch.StartNew();

        void SWatchStop()
            => T.Stop();


        public CommandHandler(IServiceProvider provider)
        {
            ISP = provider;
            discordclient = ISP.GetService<DiscordSocketClient>();
            CS = ISP.GetService<CommandService>();
            LS = ISP.GetService<LogService>();
        }

        public async Task InitializeAsync()
        {
            await CS.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: ISP);
            discordclient.MessageReceived += HandleCommandAsync;
        }

        public async Task HandleCommandAsync(SocketMessage parameterMessage)
        {
            var ch = parameterMessage.Channel as IGuildChannel;
            var G = ch.Guild as IGuild;
            //Prevent commands triggered by itself or other bots
            if (!(parameterMessage is SocketUserMessage message) || message.Author.IsBot) return;
            int argPos = 0;
            if (!(message.HasMentionPrefix(discordclient.CurrentUser, ref argPos) || message.HasStringPrefix("n]", ref argPos))) return;
            var context = new SocketCommandContext(discordclient, message);
            var result = await CS.ExecuteAsync(context, argPos, ISP);

            //Command success/fail notice message
            if (result.IsSuccess)
            {
                await LS.Write($"Command finished in {T.Elapsed.TotalSeconds.ToString("F3")} seconds", ConsoleColor.DarkMagenta);
            }
            else
            {
                //Prevents unknown command exception to be posted as an error message in discord
                if (result.Error.Value != CommandError.UnknownCommand)
                    await message.Channel.SendMessageAsync(result.ToString());
                await LS.WriteError($"{result}\n{result.ErrorReason}", ConsoleColor.DarkRed);
            }
        }
    }
}
