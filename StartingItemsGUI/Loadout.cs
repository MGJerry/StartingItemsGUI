namespace StartingItemsGUI
{
    class Loadout
    {
        // Let's save this as a JSON string as BepInEx ConfigEntry doesn't support lists.
        private BepInEx.Configuration.ConfigEntry<string> _ConfigStartingItems { get; set; }
        [System.Text.Json.Serialization.JsonConverter(typeof(StartingItemsJsonFactory))]
        private System.Collections.Generic.Dictionary<StartingItem, System.UInt32> _StartingItems = new();

        /// <summary>
        /// Loadout class
        /// </summary>
        /// <param name="configFile">The configuration file which will be read from and bound to.</param>
        /// <param name="index">ID</param>
        public Loadout(BepInEx.Configuration.ConfigFile configFile, uint index)
        {
            Log.LogDebug("We are in Loadout.");
            // Let's follow what a .toml file would do, and slap on an index on the section name, so that each loadout has a different config inside the same file.
            var loadoutSection = $"Loadouts.{index}";

            _ConfigStartingItems = configFile.Bind(loadoutSection, "StartingItems", System.Text.Json.JsonSerializer.Serialize(new System.Collections.Generic.Dictionary<StartingItem, System.UInt32>(), StartingItemsGUI.Instance.JsonFactory), "A JSON of starting items for this loadout.");
            _StartingItems = JSONToItems(_ConfigStartingItems.Value);
        }

        public System.Collections.Generic.Dictionary<StartingItem, System.UInt32> GetStartingItems()
        {
            return _StartingItems;
        }

        public void AddStartingItem(StartingItem startingItem, System.UInt32 quantity)
        {
            Log.LogDebug($"Adding starting item with raw ID: {startingItem}");
            if (_StartingItems.ContainsKey(startingItem))
            {
                _StartingItems[startingItem] += quantity;
            }
            else
            {
                _StartingItems[startingItem] = quantity;
            }

            Log.LogDebug($"Saving items JSON to config");
            _ConfigStartingItems.Value = ItemsToJSON();
        }

        public void RemoveStartingItem(StartingItem startingItem, System.UInt32 quantity)
        {
            Log.LogDebug($"Removing {quantity} starting items with ID: {startingItem}");

            if (!_StartingItems.ContainsKey(startingItem))
            {
                return;
            }

            if (_StartingItems[startingItem] > quantity)
            {
                _StartingItems[startingItem] -= quantity;
            }
            else
            {
                _StartingItems.Remove(startingItem);
            }

            _ConfigStartingItems.Value = ItemsToJSON();
        }

        private string ItemsToJSON()
        {
            Log.LogDebug("Calling ItemsToJSON");
            
            var json = System.Text.Json.JsonSerializer.Serialize(_StartingItems, StartingItemsGUI.Instance.JsonFactory);
            return json;
        }

        private System.Collections.Generic.Dictionary<StartingItem, System.UInt32> JSONToItems(string json)
        {
            Log.LogDebug($"Calling JSONToItems: {json}");
            var items = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<StartingItem, System.UInt32>>(json, StartingItemsGUI.Instance.JsonFactory);
            Log.LogDebug($"Received {items.Count} items from Config file.");
            return items;
        }
    }
}
