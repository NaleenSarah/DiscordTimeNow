using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;

namespace DiscordTimeNow
{
    public class Modules : ModuleBase
    {
        private DateTime pttl { get; set; }

        [Command("timetolive")]
        [Alias("ttl", "time")]
        [Summary("Get Time to Live stream")]
        public async Task InfoRemain()//, IUser user = null)
        {
                DateTime RecievedDate = GetETime(Context.Guild.Id);
                TimeSpan tempTime = RecievedDate - DateTime.UtcNow;
                
                await ReplyAsync(
                    $"\n{Format.Bold("Livestream:")}\n" +
                    $"{Format.Bold("Remaining Time:")} {tempTime.Days} Days, {tempTime.Hours} Hours and {tempTime.Minutes} minutes. \n\n"
                    );
        }

        [Command("timenow")]
        [Summary("Gets the Time in specified country code")]
        public async Task TimeNowCode(string countrycode = "UTC")//, IUser user = null)
        {
            DateTime RecievedDate = DateTime.UtcNow;
            countrycode = countrycode.ToUpper();
            DateTime LocalDate = TimeZoneInfo.ConvertTimeFromUtc(RecievedDate, TimeZoneInfo.FindSystemTimeZoneById(GetZone(countrycode)));

            await ReplyAsync(
                $"{Format.Bold($"The time in {countrycode} now is: {LocalDate}.")}\n");
        }

        [Command("myeventtime")]
        [Alias("met")]
        [Summary("Get Time to Live stream in indicated zone")]
        public async Task InfoTime( string zone = "UTC")//, IUser user = null)
        {
            DateTime RecievedDate = GetETime(Context.Guild.Id);
            TimeSpan tempTime = RecievedDate - DateTime.UtcNow;
            //var userInfo = user ?? Context.Client.CurrentUser;
            //RecievedDate = TimeZoneInfo.ConvertTimeFromUtc(RecievedDate, TimeZoneInfo.FindSystemTimeZoneById(GetZone(zone)));
            zone = zone.ToUpper();
            DateTime LocalDate = TimeZoneInfo.ConvertTimeFromUtc(RecievedDate, TimeZoneInfo.FindSystemTimeZoneById(GetZone(zone)));

            await ReplyAsync(
                $"{Format.Bold("Livestream Event at:")} {LocalDate} {zone}.\n\n"
                );
        }

        [Command("sett")]
        [Alias("settime")]
        [Summary("Sets the Livestream time, dd MMM yyyy hh:mm tt")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task SetT( string str)
        {
            try
            {
                IGuild guild = Context.Guild;
                DateTime LiveStreamTime = new DateTime();
                LiveStreamTime = DateTime.ParseExact(str, "dd MMM yyyy hh:mm tt", null);

                this.SetETime(guild.Id, LiveStreamTime);
                await ReplyAsync($"Done!\n\n" + $"Time Set: {GetETime(guild.Id)} \n\n");
            }
            catch (Exception e) // catch exception error, reply with the error in string format
            {
                await ReplyAsync(e.ToString());
            }

        }
        [Command("own")]
        [Summary("Sets your role")] //Do not own the games. //Pending //Branded
        public async Task SetRoleBranded()
        {
            IRole singleRoleBra = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Branded");
            IRole singleRoleRem = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Pending");
            IRole singleRoleDon = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Do not own the games.");
            IGuildUser user = Context.User as IGuildUser;
            List<ulong> userRoleIDs = user.RoleIds.ToList<ulong>();

            ulong channelIDGeneral = "CHANGE ME";
            //ulong channelIDGeneralDebug = "CHANGE ME";
            //ulong channelIDRules = "CHANGE ME";
            //ITextChannel msgChannel = Context.Guild.GetTextChannelAsync(channelID);

            if ( userRoleIDs.Exists(x => x.Equals(singleRoleDon.Id)) && !userRoleIDs.Exists(x => x.Equals(singleRoleBra.Id)) )  {
                await user.AddRoleAsync(singleRoleBra);
                await user.RemoveRoleAsync(singleRoleDon);
                var chan = await Context.Guild.GetTextChannelAsync(channelIDGeneral);
                //var chanRules = await Context.Guild.GetTextChannelAsync(channelIDRules);
                await chan.SendMessageAsync($"{Format.Bold("Congratulations " + Context.User.Mention + $", you have now indicated you own one of the games.")}");
            } else
            if ( !userRoleIDs.Exists(x => x.Equals(singleRoleBra.Id)) ) {
                await user.AddRoleAsync(singleRoleBra);
                await user.RemoveRoleAsync(singleRoleRem);
                await user.RemoveRoleAsync(singleRoleDon);
                var chan = await Context.Guild.GetTextChannelAsync(channelIDGeneral);
                //var chanRules = await Context.Guild.GetTextChannelAsync(channelIDRules);
                await chan.SendMessageAsync($"{Format.Bold("Welcome " + Context.User.Mention + $", please get a profile pic if you haven't already, and enjoy your stay.")}");
                
            }
        }
        [Command("nope")]
        [Summary("Sets your role")] //Do not own the games. //Pending //Branded
        public async Task SetRoleNope()
        {
            IRole singleRoleBra = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Branded");
            IRole singleRoleDon = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Do not own the games.");
            IRole singleRoleRem = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Pending");
            IGuildUser user = Context.User as IGuildUser;
            List<ulong> userRoleIDs = user.RoleIds.ToList<ulong>();

            //ulong channelIDGeneralDebug = "CHANGE ME";
            ulong channelIDGeneral = "CHANGE ME";

            if ( !userRoleIDs.Exists(x => x.Equals(singleRoleBra.Id)) && !userRoleIDs.Exists(x => x.Equals(singleRoleDon.Id)) )
            {
                await user.AddRoleAsync(singleRoleDon);
                await user.RemoveRoleAsync(singleRoleRem);
                var chan = await Context.Guild.GetTextChannelAsync(channelIDGeneral);
                await chan.SendMessageAsync($"{Format.Bold("Welcome " + Context.User.Mention + $", please get a profile pic if you haven't already, and enjoy your stay.")}");
            }
        }

        private void SetETime(ulong n, DateTime t)
        {
            if (Program.map.ContainsKey(n))
                Program.map[n] = t;
            else
                Program.map.Add(n, t);
        }
        private DateTime GetETime(ulong n)
        {
            if (Program.map.ContainsKey(n))
                return Program.map[n];
            else
                return DateTime.UtcNow;

        }
        private string GetZone(string z)
        {
            if (Program.DictTimeZoneAbrA.ContainsKey(z))
                return Program.DictTimeZoneAbrA[z];
            else
                return "UTC";

        }
    }
}
