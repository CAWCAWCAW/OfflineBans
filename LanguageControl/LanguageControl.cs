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

namespace LanguageControl
{
    [ApiVersion(1, 14)]
    public class LanguageControl : TerrariaPlugin
    {
        public LanguageControl(Main game)
            : base(game)
        {
            Order = 5;
        }

        public override void Initialize()
        {
            //ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.ServerConnect.Register(this, OnConnect);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            ServerApi.Hooks.ServerChat.Register(this, OnChat);
        }

        /*void OnInitialize(EventArgs e)
        {
        }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerConnect.Deregister(this, OnConnect);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
                ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
            }
            base.Dispose(disposing);
        }

        public override Version Version
        {
            get { return new Version("0.9.10.20"); }
        }
        public override string Name
        {
            get { return "LanguageControl"; }
        }
        public override string Author
        {
            get { return "Commaster"; }
        }
        public override string Description
        {
            get { return "Filters language modifications on clients."; }
        }

        private void OnConnect(ConnectEventArgs args)
        {
            TSPlayer player = new TSPlayer(args.Who);
            if (player == null)
            {
                args.Handled = true;
                return;
            }
            /*if (!TShock.Utils.ValidString(player.Name))
            {
                Log.ConsoleInfo("Caught OnConnect");
                TShock.Utils.ForceKick(player, "bad nickname!", false);
                args.Handled = true;
                return;
            }
            if (player.Name.Length < 3)
            {
                Log.ConsoleInfo("Caught OnConnect");
                TShock.Utils.ForceKick(player, "nickname too short.", false);
                args.Handled = true;
                return;
            }*/
        }

        private void OnJoin(JoinEventArgs args)
        {
            TSPlayer player = TShock.Players[args.Who];
            if (player == null)
            {
                args.Handled = true;
                return;
            }
            if (!TShock.Utils.ValidString(player.Name))
            {
                Log.ConsoleInfo("Caught OnConnect");
                TShock.Utils.ForceKick(player, "bad nickname!", false);
                args.Handled = true;
                return;
            }
            if (player.Name.Length < 3)
            {
                Log.ConsoleInfo("Caught OnConnect");
                TShock.Utils.ForceKick(player, "nickname too short.", false);
                args.Handled = true;
                return;
            }
        }

        private void OnChat(ServerChatEventArgs args)
        {
            if (!TShock.Utils.ValidString(args.Text))
            {
                TSPlayer player = TShock.Players[args.Who];
                player.SendMessage("Pfui!", Color.Chocolate);
                args.Handled = true;
                return;
            }
        }
    }
}
