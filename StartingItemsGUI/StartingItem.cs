namespace StartingItemsGUI
{
    [System.Serializable]
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.JsonSerializer))]
    public class StartingItem
    {
        /// <summary>
        /// The offset that let's us differentiate between an Item, or Equipment.
        /// </summary>
        //[field: System.NonSerialized]
        [field: System.Text.Json.Serialization.JsonIgnore]
        private const System.UInt32 _OFFSET = 65535;

        //[field: System.NonSerialized]
        [field: System.Text.Json.Serialization.JsonIgnore]
        private System.UInt32 _ID = 0;

        //[field: System.NonSerialized]
        [field: System.Text.Json.Serialization.JsonIgnore]
        private bool _isItemIndex = false;

        //[field: System.NonSerialized]
        [field: System.Text.Json.Serialization.JsonIgnore]
        private bool _isEquipmentIndex = false;

        //[property: System.Runtime.Serialization.IgnoreDataMember]
        [property: System.Text.Json.Serialization.JsonIgnore]
        public bool IsItemIndex { get { return _isItemIndex; } }

        //[property: System.Runtime.Serialization.IgnoreDataMember]
        [property: System.Text.Json.Serialization.JsonIgnore]
        public bool IsEquipmentIndex { get { return _isEquipmentIndex; } }

        // I wonder if there is a better way of doing this :thinking: Hmm...

        //[property: System.Runtime.Serialization.IgnoreDataMember]
        [property: System.Text.Json.Serialization.JsonIgnore]
        public RoR2.ItemIndex ItemIndex { get { return (RoR2.ItemIndex)_ID; } }

        //[property: System.Runtime.Serialization.IgnoreDataMember]
        [property: System.Text.Json.Serialization.JsonIgnore]
        public RoR2.EquipmentIndex EquipmentIndex { get { return (RoR2.EquipmentIndex)(_ID - _OFFSET); } }

        //[property: UnityEngine.SerializeField]
        //[property: System.Runtime.Serialization.DataMember]
        [property: System.Text.Json.Serialization.JsonInclude]
        public System.UInt32 ID { get { return _ID; } set { _ID = System.UInt32.Parse(value.ToString()); Init(); } }

        /// <summary>
        /// This is an abstraction layer for RoR2.ItemIndex/ItemDef and RoR2.EquipmentIndex/EquipmentDef
        /// Each StartingItem object could either be an RoR2.ItemIndex/ItemDef or RoR2.EquipmentIndex/EquipmentDef under the hood.
        /// This abstracts that away so this is the only object we need to deal with.
        /// </summary>
        public StartingItem(RoR2.ItemIndex itemIndex)
        {
            Log.LogDebug($"Creating Starting Item with item index: {(int)itemIndex}");
            _ID = (System.UInt32)itemIndex;

            Init();
        }

        public StartingItem(RoR2.EquipmentIndex equipmentIndex)
        {
            Log.LogDebug($"Creating Starting Item with equipment index: {(int)equipmentIndex}");
            // This offset is very important. Without it, we wouldn't be able to tell if a StartingItem is an item or equipment.
            _ID = _OFFSET + (System.UInt32)equipmentIndex;

            Init();
        }

        // Keep in mind, this will only word for a uint that's been cast to a string.
        public StartingItem(string itemStr)
        {
            Log.LogDebug($"Creating Starting Item with string ID: {itemStr}");
            _ID = System.UInt32.Parse(itemStr);

            Init();
        }

        public StartingItem(uint itemID)
        {
            Log.LogDebug($"Creating Starting Item with uint ID: {itemID}");
            _ID = itemID;

            Init();
        }

        public StartingItem(int itemID)
        {
            Log.LogDebug($"Creating Starting Item with int ID: {itemID}");
            _ID = (uint)itemID;

            Init();
        }

        private void Init()
        {
            var isItem = _ID < _OFFSET;

            _isItemIndex = isItem;
            _isEquipmentIndex = !_isItemIndex;
        }

        // Let's stick to comparing and hashing the _ID of the object. This should be enough.
        public override bool Equals(object obj)
        {
            return obj is StartingItem item && item._ID == _ID;
        }

        public override int GetHashCode()
        {
            return _ID.GetHashCode();
        }
        public static bool operator ==(StartingItem s1, StartingItem s2) => (s1._ID == s2._ID);
        public static bool operator !=(StartingItem s1, StartingItem s2) => !(s1 == s2);

        // Cheeky method to return an ID, but as a string. That way we can consume it without worrying what actual Item/Equipment it is.
        public override string ToString()
        {
            return _ID.ToString();
        }

        public static StartingItem Parse(string s)
        {
            //var x = s.Split();
            return new StartingItem(s);// { ItemIndex = (ItemIndex)int.Parse(x[0]), EquipmentIndex = (EquipmentIndex)int.Parse(x[1]) };
        }
    }
}
