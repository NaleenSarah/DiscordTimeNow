using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTimeNow
{
    class GuildData
    {
        [JsonIgnore]
        public static readonly string appdir = AppContext.BaseDirectory;

        public string PendingRole { get; set; }
        public string OwnRole { get; set; }
        public string DontOwnRole { get; set; }
        public ulong GeneralChannel { get; set; }
        public ulong Brands { get; set; }
        public ulong Commands { get; set; }
        public ulong TTLCommands { get; set; }
        public string WelcomeMSG { get; set; }


        public GuildData()
        {
            PendingRole = "";
            OwnRole = "";
            DontOwnRole = "";
            GeneralChannel = 0;
            Brands = 0;
            Commands = 0;
            TTLCommands = 0;
            WelcomeMSG = ", please get a profile pic if you haven't already, and enjoy your stay.";
        }

        public void Save(string dir = "configuration/Guilds.json")
        {
            string file = Path.Combine(appdir, dir);
            File.WriteAllText(file, ToJson());
        }
        public GuildData Load(string dir = "configuration/Guilds.json")
        {
            string file = Path.Combine(appdir, dir);
            return JsonConvert.DeserializeObject<GuildData>(File.ReadAllText(file));
        }
        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
