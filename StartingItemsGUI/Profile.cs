namespace StartingItemsGUI
{
    /// <summary>
    /// The Profile class stores 'Loadouts' (called profiles in version 2.0.0).
    /// Each 'loadout' will store the purchased items.
    /// To mimic existing behaviour, the Profile will decide the ShopMode.
    /// This results in being able to change loadouts, but still keep the currently selected shop mode.
    /// The Profile will gain points at the end of the game, no matter the loadout, if the current ShopMode allows for it (we will not gain points if we have free items).
    /// The loadout will decide what items we currently have. However that will be abstracted away, and we will interact with the Profile object.
    /// </summary>
    public class Profile
    {
        private RoR2.UserProfile _Profile = null;
        private Loadout[] _Loadouts = new Loadout[3];
        private bool _CorrectLoadouts { get { return _CurrentLoadoutIndex.Value >= 0 && _CurrentLoadoutIndex.Value <= _Loadouts.Length - 1; } }

        /// <summary>
        /// The config file for the current profile.
        /// Keep in mind: Each StartingItemsGUI.Profile will have it's own config file inside of the BepInEx/configs/StartingItemsGUI folder.
        /// </summary>
        private BepInEx.Configuration.ConfigFile _Config = null;
        private BepInEx.Configuration.ConfigEntry<uint> _CurrentLoadoutIndex { get; set; }
        private BepInEx.Configuration.ConfigEntry<Enums.EarningMode> _EarningMode { get; set; }
        private BepInEx.Configuration.ConfigEntry<Enums.ShopMode> _ShopMode { get; set; }


        private BepInEx.Configuration.ConfigEntry<uint> _EarnedPersistentTotalCredits { get; set; }
        private BepInEx.Configuration.ConfigEntry<uint> _EarnedPersistentCurrentCredits { get; set; }
        private BepInEx.Configuration.ConfigEntry<uint> _EarnedConsumableTotalCredits { get; set; }
        private BepInEx.Configuration.ConfigEntry<uint> _EarnedConsumableCurrentCredits { get; set; }


        public uint Credits { get { return GetCurrentModeCredits(); } }
        public Enums.ShopMode ShopMode { get { return _ShopMode.Value; } set { _ShopMode.Value = value; } }
        public Enums.EarningMode EarningMode { get { return _EarningMode.Value; } set { _EarningMode.Value = value; } }
        public uint LoadoutCount { get { return (uint)_Loadouts.Length; } }
        public uint CurrentLoadoutIndex { get { return _CurrentLoadoutIndex.Value; } set { _CurrentLoadoutIndex.Value = value; UIDrawer.Refresh(); } }
        public RoR2.UserProfile UserProfile { get; }

        public Profile(RoR2.UserProfile userProfile)
        {
            System.Diagnostics.Debug.Assert(StartingItemsGUI.Instance != null);

            _Profile = userProfile;
            System.Diagnostics.Debug.Assert(_Profile != null);

            InitConfig();
            System.Diagnostics.Debug.Assert(_Config != null);

            for (uint i = 0; i < _Loadouts.Length; i++)
            {
                // Initialise the Loadout from the config.
                _Loadouts[i] = new Loadout(_Config, i);
            }
        }

        private uint GetCurrentModeCredits()
        {

            return 0;
        }

        private void InitConfig()
        {
            // Each profile will have it's own config file.
            // This config file will store the loadouts.
            var profilePath = System.IO.Path.Combine(BepInEx.Paths.ConfigPath, StartingItemsGUI.PluginName, $"{_Profile.fileName}.txt");
            _Config = new BepInEx.Configuration.ConfigFile(profilePath, true);

            // Profile config
            //_Credits = _Config.Bind("Profile", "Credits", (uint)0, "The current amount of Credits this profile has.");
            _CurrentLoadoutIndex = _Config.Bind("Profile", "CurrentLoadoutIndex", (uint)0, "The index of the currently selected Loadout.");
            _EarningMode = _Config.Bind("Profile", "EarningMode", Enums.EarningMode.Stages, "The current Earning Mode for the profile (how credits are earned)");
            _ShopMode = _Config.Bind("Profile", "ShopMode", Enums.ShopMode.EarnedPersistent, "The current Shop Mode for the profile (how the items are purchased)");

            // Mode credits
            _EarnedPersistentTotalCredits = _Config.Bind("Profile", $"{Data.ShopModeNames[Enums.ShopMode.EarnedPersistent]}.TotalCredits", (uint)0, "The total amount of credits the 'Earned Persistent' shop mode has.");
            _EarnedPersistentCurrentCredits = _Config.Bind("Profile", $"{Data.ShopModeNames[Enums.ShopMode.EarnedPersistent]}.CurrentCredits", (uint)0, "The current amount of credits the 'Earned Persistent' shop mode has.");

            _EarnedConsumableTotalCredits = _Config.Bind("Profile", $"{Data.ShopModeNames[Enums.ShopMode.EarnedConsumable]}.TotalCredits", (uint)0, "The total amount of credits the 'Earned Consumable' shop mode has.");
            _EarnedConsumableCurrentCredits = _Config.Bind("Profile", $"{Data.ShopModeNames[Enums.ShopMode.EarnedConsumable]}.CurrentCredits", (uint)0, "The current amount of credits the 'Earned Consumable' shop mode has.");
        }

        /// <summary>
        /// The purchased items for this profile are taken from the currently active loadout.
        /// </summary>
        /// <returns>A dictionary of item indexes and the quantity of items for that index</returns>
        public System.Collections.Generic.Dictionary<StartingItem, uint> GetStartingItems()
        {
            System.Diagnostics.Debug.Assert(_CorrectLoadouts);

            return _Loadouts[_CurrentLoadoutIndex.Value].GetStartingItems();
        }

        /// <summary>
        /// Purchase and item for the currently active loadout.
        /// The credits are only 'used' for the currently active loadout.
        /// Upon switching to a different loadout, the respective credits amount will be interactable.
        /// Keep in mind: This does not take into consideration the price of the item. To ensure that no funky business is going on, you should check if the player has enough credits for the purchase BEFORE calling this function.
        /// </summary>
        /// <param name="itemIndex">The ItemIndex of the item to buy</param>
        /// <param name="quantity">The quantity of item</param>
        public void PurchaseItem(StartingItem startingItem, uint quantity)
        {
            Log.LogDebug($"Purchasing item: {startingItem} for loadout: {_CurrentLoadoutIndex.Value}");
            System.Diagnostics.Debug.Assert(_CorrectLoadouts);

            _Loadouts[_CurrentLoadoutIndex.Value].AddStartingItem(startingItem, quantity);
        }

        public void SellItem(StartingItem startingItem, uint quantity)
        {
            System.Diagnostics.Debug.Assert(_CorrectLoadouts);

            _Loadouts[_CurrentLoadoutIndex.Value].RemoveStartingItem(startingItem, quantity);
        }

        /// <summary>
        /// Add credits to the current Profile.
        /// </summary>
        /// <param name="points">How many credits to add</param>
        public void AddCredits(uint credits)
        {
            switch (ShopMode)
            {
                case Enums.ShopMode.EarnedPersistent:
                    _EarnedPersistentCurrentCredits.Value += credits;
                    break;
            }
            _Credits.Value += credits;
        }

        /// <summary>
        /// Remove credits from the current Profile.
        /// Keep in mind: This function does not check how many credits the player currently has.
        /// </summary>
        /// <param name="credits">How many credits to remove</param>
        public void RemoveCredits(uint credits)
        {
            // Let's just ensure we have enough credits.
            // Keep in mind: This is only executed in debug builds so the warning in this function's summary still applies.
            System.Diagnostics.Debug.Assert(_Credits.Value >= credits);

            _Credits.Value -= credits;
        }
    }
}
