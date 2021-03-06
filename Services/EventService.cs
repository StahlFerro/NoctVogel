﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace NoctVogel.Services
{
    public class EventService
    {
        private DiscordSocketClient discordclient;
        private LogService LS;

        public EventService(DiscordSocketClient discordsocketclient, LogService logservice)
        {
            discordclient = discordsocketclient;
            LS = logservice;
        }

        public Task Log(LogMessage msg) //For built-in Discord.Net logging feature that logs to console and logfile
        {
            Task.Run(async () =>
            {
                var cc = Console.ForegroundColor;
                switch (msg.Severity)
                {
                    case LogSeverity.Critical:
                        cc = ConsoleColor.DarkRed; break;
                    case LogSeverity.Error:
                        cc = ConsoleColor.Red; break;
                    case LogSeverity.Warning:
                        cc = ConsoleColor.Yellow; break;
                    case LogSeverity.Info:
                        cc = ConsoleColor.White; break;
                    case LogSeverity.Verbose:
                        cc = ConsoleColor.White; break;
                    case LogSeverity.Debug:
                        cc = ConsoleColor.White; break;
                }
                if (msg.Exception is CommandException CE)
                {
                    var G = CE.Context.Guild;
                    var tch = CE.Context.Channel;
                    var U = CE.Context.User;
                    await LS.WriteError($"⚠ {CE.Command.Aliases.First()} exception ⚠\n" +
                        $"Guild: {G.Name} ({G.Id})\n" +
                        $"Channel: {tch.Name} ({tch.Id})\n" +
                        $"User: {U} ({U.Id})\n" +
                        $"Exception:\n" +
                        $"{CE.Message}\n{CE.StackTrace}\n{CE.InnerException.Message}\n{CE.InnerException.StackTrace}");
                }
                await LS.Write(msg.ToString(), cc, TimeAppend.Short);
            });
            return Task.CompletedTask;
        }

        public Task MessageReceived(SocketMessage msg)
        {
            Task.Run(async () =>
            {
                //Outputs to console and logs to logfile if the message starts with the n] prefix or is from NoctVogel itself
                if (msg.Content.StartsWith("n]") || msg.Author.Id == 534730188873793537)
                {
                    var ch = msg.Channel as IGuildChannel;
                    var G = ch.Guild as IGuild;
                    await LS.Write($"[{msg.Author}] {msg}", ConsoleColor.Green);
                }
            });
            return Task.CompletedTask;
        }

        public Task AutoSetGame()
        {
            Task.Run(async () =>
            {
                await discordclient.SetGameAsync("First steps");
            });
            return Task.CompletedTask;
        }
    }
}