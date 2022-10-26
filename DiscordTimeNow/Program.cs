#define DEBUG

using System;
using System.Threading;
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

        //---------------------------------------------------------------------------------------------
        //Seriializable exposes the function names to Discord.
        //---------------------------------------------------------------------------------------------
        [Serializable()]
        public class GuildStuff {
            public string PendingRole { get; set; }
            public string OwnRole { get; set; }
            public string DontOwnRole { get; set; }
            public ulong GeneralChannel { get; set; }
        }

        [Serializable()]
        public class GuildStuff103
        {
            public string PendingRole { get; set; }
            public string OwnRole { get; set; }
            public string DontOwnRole { get; set; }
            public ulong GeneralChannel { get; set; }
            public ulong Brands { get; set; }
            public ulong Commands { get; set; }
            public ulong TTLCommands { get; set; }
            public string WelcomeMSG { get; set; }
        }

        static public bool thread1exit;
        Thread oThread;

        static public SortedDictionary<ulong, DateTime> map;
        static public SortedDictionary<ulong, ulong> Guildbrands;
        //static public SortedDictionary<ulong, GuildStuff> GuildStuffs;
        static public SortedDictionary<ulong, GuildStuff103> GuildStuffs103;
        static public Dictionary<string, string> DictTimeZoneAbrA;
        static public string[] ArrayUrukNames;
        static public string[] ArrayUrukTitles;

        private DiscordSocketClient client;
        private CommandHandler handler;

        string FileToken = "";
        public static readonly string appdir = AppContext.BaseDirectory;
        BotData ConfigData;

        ~Program() {
            SaveData();
            thread1exit = true;
            oThread.Abort();
        }

        public async Task Start()
        {
            map = new SortedDictionary<ulong, DateTime>();
            //GuildStuffs = new SortedDictionary<ulong, GuildStuff>();
            GuildStuffs103 = new SortedDictionary<ulong, GuildStuff103>();
            Guildbrands = new SortedDictionary<ulong, ulong>();
            thread1exit = false;
            
            ConfigData = new BotData();
            ConfigData = ConfigData.Load();
            LoadData();

            initZoneDict();
            initUrukNames();
            initUrukTitles();  

            FileToken = ConfigData.Token;



            client = new DiscordSocketClient();
            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose,
            });

#if !DEBUG
             
#endif
#if DEBUG
            
#endif
            client.Log += Logger;
            
#if !DEBUG
            await client.LoginAsync(TokenType.Bot, LiveToken);
#endif
#if DEBUG
            await client.LoginAsync(TokenType.Bot, FileToken);
#endif
            await client.StartAsync();

            var serviceProvider = ConfigureServices();
            handler = new CommandHandler(serviceProvider);
            await handler.ConfigureAsync();

            oThread = new Thread(new ThreadStart(SaveDataThread));
            oThread.Start();

            
            //Block this program untill it is closed
            await Task.Delay(-1);
            //await client.Disconnected().;

        }
        //---------------------------------------------------------------------------------------------
        //Discord Log handling, changes colour of console logs depending on severity of error.
        //---------------------------------------------------------------------------------------------
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

        public async Task restartmethodxd()
        {
            await Task.Delay(10000);

            string thisexefile = Path.Combine(appdir, "DiscordTimeNow.exe");
            System.Diagnostics.Process.Start(thisexefile);
            Environment.Exit(0);

        }

        public IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(client)
                 .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false }));
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);

            return provider;
        }

        //---------------------------------------------------------------------------------------------
        //Call to save all functions for ease of access.
        //---------------------------------------------------------------------------------------------
        static public void SaveData() {
            SaveTimeData();
            SaveGuildData();

        }
        //---------------------------------------------------------------------------------------------
        //Auto Save every X seconds.
        //---------------------------------------------------------------------------------------------
        static public void SaveDataThread()
        {
            while (!thread1exit)
            {
                SaveTimeData();
                SaveGuildData();

                Thread.Sleep(100000);
            }
        }

        //---------------------------------------------------------------------------------------------
        //Save DateTimes as bin
        //---------------------------------------------------------------------------------------------

        static public void SaveTimeData()
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var fi = new System.IO.FileInfo(@"./configuration/DateTimes.bin");

            using (var binaryFile = fi.Create())
            {
                binaryFormatter.Serialize(binaryFile, map);
                binaryFile.Flush();
            }

        }

        //static public void SaveGuildData()
        //{
        //    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //    var fi = new System.IO.FileInfo(@"./configuration/GuildStuff_103.bin");

        //    using (var binaryFile = fi.Create())
        //    {
        //        binaryFormatter.Serialize(binaryFile, GuildStuffs103);
        //        binaryFile.Flush();
        //    }

        //}

        //---------------------------------------------------------------------------------------------
        //Save Guild Data as GuildStuff.bin
        //---------------------------------------------------------------------------------------------
        static public void SaveGuildData()
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var fi = new System.IO.FileInfo(@"./configuration/GuildStuff_103.bin");

            using (var binaryFile = fi.Create())
            {
                binaryFormatter.Serialize(binaryFile, GuildStuffs103);
                binaryFile.Flush();
            }

        }

        //---------------------------------------------------------------------------------------------
        //LoadData(void)
        //Loads all the saved data for the Discord Server.
        //DateTimes() as Bin
        //GuildBrands() as Bin
        //GuildStuff() as Bin
        //---------------------------------------------------------------------------------------------
        void LoadData()
        {
            

            if (File.Exists(@"./configuration/DateTimes.bin"))
            {
                var fi = new System.IO.FileInfo(@"./configuration/DateTimes.bin");
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                SortedDictionary<ulong, DateTime> readBack;
                using (var binaryFile = fi.OpenRead())
                {
                    readBack = (SortedDictionary<ulong, DateTime>)binaryFormatter.Deserialize(binaryFile);
                }
                map = readBack;
            }

            
            //if (File.Exists(@"./configuration/GuildStuff.bin"))
            //{
            //    var fi = new System.IO.FileInfo(@"./configuration/GuildStuff.bin");
            //    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            //    SortedDictionary<ulong, GuildStuff> readBack;
            //    using (var binaryFile = fi.OpenRead())
            //    {
            //        readBack = (SortedDictionary<ulong, GuildStuff>)binaryFormatter.Deserialize(binaryFile);
            //    }
            //    GuildStuffs = readBack;

            //    foreach (KeyValuePair<ulong, GuildStuff> i in GuildStuffs)
            //    {
            //        GuildStuff103 tmpGuildNew = new GuildStuff103();
            //        tmpGuildNew.Brands = 0;
            //        tmpGuildNew.Commands = 0;
            //        tmpGuildNew.DontOwnRole = i.Value.DontOwnRole; //
            //        tmpGuildNew.GeneralChannel = i.Value.GeneralChannel; //
            //        tmpGuildNew.OwnRole = i.Value.OwnRole; //
            //        tmpGuildNew.PendingRole = i.Value.PendingRole; //
            //        tmpGuildNew.TTLCommands = 0;
            //        tmpGuildNew.WelcomeMSG = ", please get a profile pic if you haven't already, and enjoy your stay.";
            //        GuildStuffs103.Add(i.Key, tmpGuildNew);
            //    }
            //}

            if (File.Exists(@"./configuration/GuildStuff_103.bin"))
            {
                var fi = new System.IO.FileInfo(@"./configuration/GuildStuff_103.bin");
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                SortedDictionary<ulong, GuildStuff103> readBack;
                using (var binaryFile = fi.OpenRead())
                {
                    readBack = (SortedDictionary<ulong, GuildStuff103>)binaryFormatter.Deserialize(binaryFile);
                }
                GuildStuffs103 = readBack;
            }

            if (File.Exists(@"./configuration/GuildBrands.bin"))
            {
                var fi = new System.IO.FileInfo(@"./configuration/GuildBrands.bin");
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                SortedDictionary<ulong, ulong> readBack;
                using (var binaryFile = fi.OpenRead())
                {
                    readBack = (SortedDictionary<ulong, ulong>)binaryFormatter.Deserialize(binaryFile);
                }
                Guildbrands = readBack;
            }

        }
        //---------------------------------------------------------------------------------------------
        //Init TimeZones, This could be loaded from file in the future build.
        //---------------------------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------------------------
        //Names of Uruk creatures from Lord of the Rings
        //---------------------------------------------------------------------------------------------
        void initUrukNames()
        {
            ArrayUrukNames = new string[] {
                "Akoth",
                "Amûg",
                "Ashgarn",
                "Azdûsh",
                "Bagabug",
                "Barfa",
                "Blorg",
                "Bolg",
                "Borgu",
                "Brogg",
                "Bûbol",
                "Bûth",
                "Dûgza",
                "Dûsh",
                "Dûshrat",
                "Dharg",
                "Feldûsh",
                "Felgrat",
                "Flak",
                "Folgûm",
                "Ghâm",
                "Ghûra",
                "Gimub",
                "Glûk",
                "Golm",
                "Gorfel",
                "Gorgûm",
                "Goroth",
                "Grublik",
                "Gûndza",
                "Horhog",
                "Hork",
                "Horza",
                "Hoshû",
                "Hoshgrish",
                "Hûmgrat",
                "Hûra",
                "Ishgha",
                "Ishmoz",
                "Kâka",
                "Kothûg",
                "Krimp",
                "Krûk",
                "Kugáluga",
                "Lamlûg",
                "Latbag",
                "Lorm",
                "Lûga",
                "Lûgdash",
                "Lûgnak",
                "Mâku",
                "Malmûg",
                "Mogg",
                "Mormog",
                "Mozfel",
                "Muggrish",
                "Mûglûk",
                "Muzglob",
                "Nákra",
                "Nazdûg",
                "Názkûga",
                "Nazû",
                "Norsko",
                "Norûk",
                "Ogbur",
                "Ogthrak",
                "Olgoth",
                "Olrok",
                "Orthog",
                "Pâsh",
                "Pígug",
                "Prâk",
                "Pûg",
                "Pûgrish",
                "Pushkrimp",
                "Ratanák",
                "Ratbag",
                "Ratlûg",
                "Ronk",
                "Rûg",
                "Rûkdûg",
                "Shágflak",
                "Shaká",
                "Skak",
                "Skûn",
                "Snagog",
                "Takra",
                "Târz",
                "Thakrak",
                "Thrak",
                "Torz",
                "Tûgog",
                "Tûkâ",
                "Tûmhom",
                "Tûmûg",
                "Ûgakûga",
                "Ûggû",
                "Ûkbûk",
                "Ûkrom",
                "Ûkshak",
                "Ûshbaka",
                "Ûshgol",
                "Uthûg",
                "Zâthra",
                "Zog",
                "Zogdûsh",
                "Zûgor",
                "Zûmug",
                "Zunn"
               };
        }
        //---------------------------------------------------------------------------------------------
        //Names of Uruk creatures from Lord of the Rings
        //---------------------------------------------------------------------------------------------
        void initUrukTitles()
        {
            ArrayUrukTitles = new string[] {
                "Archer Trainer",
                "Ash-Skin",
                "Bag-Head",
                "Barrel-Scraper",
                "Beastmaster",
                "Beast Slayer",
                "Berserker Master",
                "Black-Blade",
                "Black-Heart",
                "Black-Thorn",
                "Blade Master",
                "Blade Sharpener",
                "Blade Smith",
                "Blood Licker",
                "Blood-Lover",
                "Blood-Hand",
                "Blood-Storm",
                "Bone Collector",
                "Bone-Crusher",
                "Bone-Licker",
                "Bone-Ripper",
                "Bone-Snapper",
                "Brain Damaged",
                "Brawl-Master",
                "Bow Master",
                "Brawler",
                "Bright-Eyes",
                "Broken-Shield",
                "Cannibal",
                "Caragor-Fang",
                "Caragor Slayer",
                "Caragor Tamer",
                "Corpse-Eater",
                "Corpse Grinder",
                "Cave-Rat",
                "Chain Driver",
                "Dead-Eye",
                "Death-Blade",
                "Deathbringer",
                "Drooler",
                "Dung-Collector",
                "Dwarf-Eater",
                "Dwarf-Killer",
                "Eagle Eye",
                "Elf-Slayer",
                "Evil Eye",
                "Eye-Gouger",
                "Fast Feet",
                "Fat Head",
                "Fire-Brander",
                "Flame Monger",
                "Flesh Glutton",
                "Flesh-Picker",
                "Flesh-Render",
                "Flesh-Rot",
                "Foul-Spawn",
                "Frog-Blood",
                "Giggles",
                "Graug Catcher",
                "Graug Slayer",
                "Grog-Burner",
                "Ghûl Hunter",
                "Ghûl Lover",
                "Ghûl Slayer",
                "Gold-Fang",
                "Guard Master",
                "Halfling-Lover",
                "Head-Chopper",
                "Head-Hunter",
                "Heart-Eater",
                "Horn Blower",
                "Hot Tongs",
                "Iron-Arm",
                "Jaws",
                "Jitters",
                "King-Slayer",
                "Learned Scribe",
                "Lice-Head",
                "Life-Drinker",
                "Limp-Leg",
                "Literate One",
                "Lock-Jaw",
                "Long-Tooth",
                "Lucky Shot",
                "Lump-Head",
                "Mad-Eye",
                "Maggot-Nest",
                "Man-Hunter",
                "Man-Stalker",
                "Meat Grinder",
                "Meat-Hooks",
                "Metal-Beard",
                "Metal Beater",
                "Neck Snapper",
                "Night-Bringer",
                "Oath-Breaker",
                "of Lithlad",
                "of the Black Gate",
                "of the Pit",
                "of the Spiders",
                "of the Stench",
                "of the Welts",
                "One-Eye",
                "Pain-Lover",
                "Pit Fighter",
                "Plague-Bringer",
                "Pot-Licker",
                "Prison Master",
                "the Quarter Master",
                "Quick-Blades",
                "Rabble Rouser",
                "Raid Leader",
                "Ranger-Killer",
                "Ravager",
                "Raw-Head",
                "Runny-Bowls",
                "Sawbones",
                "Scar-Artist",
                "Shaman",
                "Shield Master",
                "Skull Bow",
                "Skull-Cracker",
                "Slashface",
                "Slave Lover",
                "Slave Taskmaster",
                "Storm-Bringer",
                "Sword Master",
                "the All-Eater",
                "the Advisor",
                "the Amputator",
                "the Angry",
                "the Armorer",
                "the Assassin",
                "the Beheader",
                "the Biter",
                "the Black",
                "the Blacksmith",
                "the Bleeder",
                "the Bloated",
                "the Bloody",
                "the Blue",
                "the Bone Collector",
                "the Bowmaster",
                "the Brander",
                "the Brave",
                "the Breaker",
                "the Brewer",
                "the Brother",
                "the Brown",
                "the Brute",
                "the Butcher",
                "the Catcher",
                "the Cave-Born",
                "the Champion",
                "the Choker",
                "the Chunky",
                "the Claw",
                "the Clever",
                "the Collector",
                "the Complainer",
                "the Cook",
                "the Corruptor",
                "the Coward",
                "the Crafty",
                "the Crazy",
                "the Cruel",
                "the Crippler",
                "the Crow",
                "the Dark",
                "the Defender",
                "the Defiler",
                "the Destroyer",
                "the Devourer",
                "the Destroyer",
                "the Diseased",
                "the Disgusting",
                "the Dreamer",
                "the Driver",
                "the Drowned",
                "the Drunk",
                "the Dumb",
                "the Duelist",
                "the Elder",
                "the Endless",
                "the Ever-Wounded",
                "the Executioner",
                "the Fearless",
                "the Filthy",
                "the Flogger",
                "the Fanatical",
                "the Flesh Glutton",
                "the Fool",
                "the Foul",
                "the Friendly",
                "the Funny One",
                "the Gentle",
                "the Gluttonous",
                "the Gorger",
                "the Greedy",
                "the Grim",
                "the Grinder",
                "the Gutless",
                "the Hacker",
                "the Handsome",
                "the Heartless",
                "the Hell-Hawk",
                "the Hook",
                "the Humiliator",
                "the Hungry",
                "the Immovable",
                "the Infernal",
                "the Judge",
                "the Killer",
                "the Kin-Slayer",
                "the Knife",
                "the Large",
                "the Literate One",
                "the Legend",
                "the Loaded",
                "the Lookout",
                "the Mad",
                "the Mangler",
                "the Marauder",
                "the Massive",
                "the Man-Eater",
                "the Meat Hoarder",
                "the Merciful",
                "the Messenger",
                "the Mindless",
                "the Merciless",
                "the Mountain",
                "the Murderous",
                "the Other Twin",
                "the Painted",
                "the Proud",
                "the Puny",
                "the Rash",
                "the Rat",
                "the Raven",
                "the Red",
                "the Reckless",
                "the Relentless",
                "the Ripper",
                "the Ruinous",
                "the Runner",
                "the Runt",
                "the Sadistic",
                "the Savage",
                "the Scholar",
                "the Screamer",
                "the Serpent",
                "the Shadow",
                "the Shield",
                "the Skinless",
                "the Slasher",
                "the Slaughterer",
                "the Small",
                "the Smasher",
                "the Spike",
                "the Stinger",
                "the Stout",
                "the Surgeon",
                "the Swift",
                "the Tongue",
                "the Trainer",
                "the Twin",
                "the Unkillable",
                "the Vile",
                "the Wanderer",
                "the Watcher",
                "the Weak",
                "the Whiner",
                "the Wise",
                "the Wrestler",
                "Thin Bones",
                "Thunderhead",
                "Tree-Killer",
                "Troll Slayer",
                "Troll-Born",
                "Ugly Face",
                "Who Flees",
                "Lice-Head",
                "Wraith Slayer",
                "Wrath-Breeder",
                "the Pink",
                "Wild",
                "the play thing of Sarah"
               };
        }
    }
}
