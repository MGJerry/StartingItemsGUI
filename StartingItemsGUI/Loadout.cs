namespace StartingItemsGUI
{
    class Loadout
    {
        // Let's save this as a JSON string as BepInEx ConfigEntry doesn't support lists.
        private BepInEx.Configuration.ConfigEntry<string> _ConfigStartingItems { get; set; }
        [System.Text.Json.Serialization.JsonConverter(typeof(StartingItemsJsonFactory))]
        private System.Collections.Generic.Dictionary<StartingItem, uint> _StartingItems = new();

        /// <summary>
        /// Loadout class
        /// </summary>
        /// <param name="configFile">The configuration file which will be read from and bound to.</param>
        /// <param name="index">ID</param>
        public Loadout(BepInEx.Configuration.ConfigFile configFile, uint index)
        {
            // Let's follow what a .toml file would do, and slap on an index on the section name, so that each loadout has a different config inside the same file.
            var loadoutSection = $"Loadouts.{index}";

            //_ConfigStartingItems = configFile.Bind(loadoutSection, "StartingItems", Newtonsoft.Json.JsonConvert.SerializeObject(new System.Collections.Generic.Dictionary<StartingItem, uint>()), "A JSON of starting items for this loadout.");
            _ConfigStartingItems = configFile.Bind(loadoutSection, "StartingItems", System.Text.Json.JsonSerializer.Serialize(new System.Collections.Generic.Dictionary<StartingItem, uint>()), "A JSON of starting items for this loadout.");
            _StartingItems = JSONToItems(_ConfigStartingItems.Value);
        }

        public System.Collections.Generic.Dictionary<StartingItem, uint> GetStartingItems()
        {
            return _StartingItems;
        }

        public void AddStartingItem(StartingItem startingItem, uint quantity)
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
            var item = new StartingItem(100);
            Log.LogDebug($"StartingItem 100: {item}");
            //Log.LogDebug($"Serialized StartingItem 100: {Newtonsoft.Json.JsonConvert.SerializeObject(item)}");
            Log.LogDebug($"Serialized StartingItem 100: {System.Text.Json.JsonSerializer.Serialize(item)}");
            Log.LogDebug("Finishing method call.");
            Log.LogDebug($"Current Value: {_ConfigStartingItems.Value}");
            _ConfigStartingItems.Value = ItemsToJSON();
            Log.LogDebug($"New Value: {_ConfigStartingItems.Value}");
        }

        public void RemoveStartingItem(StartingItem startingItem, uint quantity)
        {
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
            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(_StartingItems);
            var json = System.Text.Json.JsonSerializer.Serialize(_StartingItems);
            Log.LogDebug($"{json}");
            return json;
        }

        private System.Collections.Generic.Dictionary<StartingItem, uint> JSONToItems(string json)
        {
            Log.LogDebug($"Calling JSONToItems: {json}");
            //var items = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<StartingItem, uint>>(json);
            var items = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<StartingItem, uint>>(json);
            Log.LogDebug($"Received {items.Count} items from Config file.");
            return items;
        }
    }
}
