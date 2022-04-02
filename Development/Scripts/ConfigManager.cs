namespace StartingItemsGUI
{
    internal static class ConfigManager
    {
        // These values will change in the future.
        // There could either be:
        // 1 config file that will store information for all user profiles.
        // Multiple config files, each storing configs for one RoR2 profile (not to be mistaken with the profiles in this mod)
        private static readonly Tommy.TomlTable DefaultConfig = new()
        {
            ["title"] = "Starting Items GUI Config",
            ["version"] = $"{StartingItemsGUI.PluginVersion}",
            ["default_prices"] =
            {
                ["items"] =
                {
                    ["tier1"] = 1000,
                    ["tier2"] = 3750,
                    ["tier3"] = 12500,
                    ["boss"] = 14500,
                    ["lunar"] = 19000,
                    ["default"] = 19000
                },
                ["equipment"] =
                {
                    ["tier1"] = 1000,
                    ["lunar"] = 19000,
                    ["default"] = 8000
                }
            }
        };
       
        public static Tommy.TomlTable Config { get; set; } = null;

        public static string ConfigPath = System.IO.Path.Combine(BepInEx.Paths.BepInExRootPath, "config", StartingItemsGUI.PluginName);
        public static string ConfigFile = System.IO.Path.Combine(ConfigPath, StartingItemsGUI.PluginName + ".toml");

        public static void MakeConfigDirectory()
        {
            Util.MakeDirectoryExist(ConfigPath);
        }

        // Read the config for the first time since application launch
        public static void InitConfig()
        {
            Log.LogInfo("Initializing config.");

            if (!System.IO.Directory.Exists(ConfigPath))
            {
                Log.LogInfo("Making config directory.");
                MakeConfigDirectory();
            }

            if (System.IO.File.Exists(ConfigFile))
            {
                Log.LogInfo("Config file exists. Reading.");
                ReadConfig();
            }
            else
            {
                Log.LogInfo("Config file doesn't exist. Writing default config to file..");
                // Else generate a fresh config.
                Config = DefaultConfig;
                SaveConfig();
            }
        }

        public static void DeleteConfig()
        {
            if (System.IO.File.Exists(ConfigFile))
            {
                Log.LogInfo("Deleting config.");
                System.IO.File.Delete(ConfigFile);
            }
        }

        // Might change this to hash the current config in memory and the config on disk.
        // We would then only save if there is a difference.
        public static void SaveConfig()
        {
            Log.LogInfo("Saving config.");

            using var writer = System.IO.File.CreateText(ConfigFile);

            Config.WriteTo(writer);

            writer.Close();

            Log.LogInfo("Saved config.");
        }

        public static void ReadConfig()
        {
            Log.LogInfo("Reading config.");
            if (!System.IO.File.Exists(ConfigFile))
            {
                Config = DefaultConfig;
                return;
            }

            using var reader = System.IO.File.OpenText(ConfigFile);

            try
            {
                var config = Tommy.TOML.Parse(reader);

                reader.Close();

                Config = config;

                if (Config["version"] != StartingItemsGUI.PluginVersion)
                {
                    Log.LogWarning("Incorrect config version. Updating... If you have updated the mod, ignore this warning.");
                    Config["version"] = StartingItemsGUI.PluginVersion;
                    SaveConfig();
                }
            }
            catch (Tommy.TomlParseException ex)
            {
                // Get access to the table that was parsed with best-effort.
                var table = ex.ParsedTable;

                // Handle syntax error in whatever fashion you prefer
                foreach (var syntaxEx in ex.SyntaxErrors)
                {
                    Log.LogError($"Error on {syntaxEx.Column}:{syntaxEx.Line}: {syntaxEx.Message}");
                    Log.LogError($"Parsed data: {table}");
                }

                Config = DefaultConfig;
                SaveConfig();
            }
        }
    }
}
