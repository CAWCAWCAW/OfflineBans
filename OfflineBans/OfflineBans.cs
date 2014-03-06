using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace OfflineBans
{
    [ApiVersion(1, 15)]
    public class OfflineBans : TerrariaPlugin
    {
        public OfflineBans(Main game)
            : base(game)
        {
            Order = -1;
        }

        public override void Initialize()
        {
			Commands.ChatCommands.Add(new Command("tshock.admin.ban", OfflineBan, "oban"));
        }

        public override Version Version
        {
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public override string Name
        {
            get { return "OfflineBans"; }
        }

        public override string Author
        {
            get { return "Commaster & simon311"; }
        }

        public override string Description
        {
            get { return "Bans for offline registered users."; }
        }

        private static void OfflineBan(CommandArgs args)
        {
            if (args.Parameters.Count < 1 || args.Parameters[0].ToLower() == "help")
			{
				args.Player.SendInfoMessage("Syntax: /oban add \"name\" [reason]");
				return;
			}

			if (args.Parameters[0].ToLower() != "add")
			{
				#region Default commands
				args.Player.SendInfoMessage("Try use /ban.");
                return;
				#endregion Default commands
			}

			if (args.Parameters.Count >= 2)
			{
				#region Add ban
				string plStr = args.Parameters[1];
				var player = TShock.Users.GetUserByName(plStr);
				if (player == null)
				{
					args.Player.SendErrorMessage("Invalid user!");
					return;
				}
				else
				{
					string reason = args.Parameters.Count > 2
									? String.Join(" ", args.Parameters.GetRange(2, args.Parameters.Count - 2))
									: "Misbehavior.";
					bool force = !args.Player.RealPlayer;
					string adminUserName = args.Player.UserAccountName;
					adminUserName = String.IsNullOrWhiteSpace(adminUserName) ? args.Player.Name : adminUserName;
					if (force || !TShock.Groups.GetGroupByName(player.Group).HasPermission(Permissions.immunetoban))
					{
						List<string> KnownIps = JsonConvert.DeserializeObject<List<string>>(player.KnownIps);
						string ip = KnownIps[KnownIps.Count - 1];
						string uuid = player.UUID;
						string playerName = player.Name;
						TShock.Bans.AddBan(ip, playerName, uuid, reason, false, adminUserName);
						var players = TShock.Utils.FindPlayer(player.Name);
						if (players.Count == 1) players[0].Disconnect(string.Format("Banned: {0}", reason));
						Log.ConsoleInfo(string.Format("{0} has banned {1} for : '{2}'", adminUserName, playerName, reason));
						string verb = force ? "force-" : "";
						if (String.IsNullOrWhiteSpace(adminUserName))
							TSPlayer.All.SendInfoMessage((string.Format("{0} was {1}banned for '{2}'", playerName, verb, reason.ToLower())));
						else
							TSPlayer.All.SendInfoMessage(string.Format("{0} {1}banned {2} for '{3}'", adminUserName, verb, playerName, reason.ToLower()));
					}
					else
					{
						args.Player.SendErrorMessage("You can't ban another admin!");
					}
				}
				return;
				#endregion Add ban
				
            }
			args.Player.SendInfoMessage("Syntax: /oban add \"name\" [reason]");
        }
    }
}
