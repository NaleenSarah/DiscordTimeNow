using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace DiscordTimeNow
{
    public class Program
    {
        public static void Main(string[] args) =>
           new Program().Start().GetAwaiter().GetResult();

        static public SortedDictionary<ulong, DateTime> map;
        static public Dictionary<string, string> DictTimeZoneAbrA;
        
        private DiscordSocketClient client;
        private CommandHandler handler;

        public async Task Start()
        {
            map = new SortedDictionary<ulong, DateTime>();
            //DictTimeZoneAbrA = new Dictionary<string, string>();

            initZoneDict();

            client = new DiscordSocketClient();
            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose,
            });

            string LiveToken = "CHANGE ME";
            //string DebugToken = "CHANGE ME";

            client.Log += Logger;
            //await client.LoginAsync(TokenType.Bot, DebugToken);
            await client.LoginAsync(TokenType.Bot, LiveToken);
            await client.StartAsync();

            var serviceProvider = ConfigureServices();
            handler = new CommandHandler(serviceProvider);
            await handler.ConfigureAsync();



        //Block this program untill it is closed
        await Task.Delay(-1);

        }
        private static Task Logger(LogMessage lmsg)
        {
            var cc = Console.ForegroundColor;
            switch (lmsg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now} [{lmsg.Severity,8}] {lmsg.Source}: {lmsg.Message}");
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }
        public IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(client)
                 .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false }));
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);

            return provider;
        }

        void initZoneDict()
        {
            DictTimeZoneAbrA = new Dictionary<string, string> { 
                { "IDL" , "Dateline Standard Time" },
                { "UTC-11" , "UTC-11" },
                { "HAST" , "Hawaiian Standard Time" },
                { "AKST" , "Alaskan Standard Time" },
                { "PDT" , "Pacific Standard Time" },
                { "PST" , "Pacific Standard Time" },
                { "MST" , "Mountain Standard Time" },
                { "CST" , "Central America Standard Time" },
                { "CDT" , "Central America Standard Time" },
                { "CT" , "Central Standard Time" },
                { "EDT" , "Eastern Standard Time" },
                { "EST" , "Eastern Standard Time" },
                { "VET" , "Venezuela Standard Time" },
                { "AST" , "Atlantic Standard Time" },
                { "BRT" , "Central Brazilian Standard Time" },
                { "PSAST" , "Pacific SA Standard Time" },
                { "SAWST" , "SA Western Standard Time" },
                { "SAPST" , "Paraguay Standard Time" },
                { "NST" , "Newfoundland Standard Time" },
                { "GNST" , "Greenland Standard Time" },
                { "SAEST" , "SA Eastern Standard Time" },
                { "ART" , "Argentina Standard Time" },
                { "GST" , "Mid-Atlantic Standard Time" },
                { "UTC-2" , "UTC-2" },
                { "AZOT" , "Azores Standard Time" },
                { "CVT" , "Cabo Verde Standard Time" },
                { "GMT" , "UTC" },
                { "BST" , "GMT Standard Time" },
                { "UTC" , "UTC" },
                { "CET" , "Central Europe Standard Time" },
                { "RST" , "Romance Standard Time" },
                { "ECT" , "W. Central Africa Standard Time" },
                { "WET" , "W. Europe Standard Time" },
                { "NMST" , "Namibia Standard Time" },
                { "EET" , "E. Europe Standard Time" },
                { "EGST" , "Egypt Standard Time" },
                { "FLE" , "FLE Standard Time" },
                { "GTBST" , "GTB Standard Time" },
                { "GTB" , "GTB Standard Time" },
                { "JST" , "Jordan Standard Time" },
                { "MEST" , "Middle East Standard Time" },
                { "SAST" , "South Africa Standard Time" },
                { "ABST" , "Arab Standard Time" },
                { "ARST" , "Arabic Standard Time" },
                { "EAT" , "E. Africa Standard Time" },
                { "IRST" , "Iran Standard Time" },
                { "MSK" , "Russian Standard Time" },
                { "AZT" , "Azerbaijan Standard Time" },
                { "AMT" , "Caucasus Standard Time" },
                { "GET" , "Georgian Standard Time" },
                { "MUT" , "Mauritius Standard Time" },
                { "AFT" , "Afghanistan Standard Time" },
                { "WAST" , "West Asia Standard Time" },
                { "PKT" , "Pakistan Standard Time" },
                { "IST" , "India Standard Time" },
                { "SLST" , "Sri Lanka Standard Time" },
                { "NPT" , "Nepal Standard Time" },
                { "YEKT" , "Ekaterinburg Standard Time" },
                { "NCAST" , "Central Asia Standard Time" },
                { "BTT" , "Bangladesh Standard Time" },
                { "MYST" , "Myanmar Standard Time" },
                { "THA" , "SE Asia Standard Time" },
                { "KRAT" , "North Asia Standard Time" },
                { "CHST" , "China Standard Time" },
                { "SST" , "Singapore Standard Time" },
                { "TIST" , "Taipei Standard Time" },
                { "AWST" , "W. Australia Standard Time" },
                { "IRKT" , "Ulaanbaatar Standard Time" },
                { "KST" , "Korea Standard Time" },
                { "TST" , "Tokyo Standard Time" },
                { "ACST" , "AUS Central Standard Time" },
                { "YAKT" , "Yakutsk Standard Time" },
                { "AEST" , "AUS Eastern Standard Time" },
                { "TAST" , "Tasmania Standard Time" },
                { "WPST" , "West Pacific Standard Time" },
                { "VLAT" , "Vladivostok Standard Time" },
                { "SBT" , "Central Pacific Standard Time" },
                { "FJT" , "Fiji Standard Time" },
                { "NZST" , "New Zealand Standard Time" },
                { "UTC+12" , "UTC+12" },
                { "TOT" , "Tonga Standard Time" },
                { "SMST" , "Samoa Standard Time" }
            };
        }
    }
}
