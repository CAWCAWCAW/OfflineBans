using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;
using TShockAPI.Net;

namespace OfflineBans
{
    [ApiVersion(1, 14)]
    public class OfflineBans : TerrariaPlugin
    {
        public OfflineBans(Main game)
            : base(game)
        {
            Order = 5;
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        }

        void OnInitialize(EventArgs e)
        {
            Commands.ChatCommands.Add(new Command("tshock.admin.ban", OfflineBan, "oban")
                {
                    AllowServer = true
                });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override Version Version
        {
            get { return new Version("0.9.10.20"); }
        }
        public override string Name
        {
            get { return "OfflineBans"; }
        }
        public override string Author
        {
            get { return "Commaster"; }
        }
        public override string Description
        {
            get { return "Bans for offline registered users."; }
        }

        private static void OfflineBan(CommandArgs args)
        {
            if (args.Parameters.Count == 0 || args.Parameters[0].ToLower() == "help")
			{
				args.Player.SendInfoMessage("Syntax: /oban add name [reason]");
                args.Player.SendInfoMessage("Other subcommands are provided by /ban.");
				return;
			}
			if (args.Parameters[0].ToLower() == "list" || args.Parameters[0].ToLower() == "listip")
			{
				#region Default commands
				args.Player.SendInfoMessage("Use the default /ban for that.");
                return;
				#endregion Default commands
			}
			if (args.Parameters.Count >= 2)
			{
				if (args.Parameters[0].ToLower() == "add")
				{
					#region Add ban
					string plStr = args.Parameters[1];
					var player = TShock.Users.GetUserByName(plStr);
					if (player == null)
					{
						args.Player.SendErrorMessage("Invalid player!");
					}
					else
					{
						string reason = args.Parameters.Count > 2
											? String.Join(" ", args.Parameters.GetRange(2, args.Parameters.Count - 2))
											: "Misbehavior.";
                        bool force = !args.Player.RealPlayer;
                        string adminUserName = args.Player.UserAccountName;
                        if (force || !TShock.Groups.GetGroupByName(player.Group).HasPermission(Permissions.immunetoban))
			            {
                            string ip = player.KnownIps.Substring(player.KnownIps.IndexOf("\"")+1);
                            ip = ip.Substring(0, ip.IndexOf("\""));
				            string uuid = player.UUID;
				            string playerName = player.Name;
                            //Log.ConsoleInfo(string.Format("Ready to ban IP:{0} UUID:{1} Name:{2}",ip,uuid,playerName));
				            TShock.Bans.AddBan(ip, playerName, uuid, reason, false, adminUserName);
				            Log.ConsoleInfo(string.Format("Banned {0} for : '{1}'", playerName, reason));
				            string verb = force ? "force " : "";
				            if (string.IsNullOrWhiteSpace(adminUserName))
					            TShock.Utils.Broadcast(string.Format("{0} was {1}banned for '{2}'", playerName, verb, reason.ToLower()));
				            else
					            TShock.Utils.Broadcast(string.Format("{0} {1}banned {2} for '{3}'", adminUserName, verb, playerName, reason.ToLower()));
			            }
						else
						{
							args.Player.SendErrorMessage("You can't ban another admin!");
						}
					}
					return;
					#endregion Add ban
				}
				else
				{
					#region Default commands 2
				    args.Player.SendInfoMessage("Use the default /ban for that.");
                    return;
				    #endregion Default commands 2
				}
            }
			args.Player.SendErrorMessage("Invalid syntax or old command provided.");
			args.Player.SendErrorMessage("Type /oban help for more information.");
        }
    }
}
