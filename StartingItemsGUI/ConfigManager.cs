namespace StartingItemsGUI
{
    // This is named ConfigManager as Config would collide with BepInEx.Config
    public static class ConfigManager
    {
        public static BepInEx.Configuration.ConfigEntry<int> ConfigVersion { get; set; }
        public static BepInEx.Configuration.ConfigEntry<bool> ModEnabled { get; set; }

        // Mode states - Not to be confused with mod state.
        public static BepInEx.Configuration.ConfigEntry<bool> EarnedConsumableModeEnabled { get; set; }
        public static BepInEx.Configuration.ConfigEntry<bool> EarnedPersistentModeEnabled { get; set; }
        public static BepInEx.Configuration.ConfigEntry<bool> FreeModeEnabled { get; set; }

        public static BepInEx.Configuration.ConfigEntry<bool> ShowAllItems { get; set; }

        // These could potentially be consolidated into one dictionary.
        public static BepInEx.Configuration.ConfigEntry<uint> TierOnePrice { get; set; }
        public static BepInEx.Configuration.ConfigEntry<uint> TierTwoPrice { get; set; }
        public static BepInEx.Configuration.ConfigEntry<uint> TierThreePrice { get; set; }
        public static BepInEx.Configuration.ConfigEntry<uint> BossPrice { get; set; }
        public static BepInEx.Configuration.ConfigEntry<uint> LunarPrice { get; set; }
        public static BepInEx.Configuration.ConfigEntry<uint> EquipmentPrice { get; set; }
        public static BepInEx.Configuration.ConfigEntry<uint> LunarEquipmentPrice { get; set; }
        public static BepInEx.Configuration.ConfigEntry<uint> EliteEquipmentPrice { get; set; }
        public static BepInEx.Configuration.ConfigEntry<uint> DefaultItemPrice { get; set; }

        // Credit multipliers
        // These could probably also be consolidated into a dictionary
        public static BepInEx.Configuration.ConfigEntry<float> WinMultiplierConsumable { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> LoseMultiplierConsumable { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> ObliterationMultiplierConsumable { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> LimboMultiplierConsumable { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> EasyMultiplierConsumable { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> NormalMultiplierConsumable { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> HardMultiplierConsumable { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> EclipseMultiplierConsumable { get; set; }

        public static BepInEx.Configuration.ConfigEntry<float> WinMultiplierPersistent { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> LoseMultiplierPersistent { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> ObliterationMultiplierPersistent { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> LimboMultiplierPersistent { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> EasyMultiplierPersistent { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> NormalMultiplierPersistent { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> HardMultiplierPersistent { get; set; }
        public static BepInEx.Configuration.ConfigEntry<float> EclipseMultiplierPersistent { get; set; }

        // It would be so nice to add RiskOfOptions compatibility for this in the future :eyes: :wink:
        public static void InitConfig()
        {
            System.Diagnostics.Debug.Assert(StartingItemsGUI.Instance != null);

            var modConfig = "ModConfig";
            // General mod config
            ConfigVersion = StartingItemsGUI.Instance.Config.Bind(modConfig, "ConfigVersion", 1, "Configuration version. This could change in the future.");
            ModEnabled = StartingItemsGUI.Instance.Config.Bind(modConfig, "ModEnabled", true, "Is the mod currently enabled.");

            EarnedConsumableModeEnabled = StartingItemsGUI.Instance.Config.Bind(modConfig, "EarnedConsumableModeEnabled", true, "Is the 'Earned Consumable' mode currently enabled.");
            EarnedPersistentModeEnabled = StartingItemsGUI.Instance.Config.Bind(modConfig, "EarnedPersistentModeEnabled", true, "Is the 'Earned Persistent' mode currently enabled.");
            FreeModeEnabled = StartingItemsGUI.Instance.Config.Bind(modConfig, "FreeModeEnabled", true, "Is the 'Free' mode currently enabled.");

            ShowAllItems = StartingItemsGUI.Instance.Config.Bind(modConfig, "ShowAllItems", true, "Show all available items in the shop (even ones not yet discovered by the player).");

            var prices = "Prices";
            // Item prices
            TierOnePrice = StartingItemsGUI.Instance.Config.Bind(prices, "TierOne", (uint)40, "How much does a Tier One item cost.");
            TierTwoPrice = StartingItemsGUI.Instance.Config.Bind(prices, "TierTwo", (uint)100, "How much does a Tier Two item cost.");
            TierThreePrice = StartingItemsGUI.Instance.Config.Bind(prices, "TierThree", (uint)400, "How much does a Tier Three item cost.");
            BossPrice = StartingItemsGUI.Instance.Config.Bind(prices, "Boss", (uint)550, "How much does a Boss item cost.");
            LunarPrice = StartingItemsGUI.Instance.Config.Bind(prices, "Lunar", (uint)750, "How much does a Lunar item cost.");

            // Equipment prices
            EquipmentPrice = StartingItemsGUI.Instance.Config.Bind(prices, "Equipment", (uint)350, "How much does normal Equipment cost.");
            LunarEquipmentPrice = StartingItemsGUI.Instance.Config.Bind(prices, "LunarEquipment", (uint)750, "How much does Lunar Equipment cost.");
            EliteEquipmentPrice = StartingItemsGUI.Instance.Config.Bind(prices, "EliteEquipment", (uint)1000, "How much does Elite Equipment cost.");

            // Default Prices
            DefaultItemPrice = StartingItemsGUI.Instance.Config.Bind(prices, "DefaultItemPrice", (uint)1000, "How much does a Default Item cost.");

            var creditMultipliers = "CreditMultipliers";
            // Credits multipliers
            // Game ending multipliers for 'Earned Consumable'
            WinMultiplierConsumable = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "WinMultiplierConsumable", 3.0f, "The multiplier for how many credits to award for a win when the current mode is 'Earned Consumable'.");
            LoseMultiplierConsumable = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "LoseMultiplierConsumable", 1.5f, "The multiplier for how many credits to award for a loss when the current mode is 'Earned Consumable'.");
            ObliterationMultiplierConsumable = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "ObliterationMultiplierConsumable", 2.0f, "The multiplier for how many credits to award for an obliteration when the current mode is 'Earned Consumable'.");
            LimboMultiplierConsumable = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "LimboMultiplierConsumable", 2.5f, "The multiplier for how many credits to award for a limbo when the current mode is 'Earned Consumable'.");
            // Difficulty multipliers for 'Earned Consumable'
            EasyMultiplierConsumable = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "EasyMultiplierConsumable", 2.0f, "The multiplier for how many credits to award on easy difficulty when the current mode is 'Earned Consumable'.");
            NormalMultiplierConsumable = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "NormalMultiplierConsumable", 4.0f, "The multiplier for how many credits to award on normal difficulty when the current mode is 'Earned Consumable'.");
            HardMultiplierConsumable = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "HardMultiplierConsumable", 8.0f, "The multiplier for how many credits to award on hard difficulty when the current mode is 'Earned Consumable'.");
            EclipseMultiplierConsumable = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "EclipseMultiplierConsumable", 10.0f, "The multiplier for how many credits to award on eclipse difficulty when the current mode is 'Earned Consumable'.");

            // Game ending multipliers for 'Earned Persistent'
            WinMultiplierPersistent = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "WinMultiplierPersistent", 2.5f, "The multiplier for how many credits to award for a win when the current mode is 'Earned Persistent'.");
            LoseMultiplierPersistent = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "LoseMultiplierPersistent", 1.0f, "The multiplier for how many credits to award for a loss when the current mode is 'Earned Persistent'.");
            ObliterationMultiplierPersistent = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "ObliterationMultiplierPersistent", 1.5f, "The multiplier for how many credits to award for an obliteration when the current mode is 'Earned Persistent'.");
            LimboMultiplierPersistent = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "LimboMultiplierPersistent", 2.0f, "The multiplier for how many credits to award for a limbo when the current mode is 'Earned Persistent'.");
            // Difficulty multipliers for 'Earned Persistent'
            EasyMultiplierPersistent = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "EasyMultiplierPersistent", 1.0f, "The multiplier for how many credits to award on easy difficulty when the current mode is 'Earned Persistent'.");
            NormalMultiplierPersistent = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "NormalMultiplierPersistent", 2.0f, "The multiplier for how many credits to award on normal difficulty when the current mode is 'Earned Persistent'.");
            HardMultiplierPersistent = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "HardMultiplierPersistent", 4.0f, "The multiplier for how many credits to award on hard difficulty when the current mode is 'Earned Persistent'.");
            EclipseMultiplierPersistent = StartingItemsGUI.Instance.Config.Bind(creditMultipliers, "EclipseMultiplierPersistent", 6.0f, "The multiplier for how many credits to award on eclipse difficulty when the current mode is 'Earned Persistent'.");
        }

        // Reload StartingItemsGUI
        [RoR2.ConCommand(commandName = "reload_sigui", flags = RoR2.ConVarFlags.None, helpText = "Reload StartingItemsGUI Config.")]
        private static void CCReloadConfig(RoR2.ConCommandArgs args)
        {
            System.Diagnostics.Debug.Assert(StartingItemsGUI.Instance != null);

            StartingItemsGUI.Instance.Config.Reload();
        }
    }
}
