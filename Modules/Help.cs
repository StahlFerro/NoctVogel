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
    public class Help : ModuleBase<SocketCommandContext>
    {
        private CommandService CS;
        public Help(CommandService service)
        {
            CS = service;
        }

        [Command("help")]
        [Alias("h")]
        [RequireContext(ContextType.Guild)]
        public async Task HelpModule([Remainder] string command = null)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                var realtotal = CS.Commands.Count();
                var E = new EmbedBuilder()
                    .WithColor(new Color(177, 63, 240))
                    .WithDescription("These are all of my available commands. Type s]help `command` for more info");
                int number = 1;
                int totalc = 0;
                var modules = CS.Modules.OrderBy(x => x.Name);
                foreach (var module in modules)
                {
                    if (module.Name == "Help" || module.Name == "BotOwner") { continue; }
                    string description = null;
                    var commands = module.Commands.OrderBy(x => x.Name);
                    foreach (var c in commands)
                    {
                        //var result = await c.CheckPreconditionsAsync(Context);
                        //if (result.IsSuccess)
                        description += $"`{c.Name}`   ";
                    }
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        E.AddField($"{number}.  {module.Name}", description);
                    }
                    totalc += commands.Count();
                    number++;
                }
                E.WithTitle($"NoctVogel Commands ({totalc} available)");
                await ReplyAsync("", false, E.Build());
            }

            else
            {
                var result = CS.Search(Context, command);
                if (!result.IsSuccess)
                {
                    await ReplyAsync($"Sorry, I couldn't find any commands that matches **{command}**."); return;
                }

                var E = new EmbedBuilder().WithColor(new Discord.Color(177, 63, 240));
                foreach (var match in result.Commands)
                {
                    var c = match.Command;
                    var ca = c.Aliases.Select(x => "s]" + x.ToString()).ToArray();
                    E.WithTitle(string.Join(" / ", ca));
                    E.WithDescription(c.Summary);
                    //E.AddField("Parameters: ", $"{String.Join(", ", c.Parameters.Select(p => p.Name))}");
                }
                await ReplyAsync("", false, E.Build());
            }
        }

    }
}
