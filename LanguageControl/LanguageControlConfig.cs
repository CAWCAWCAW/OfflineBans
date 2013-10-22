using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.Hooks;

namespace LanguageControl
{
    public class LanguageControlConfig
    {
        private static string ConfigFile = Path.Combine("tshock", "LanguageControl.json");

        public bool FilterNicknames = true;
        public bool FilterChat = true;
        public int ShortestNickname = 3;
        public bool AllowPureNumerical = false;
        
        private LanguageControlConfig Write()
        {
            File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(this, Formatting.Indented));
            return this;
        }

        private static LanguageControlConfig Read()
        {
            if (!File.Exists(ConfigFile))
            {
                WriteDefaults();
            }
            return JsonConvert.DeserializeObject<LanguageControlConfig>(File.ReadAllText(ConfigFile));
        }

        private static void WriteDefaults()
        {
            File.WriteAllText(ConfigFile, @"{
    ""FilterNicknames"": true,
    ""FilterChat"": true,
    ""ShortestNickname"": 3,
    ""AllowPureNumerical"": false
}");
        }

        public static void Load()
        {
            try
            {
                LanguageControl.Config = LanguageControlConfig.Read().Write();
            }
            catch (Exception e)
            {
                Log.ConsoleError("[LanguageControl] Config failed. Check logs for more details.");
                Log.Error(e.ToString());
            }
        }

        public static void Reload(ReloadEventArgs args)
        {
            try
            {
                LanguageControl.Config = LanguageControlConfig.Read().Write();
                args.Player.SendSuccessMessage("[LanguageControl] Config reloaded successfully.");
            }
            catch (Exception ex)
            {
                args.Player.SendErrorMessage("[LanguageControl] Reload failed. Check logs for more details.");
                Log.Error(string.Concat("[LanguageControl] Config failed:\n", ex.ToString()));
            }
        }
    }
}
