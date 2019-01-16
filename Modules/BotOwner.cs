using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace NoctVogel.Modules
{
    public class BotOwner : ModuleBase<SocketCommandContext>
    {

        public BotOwner(IServiceProvider ISP)
        {
        }

        [Command("end")]
        [RequireOwner]
        public async Task ShutDown(){
            await ReplyAsync("Shutting down.");
            Environment.Exit(0);
        }
        
        [Command("reboot")]
        [RequireOwner]
        public async Task Reboot(){
            await ReplyAsync("Restarting.");
            Environment.Exit(1);
        }
    }
}
