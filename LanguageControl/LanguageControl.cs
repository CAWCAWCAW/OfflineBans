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
        public static LanguageControlConfig Config = new LanguageControlConfig();
        public LanguageControl(Main game)
            : base(game)
        {
            Order = 5;
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            ServerApi.Hooks.ServerChat.Register(this, OnChat);
        }

        void OnInitialize(EventArgs e)
        {
            GeneralHooks.ReloadEvent += OnReload;
            LanguageControlConfig.Load();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
                ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
                GeneralHooks.ReloadEvent -= OnReload;
            }
            base.Dispose(disposing);
        }

        public override Version Version
        {
            get { return new Version("0.9.10.23"); }
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

        private void OnJoin(JoinEventArgs args)
        {
            TSPlayer player = TShock.Players[args.Who];
            if (player == null)
            {
                args.Handled = true;
                return;
            }
            if (!TShock.Utils.ValidString(player.Name) && Config.FilterNicknames)
            {
                TShock.Utils.ForceKick(player, "bad nickname!", false);
                args.Handled = true;
                return;
            }
            if (player.Name.Length < Config.ShortestNickname)
            {
                TShock.Utils.ForceKick(player, "nickname too short.", false);
                args.Handled = true;
                return;
            }
        }

        private void OnChat(ServerChatEventArgs args)
        {
            if (!TShock.Utils.ValidString(args.Text) && Config.FilterChat)
            {
                TSPlayer player = TShock.Players[args.Who];
                player.SendMessage("Pfui!", Color.Chocolate);
                args.Handled = true;
                return;
            }
        }

        private void OnReload(ReloadEventArgs args)
        {
            LanguageControlConfig.Reload(args);
        }
    }
}
