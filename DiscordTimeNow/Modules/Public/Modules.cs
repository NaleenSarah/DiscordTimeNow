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
        [Command("help")]
        [Alias("help_dlh")]
        [Summary("Provides help infomartion")]
        public async Task InfoHelp()
        {
            IGuild guild = Context.Guild;
            IGuildUser user = Context.User as IGuildUser;
            if (Context.User.Id == 121586762622566405 || user.GuildPermissions.Administrator)
            {
                await ReplyAsync(
                $"{Format.Bold(" __Discord Lobby Helper__ ")}\n\n" +
                    $"{Format.Bold("__Admin__ ")}\n" +
                    $"exec_dlh: set defaults for the server, run exec_dlh command for more info.\n" +
                    $"exec_dlh_x: \"<Pending Role>\" \"<Own Role>\" \"<Don'tOwn Role>\" <GenChatId>\n" +
                    $"exec_dlh_pending \"<Name>\": Sets the \"<Name>\" of Pending Role\n" +
                    $"exec_dlh_own \"<Name>\": Sets the \"<Name>\" of Owning Role\n" +
                    $"exec_dlh_don \"<Name>\": Sets the \"<Name>\" of Dont Own Role\n" +
                    $"exec_dlh_gchan <ChannelID>: Sets the ID # of Channel\n" +
                    $"{Format.Bold("__Mod__ ")}\n" +
                    $"sett \"dd MMM yyyy hh: mm tt\" < ZONE >, \"18 Sep 1970 04:14 PM\" UTC, default < Zone > in PDT\n" +
                    $"dlh_get:, Gets the setup values\n\n" +
                    $"{Format.Bold("__Public__ ")}\n" +
                    $"time, ttl, timetolive:\t Displays Time until Event\n" +
                    $"timenow <ZONE>:\t Displays Time in <Zone>, default = UTC\n" +
                    $"myeventtime, met <ZONE>:\t Displays Event Time in <Zone>, default = UTC\n" +
                    $"zonediff, UTC+ <ZONE>:\t Displays +/- Zone Hours from UTC, default = UTC\n" +
                    $"own:\t Sets role of user to own game.\n" +
                    $"nope:\t Sets role of user to don't own game.\n" +
                    $"uruk:\t Display a random Uruk Name.\n" +
                    $"urukme:\t Sets your name to a random Uruk.\n" + 
                    $"nickreset:\t Resets your Nick.\n" +
                    $"dlh_stats:\t Stats baby!.\n"
                    );
            } else
            if (user.GuildPermissions.ManageRoles)
            {
                await ReplyAsync(
                    $"{Format.Bold(" __Discord Lobby Helper__ ")}\n\n" +
                    $"{Format.Bold("__Mod__ ")}\n" +
                    $"sett \"dd MMM yyyy hh: mm tt\" < ZONE >, \"18 Sep 1970 04:14 PM\" UTC, default < Zone > in PDT\n" +
                    $"dlh_get:, Gets the setup values\n\n" +
                    $"{Format.Bold("__Public__ ")}\n" +
                    $"time, ttl, timetolive:\t Displays Time until Event\n" +
                    $"timenow <ZONE>:\t Displays Time in <Zone>, default = UTC\n" +
                    $"myeventtime, met <ZONE>:\t Displays Event Time in <Zone>, default = UTC\n" +
                    $"zonediff, UTC+ <ZONE>:\t Displays +/- Zone Hours from UTC, default = UTC\n" +
                    $"own:\t Sets role of user to own game.\n" +
                    $"nope:\t Sets role of user to don't own game.\n" +
                    $"uruk:\t Display a random Uruk Name.\n" +
                    $"urukme:\t Sets your name to a random Uruk.\n" +
                    $"nickreset:\t Resets your Nick.\n" +
                    $"dlh_stats:\t Stats baby!.\n"
                     );
            }
            else
            {
                await ReplyAsync(
                    $"{Format.Bold(" __Discord Lobby Helper__ ")}\n\n" +

                    $"time, ttl, timetolive:\t Displays Time until Event\n" +
                    $"timenow <ZONE>:\t Displays Time in <Zone>, default = UTC\n" +
                    $"myeventtime, met <ZONE>:\t Displays Event Time in <Zone>, default = UTC\n" +
                    $"zonediff, UTC+ <ZONE>:\t Displays +/- Zone Hours from UTC, default = UTC\n" +
                    $"own:\t Sets role of user to own game.\n" +
                    $"nope:\t Sets role of user to don't own game.\n" +
                    $"uruk:\t Display a random Uruk Name.\n" +
                    $"urukme:\t Sets your name to a random Uruk.\n" +
                    $"nickreset:\t Resets your Nick.\n" +
                    $"dlh_stats:\t Stats baby!.\n"
                     );
            }
            Program.GuildStuffs103[guild.Id].TTLCommands++;
            Program.GuildStuffs103[guild.Id].Commands++;
        }

        [Command("dlh_testsave")]
        [Summary("Provides a test Save, Should never need to be run.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task TestSave() {
            if (Context.User.Id == 121586762622566405)
            {
                Program.SaveData();
                await ReplyAsync(
                      $"{Format.Bold("Test Save Complete")}.");
            }
            else {
                await ReplyAsync(
                       $"{Format.Bold("Test Save Complete")}.");
            }

        }

        [Command("exec_dlh")]
        [Summary("Default Values for bot setup")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task SetupGuild()
        {
            IGuildUser user = Context.User as IGuildUser;
            if (Context.User.Id == 121586762622566405 || user.GuildPermissions.Administrator) { 
                    IGuild guild = Context.Guild;
                int PreSet = SetGuildStuff(guild.Id);

                switch (PreSet)
                {
                    case 0: //Defaults
                        await ReplyAsync(
                            $"{Format.Bold("Use additional Setup Commands:\n exec_dlh_pending, exec_dlh_own, \n exec_dlh_don, exec_dlh_gchan. \n To fully configure bot.")}\n Use help command for instructions.\n"
                            );
                        break;
                    case 1: //Changes Set
                        await ReplyAsync(
                           $"{Format.Bold("Changes have been made\n")}.");
                        break;
                    case 2: //No Changes
                        await ReplyAsync(
                        $"{Format.Bold("No Changes have been made\n")}.");
                        break;
                }
                await ReplyAsync(
                    $"{Format.Bold("Setup Altered Done:")}.\n"
                    );
                Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
                Program.GuildStuffs103[Context.Guild.Id].Commands++;
            }
        }

        [Command("exec_dlh_x")]
        [Summary("Default Values for bot setup")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task SetupGuildX(string penRole, string ownRole, string donRole, ulong genId)
        {
            IGuildUser user = Context.User as IGuildUser;
            if (Context.User.Id == 121586762622566405 || user.GuildPermissions.Administrator)
            {
                IGuild guild = Context.Guild;

                Program.GuildStuff103 tmpGuildStuff = GetGuildStuff(guild.Id);
                tmpGuildStuff.PendingRole = penRole;
                tmpGuildStuff.OwnRole = ownRole;
                tmpGuildStuff.DontOwnRole = donRole;
                tmpGuildStuff.GeneralChannel = genId;
                tmpGuildStuff.WelcomeMSG = ", please get a profile pic if you haven't already, and enjoy your stay.";
                int PreSet = SetGuildStuff(guild.Id, tmpGuildStuff);

                await ReplyAsync(
                    $"{Format.Bold("Setup Altered Done.")}.\n"
                    );
            }
            else {
                await ReplyAsync(
                    $"{Format.Bold("Administrator Perms Required!")}.\n"
                    );
            }
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }

        [Command("dlh_get")]
        [Summary("Gets Values for bot setup")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task GetGuildStuff()
        {
            IGuild guild = Context.Guild;

            Program.GuildStuff103 tmpGuildStuff = GetGuildStuff(guild.Id);

            await ReplyAsync(
                $"\n{Format.Bold("Pending Role:")} {tmpGuildStuff.PendingRole}.\n" +
                $"{Format.Bold("Owning Role:")} {tmpGuildStuff.OwnRole}.\n" +
                $"{Format.Bold("Don't Own Role:")} {tmpGuildStuff.DontOwnRole}.\n" +
                $"{Format.Bold("Channel:")} {tmpGuildStuff.GeneralChannel}.\n"
                );
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;

        }

        [Command("exec_dlh_pending")]
        [Summary("Set Pending Role")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetupPendingRole(string penRole)
        {
            IGuild guild = Context.Guild;

            Program.GuildStuff103 tmpGuildStuff = GetGuildStuff(guild.Id);
            tmpGuildStuff.PendingRole = penRole;
            int PreSet = SetGuildStuff(guild.Id, tmpGuildStuff);

            await ReplyAsync(
                $"{Format.Bold("Pending role changed.")}.\n"
                );
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }

        [Command("exec_dlh_own")]
        [Summary("Set Owning Role")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetupOwnRole(string ownRole)
        {
            IGuild guild = Context.Guild;
            Program.GuildStuff103 tmpGuildStuff = GetGuildStuff(guild.Id);
            tmpGuildStuff.OwnRole = ownRole;
            int PreSet = SetGuildStuff(guild.Id, tmpGuildStuff);

            await ReplyAsync(
                $"{Format.Bold("Owning role changed.")}.\n"
                );
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }

        [Command("exec_dlh_don")]
        [Summary("Set Don't Role")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetupDonRole(string donRole)
        {
            IGuild guild = Context.Guild;
            Program.GuildStuff103 tmpGuildStuff = GetGuildStuff(guild.Id);
            tmpGuildStuff.DontOwnRole = donRole;
            int PreSet = SetGuildStuff(guild.Id, tmpGuildStuff);

            await ReplyAsync(
                $"{Format.Bold("Don't role changed.")}.\n"
                );
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }

        [Command("exec_dlh_gchan")]
        [Summary("Set General Chan Id")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetupGeneralChannelID(ulong genId)
        {
            IGuild guild = Context.Guild;
            Program.GuildStuff103 tmpGuildStuff = GetGuildStuff(guild.Id);
            tmpGuildStuff.GeneralChannel = genId;
            int PreSet = SetGuildStuff(guild.Id, tmpGuildStuff);

            await ReplyAsync(
                $"{Format.Bold("General Channel ID Set.")}.\n"
                );
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }

        [Command("timetolive")]
        [Alias("ttl", "time")]
        [Summary("Get Time to Live stream")]
        public async Task InfoRemain()
        {
            DateTime RecievedDate = GetETime(Context.Guild.Id);
            TimeSpan tempTime = RecievedDate - DateTime.UtcNow;

            string countrycode = "PDT";
            DateTime LocalDatePDT = TimeZoneInfo.ConvertTimeFromUtc(RecievedDate, TimeZoneInfo.FindSystemTimeZoneById(GetZone(countrycode)));
            DateTime LocatNowTimePDT = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(GetZone(countrycode)));

            if (tempTime < TimeSpan.Zero)
            {
                await ReplyAsync($"\n{Format.Bold(" __Next Live Stream__:")} Has been and gone.\n");
            }
            else if ((tempTime.TotalHours < 24)) {
                await ReplyAsync(
                    $"{Format.Bold(" __Next Live Stream__ ")}\n" +
                    $"{Format.Bold($"{TodayTomorrow(LocatNowTimePDT, tempTime.TotalHours, GetZone(countrycode))} at:")} {LocalDatePDT.ToShortTimeString()} US Pacific Time\n" +
                    $"{Format.Bold($"Remaining Time:")} {tempTime.Hours} Hours and {tempTime.Minutes} minutes.\n"
                       );
            }
            else
            {
                await ReplyAsync(
                    $"{Format.Bold(" __Next Live Stream__ ")}\n" +
                    $"{Format.Bold("Remaining Time:")} {tempTime.Days} Days, {tempTime.Hours} Hours and {tempTime.Minutes} minutes.\n"
                        );
            }
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }

        [Command("timenow")]
        [Summary("Gets the Time in specified country code")]
        public async Task TimeNowCode(string countrycode = "UTC")
        {
            DateTime RecievedDate = DateTime.UtcNow;
            countrycode = countrycode.ToUpper();
            DateTime LocalDate = TimeZoneInfo.ConvertTimeFromUtc(RecievedDate, TimeZoneInfo.FindSystemTimeZoneById(GetZone(countrycode)));

            await ReplyAsync(
                $"{Format.Bold($"The time in {countrycode} now is: {LocalDate}.")}\n");
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }

        [Command("myeventtime")]
        [Alias("met")]
        [Summary("Get Time to Live stream in indicated zone")]
        public async Task InfoTime( string zone = "UTC")
        {
            DateTime RecievedDate = GetETime(Context.Guild.Id);
            //TimeSpan tempTime = RecievedDate - DateTime.UtcNow;

            zone = zone.ToUpper();
            DateTime LocalDate = TimeZoneInfo.ConvertTimeFromUtc(RecievedDate, TimeZoneInfo.FindSystemTimeZoneById(GetZone(zone)));

            await ReplyAsync(
                $"{Format.Bold("Livestream Event at:")} {LocalDate} {zone}.\n\n"
                );
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }

        [Command("zonediff")]
        [Alias("UTC+")]
        [Summary("Get Time to Live stream in indicated zone")]
        public async Task InfoUTCDiff(string zone = "UTC")//, IUser user = null)
        {
            zone = zone.ToUpper();
            TimeSpan DiffHours = GetUTCDiff(zone);

            await ReplyAsync(
                $"{Format.Bold("Difference from UTC, Hours:")} {DiffHours.Hours}, {Format.Bold("Mins:")} {DiffHours.Minutes}.\n\n"
                );
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }

        [Command("sett")]
        [Alias("settime")]
        [Summary("Sets the Livestream time, dd MMM yyyy hh:mm tt")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task SetT( string strDate = "none" , string strZONE = "PST")
        {
            IGuild guild = Context.Guild;
            if (strDate.ToUpper() != "NONE")
            {
                DateTime LiveStreamTime = new DateTime();
                LiveStreamTime = DateTime.ParseExact(strDate, "dd MMM yyyy hh:mm tt", null);
                LiveStreamTime = DateTime.SpecifyKind(LiveStreamTime, DateTimeKind.Unspecified);

                TimeSpan DiffHours = GetUTCDiff(strZONE);
                LiveStreamTime = LiveStreamTime.AddHours(-DiffHours.Hours);
                LiveStreamTime = LiveStreamTime.AddMinutes(-DiffHours.Minutes);

                this.SetETime(guild.Id, LiveStreamTime);

                if (strZONE == "UTC")
                    await ReplyAsync($"Time Set: {GetETime(guild.Id)} UTC\n");
                else
                {
                    DateTime LocalDate = TimeZoneInfo.ConvertTimeFromUtc(LiveStreamTime, TimeZoneInfo.FindSystemTimeZoneById(GetZone(strZONE)));
                    await ReplyAsync($"Time Set: {LocalDate} {strZONE}\n" + $"Time Set: {GetETime(guild.Id)} UTC\n");
                }
            }
            else {
                this.SetETime(guild.Id, DateTime.UtcNow);
                await ReplyAsync($"Done!\n\n" + $"Time Set: None!\n");
            }
            Program.SaveTimeData();
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }
        [Command("own")]
        [Summary("Sets your role")] //Do not own the games. //Pending //Branded
        public async Task SetRoleBranded()
        {
            IGuild guild = Context.Guild;
            Program.GuildStuff103 tmpGS = GetGuildStuff(guild.Id);

            IRole singleRoleBra = Context.Guild.Roles.FirstOrDefault(x => x.Name == tmpGS.OwnRole);
            IRole singleRoleRem = Context.Guild.Roles.FirstOrDefault(x => x.Name == tmpGS.PendingRole);
            IRole singleRoleDon = Context.Guild.Roles.FirstOrDefault(x => x.Name == tmpGS.DontOwnRole);
            IGuildUser user = Context.User as IGuildUser;
            List<ulong> userRoleIDs = user.RoleIds.ToList<ulong>();

            ulong channelIDGeneral = tmpGS.GeneralChannel;

            if (userRoleIDs.Exists(x => x.Equals(singleRoleDon.Id)) && !userRoleIDs.Exists(x => x.Equals(singleRoleBra.Id)))
            {
                await user.AddRoleAsync(singleRoleBra);
                await user.RemoveRoleAsync(singleRoleDon);
                var chan = await Context.Guild.GetTextChannelAsync(channelIDGeneral);
                await chan.SendMessageAsync($"{Format.Bold("Congratulations " + Context.User.Mention + $", you have now indicated you own one of the games.")}");
                Program.GuildStuffs103[guild.Id].Brands++;
            }
            else
            if (!userRoleIDs.Exists(x => x.Equals(singleRoleBra.Id)))
            {
                await user.AddRoleAsync(singleRoleBra);
                await user.RemoveRoleAsync(singleRoleRem);
                await user.RemoveRoleAsync(singleRoleDon);
                var chan = await Context.Guild.GetTextChannelAsync(channelIDGeneral);
                await chan.SendMessageAsync($"{Format.Bold("Welcome " + Context.User.Mention + $", please get a profile pic if you haven't already, and enjoy your stay.")}");
                Program.GuildStuffs103[guild.Id].Brands++;
            }
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;

        }
        [Command("nope")]
        [Summary("Sets your role")] //Do not own the games. //Pending //Branded
        public async Task SetRoleNope()
        {
            IGuild guild = Context.Guild;
            Program.GuildStuff103 tmpGS = GetGuildStuff(guild.Id);

            IRole singleRoleBra = Context.Guild.Roles.FirstOrDefault(x => x.Name == tmpGS.OwnRole);
            IRole singleRoleRem = Context.Guild.Roles.FirstOrDefault(x => x.Name == tmpGS.PendingRole);
            IRole singleRoleDon = Context.Guild.Roles.FirstOrDefault(x => x.Name == tmpGS.DontOwnRole);
            IGuildUser user = Context.User as IGuildUser;
            List<ulong> userRoleIDs = user.RoleIds.ToList<ulong>();

            ulong channelIDGeneral = tmpGS.GeneralChannel;

            if (!userRoleIDs.Exists(x => x.Equals(singleRoleBra.Id)) && !userRoleIDs.Exists(x => x.Equals(singleRoleDon.Id)))
            {
                await user.AddRoleAsync(singleRoleDon);
                await user.RemoveRoleAsync(singleRoleRem);
                var chan = await Context.Guild.GetTextChannelAsync(channelIDGeneral);
                await chan.SendMessageAsync($"{Format.Bold("Welcome " + Context.User.Mention + $", please get a profile pic if you haven't already, and enjoy your stay.")}");
            }
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }

        [Command("uruk")]
        [Summary("Generates a random Uruk name")]
        public async Task cmdUrukName()//, IUser user = null)
        {
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
            await ReplyAsync($"{MakeUrukName()}\n");
        }

        [Command("dlh_stats")]
        [Alias("TimeToSuffer")]
        [Summary("Sarah is my Master")]
        public async Task cmdStats()//, IUser user = null)
        {
            IGuild guild = Context.Guild;
            Program.GuildStuff103 tmpStats = new Program.GuildStuff103();
            tmpStats = GetGuildStuff(guild.Id);

            EmbedBuilder BuilderMsg = new EmbedBuilder();

            BuilderMsg.WithTitle("Stats");
            BuilderMsg.AddInlineField($"Brands", tmpStats.Brands);

            BuilderMsg.AddInlineField($"Commands", tmpStats.TTLCommands);
            await Context.Channel.SendMessageAsync("", false, BuilderMsg);
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }

        [Command("creator")]
        [Alias("info")]
        [Summary("Sarah is my Master")]
        public async Task cmdMaster()//, IUser user = null)
        {
            EmbedBuilder BuilderMsg = new EmbedBuilder();

            BuilderMsg.WithTitle("Discord Lobby Helper");
            BuilderMsg.AddField($"Programmer:", "SarahWild#0439, Sarah Cooper MSc");
            BuilderMsg.AddField($"Suggested by:", "TalionDiscord#7435");
            BuilderMsg.AddField($"Hosted by:", "Hortator");
            BuilderMsg.AddField($"Discord.NET:", "1.0.2");
            BuilderMsg.AddField($"Version:", "1.1.1");
            BuilderMsg.AddField($"GitHub Source:", "https://github.com/SarahWild/DiscordTimeNow");
            
            await Context.Channel.SendMessageAsync("", false, BuilderMsg);
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }

        /*To Do*/
        [Command("urukme")]
        [Summary("Generates a random Uruk name for you")]
        [RequireUserPermission(GuildPermission.ChangeNickname)]
        public async Task cmdUrukNameMe()//, IUser user = null)
        {
            try
            {
                string tmpUrukName = MakeUrukName();
                IGuildUser user = Context.User as IGuildUser;
                await user.ModifyAsync(x => x.Nickname = new Optional<string>(tmpUrukName));

                await ReplyAsync($"{Context.User.Username}#{Context.User.Discriminator} You are now, {Context.User.Mention}!\n");
            }
            catch (Exception e)
            {
                string filePath = @"D:\Error.txt";

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + e.Message + "<br/>" + Environment.NewLine + "StackTrace :" + e.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
                await ReplyAsync($"Gollum broke it!\n");
                Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
                Program.GuildStuffs103[Context.Guild.Id].Commands++;
            }
        }

        [Command("nickreset")]
        [Summary("Generates a random Uruk name for you")]
        [RequireUserPermission(GuildPermission.ChangeNickname)]
        public async Task cmdNickReset()//, IUser user = null)
        {
            try
            {
                IGuildUser user = Context.User as IGuildUser;
                await user.ModifyAsync(x => x.Nickname = new Optional<string>(""));

                await ReplyAsync($"{Context.User.Username}#{Context.User.Discriminator} You are now, {Context.User.Mention}!\n");
            }
            catch (Exception e)
            {
                string filePath = @"D:\Error.txt";

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + e.Message + "<br/>" + Environment.NewLine + "StackTrace :" + e.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
                await ReplyAsync($"Gollum broke it!\n");
            }
            Program.GuildStuffs103[Context.Guild.Id].TTLCommands++;
            Program.GuildStuffs103[Context.Guild.Id].Commands++;
        }


        private void SetETime(ulong n, DateTime t)
        {
            if (Program.map.ContainsKey(n))
                Program.map[n] = t;
            else
                Program.map.Add(n, t);
        }

        private Program.GuildStuff103 GetGuildStuff(ulong n)
        {
            if (Program.GuildStuffs103.ContainsKey(n))
            {
                return Program.GuildStuffs103[n];
            }
            else
            {
                Program.GuildStuff103 GS = new Program.GuildStuff103();
                GS.DontOwnRole = "Do not own the games.";
                GS.PendingRole = "Pending";
                GS.OwnRole = "Branded";
                GS.GeneralChannel = 0;
                GS.Brands = 0;
                GS.Commands = 0;
                GS.TTLCommands = 0;
                GS.WelcomeMSG = ", please get a profile pic if you haven't already, and enjoy your stay.";
                Program.GuildStuffs103.Add(n, GS);
                return Program.GuildStuffs103[n];
            }

        }

        private int SetGuildStuff(ulong n, Program.GuildStuff103 pGS)
        {
            if (Program.GuildStuffs103.ContainsKey(n))
            {
                Program.GuildStuffs103[n] = pGS;
                return 1;
            }
            else {
                Program.GuildStuff103 GS = new Program.GuildStuff103();
                GS.DontOwnRole = "Do not own the games.";
                GS.PendingRole = "Pending";
                GS.OwnRole = "Branded";
                GS.GeneralChannel = 0;
                GS.Brands = 0;
                GS.Commands = 0;
                GS.TTLCommands = 0;
                GS.WelcomeMSG = ", please get a profile pic if you haven't already, and enjoy your stay.";
                Program.GuildStuffs103.Add(n, GS);
                return 0;
            }
               
        }
        private int SetGuildStuff(ulong n)
        {
            if (Program.GuildStuffs103.ContainsKey(n))
            {
                return 2;
            }
            else
            {
                Program.GuildStuff103 GS = new Program.GuildStuff103();
                GS.DontOwnRole = "Do not own the games.";
                GS.PendingRole = "Pending";
                GS.OwnRole = "Branded";
                GS.GeneralChannel = 0;
                GS.Brands = 0;
                GS.Commands = 0;
                GS.TTLCommands = 0;
                GS.WelcomeMSG = ", please get a profile pic if you haven't already, and enjoy your stay.";
                Program.GuildStuffs103.Add(n, GS);
                return 0;
            }
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
        private string MakeUrukName()
        {
            Random rand = new Random();
            int nme = rand.Next(0, Program.ArrayUrukNames.Length);
            int tle = rand.Next(0, Program.ArrayUrukTitles.Length);

            return $"{Program.ArrayUrukNames[nme]} {Program.ArrayUrukTitles[tle]}";

        }
        private string TodayTomorrow(DateTime localDate, double TotalHours, string zone)
        {
            DateTime tmpNewTime = localDate.AddHours(TotalHours);
            if (tmpNewTime.Day > localDate.Day)
                return "Tomorrow";
            else return "Today";
        }
        private TimeSpan GetUTCDiff(string strZONE) {
            strZONE.ToUpper();
            DateTime timeUTC = DateTime.UtcNow;
            DateTime timeZone = TimeZoneInfo.ConvertTimeFromUtc(timeUTC, TimeZoneInfo.FindSystemTimeZoneById(GetZone(strZONE)));

            TimeSpan DifferenceTime = timeZone.Subtract(timeUTC);
                return DifferenceTime;
        }
    }
}
