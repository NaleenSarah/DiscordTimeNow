# DiscordTimeNow

***New in 1.1.1

Added a json file in /configuration/config.json Add your bot token key here.
File also contains a prefix which is not currently in use.

Added new command dlh_stats

Added a json load for the config
Added file GuildData.cs for future work


***Discord.Net 1.0.0, Lobby and Time bot.

**Admin 
exec_dlh: 	set defaults for the server, run exec_dlh command for more info.
exec_dlh_x: "<Pending Role>" "<Own Role>" "<Don'tOwn Role>" <GenChatId>
exec_dlh_pending "<Name>":	Sets the "<Name>" of Pending Role
exec_dlh_own "<Name>":	Sets the "<Name>" of Owning Role
exec_dlh_don "<Name>":	Sets the "<Name>" of Dont Own Role
exec_dlh_gchan <ChannelID>:	Sets the ID # of Channel

**Mod 
sett "dd MMM yyyy hh: mm tt" < ZONE >, "18 Sep 1970 04:14 PM" UTC, default < Zone > in PDT
dlh_get:, Gets the setup values

**Public 
time, ttl, timetolive:		Displays Time until Event
timenow <ZONE>:     Displays Time in <Zone>, default = UTC
myeventtime, met <ZONE>:	Displays Event Time in <Zone>, default = UTC
zonediff, UTC+ <ZONE>:		Displays +/- Zone Hours from UTC, default = UTC
own:	Sets role of user to own game.
nope:	Sets role of user to don't own game.
uruk:	Display a random Uruk Name.
urukme: Changes nickname of invoker to random uruk name.
dlh_stats:	Basic Stats tracking.


Version 1.1.1 Release 15 Oct 2017.
Created by Sarah Cooper, MSc, 6ft6.
