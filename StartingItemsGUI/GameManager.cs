using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using R2API.Networking.Interfaces;

namespace StartingItemsGUI
{
    public class Connection : INetMessage {
        public uint _connectionID = 0;

        public void Serialize(NetworkWriter writer) {
            writer.Write(_connectionID);
        }

        public void Deserialize(NetworkReader reader) {
            _connectionID = reader.ReadUInt32();
        }

        public void OnReceived() {
            GameManager.FinalizePurchases(this);
        }
    }

    public class ItemPurchased : INetMessage {
        public System.UInt32 _itemID = 0;
        public uint _itemCount = 0;
        public uint _connectionID = 0;

        public void Serialize(NetworkWriter writer) {
            writer.Write(_itemID);
            writer.Write(_itemCount);
            writer.Write(_connectionID);
        }

        public void Deserialize(NetworkReader reader) {
            _itemID = reader.ReadUInt32();
            _itemCount = reader.ReadUInt32();
            _connectionID = reader.ReadUInt32();
        }

        public void OnReceived() {
            GameManager.ReceiveItem(this);
        }
    }

    public class SpawnItems : INetMessage {
        public Enums.ShopMode _mode = Enums.ShopMode.Uninitialized;
        public uint _connectionID = 0;

        public void Serialize(NetworkWriter writer) {
            writer.Write((int)_mode);
            writer.Write(_connectionID);
        }

        public void Deserialize(NetworkReader reader) {
            _mode = (Enums.ShopMode)reader.ReadInt32();
            _connectionID = reader.ReadUInt32();
        }

        public void OnReceived() {
            GameManager.AttemptSpawnItems(this);
        }
    }

    public class GameManager : MonoBehaviour {
        // Remove these.
        static public Dictionary<uint, List<bool>> status = new();
        static public Dictionary<uint, int> modes = new();
        static public Dictionary<uint, Dictionary<StartingItem, uint>> items = new();
        static public Dictionary<uint, CharacterMaster> characterMasters = new();
        static public List<Coroutine> spawnItemCoroutines = new();

        static public void SetCharacterMaster(uint netId, CharacterMaster characterMaster) {
            characterMasters.Add(netId, characterMaster);
            status[netId][1] = true;
            SpawnItems(netId);
        }

        static public void SendItems(NetworkUser networkUser)
        {
            if (StartingItemsGUI.Instance.ModEnabled)
            {
                var startingItems = StartingItemsGUI.Instance.CurrentProfile.GetStartingItems();

                if (StartingItemsGUI.Instance.CurrentProfile.ShopMode != Enums.ShopMode.Random)
                {
                    foreach (var startingItem in startingItems)
                    {
                        var itemPurchased = new ItemPurchased
                        {
                            _itemID = startingItem.Key.ID,
                            _itemCount = startingItem.Value,
                            _connectionID = networkUser.netId.Value
                        };
                        itemPurchased.Send(R2API.Networking.NetworkDestination.Server);
                    }
                    SpawnItems spawnItems = new(){ _mode = StartingItemsGUI.Instance.CurrentProfile.ShopMode, _connectionID = networkUser.netId.Value };
                    spawnItems.Send(R2API.Networking.NetworkDestination.Server);
                }
            }
        }

        public static void ReceiveItem(ItemPurchased givenItem)
        {
            if (givenItem._itemID != 0 && givenItem._itemCount != 0 && givenItem._connectionID != 0)
            {
                if (!status[givenItem._connectionID][0])
                {
                    if (!items[givenItem._connectionID].ContainsKey(new StartingItem(givenItem._itemID)))
                    {
                        if (Data.ItemExists(new StartingItem(givenItem._itemID)))
                        {
                            items[givenItem._connectionID].Add(new StartingItem(givenItem._itemID), givenItem._itemCount);
                        }
                    }
                }
            }
        }

        static public void AttemptSpawnItems(SpawnItems spawnInfo) {
            if (spawnInfo._mode != Enums.ShopMode.Uninitialized && spawnInfo._connectionID != 0) {
                if (!status[spawnInfo._connectionID][0]) {
                    modes[spawnInfo._connectionID] = (int)spawnInfo._mode;
                    status[spawnInfo._connectionID][0] = true;
                    SpawnItems(spawnInfo._connectionID);
                }
            }
        }

        static public void SpawnItems(uint connectionID)
        {
            if (status[connectionID][0] && status[connectionID][1]) {
                if (!status[connectionID][2]) {
                    status[connectionID][2] = true;
                    if (StartingItemsGUI.Instance.ModEnabled && (int)StartingItemsGUI.Instance.CurrentProfile.ShopMode == modes[connectionID]) {
                        spawnItemCoroutines.Add(StartingItemsGUI.Instance.StartCoroutine(SpawnItemsCoroutine(connectionID)));
                    }
                }
            }
        }

        static System.Collections.IEnumerator SpawnItemsCoroutine(uint connectionID) {
            if (characterMasters[connectionID].GetBody() == null) {
                yield return new UnityEngine.WaitUntil(() => characterMasters[connectionID].GetBody() != null);
            }

            foreach (var itemID in items[connectionID].Keys)
            {
                for (var itemCount = 0; itemCount < items[connectionID][itemID]; itemCount++)
                {
                    if (itemID.IsItemIndex)
                    {
                        characterMasters[connectionID].GetBody().inventory.GiveItem(itemID.ItemIndex);
                    }
                    else if (itemID.IsEquipmentIndex)
                    {
                        characterMasters[connectionID].GetBody().inventory.SetEquipmentIndex(itemID.EquipmentIndex);
                    }
                }
                PickupIndex pickupIndex = new();
                bool pickupCreated = false;
                if (itemID.IsItemIndex)
                {
                    pickupCreated = true;
                    pickupIndex = PickupCatalog.FindPickupIndex(itemID.ItemIndex);
                }
                else if (itemID.IsEquipmentIndex)
                {
                    pickupCreated = true;
                    pickupIndex = PickupCatalog.FindPickupIndex(itemID.EquipmentIndex);
                }

                if (pickupCreated)
                {
                    Chat.AddPickupMessage(characterMasters[connectionID].GetBody(), PickupCatalog.GetPickupDef(pickupIndex).nameToken, PickupCatalog.GetPickupDef(pickupIndex).baseColor, System.Convert.ToUInt32(items[connectionID][itemID]));
                }
            }
            if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.EarnedConsumable) {
                Connection connection = new Connection { _connectionID = connectionID };
                connection.Send(R2API.Networking.NetworkDestination.Clients);
            }
        }

        static public void FinalizePurchases(Connection connection) {
            if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.EarnedConsumable) {
                foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList) {
                    if (networkUser.netId.Value == connection._connectionID) {
                        if (networkUser.isLocalPlayer) {

                        }
                        break;
                    }
                }
            }
        }

        static public void ClearItems()
        {
            status.Clear();
            modes.Clear();
            items.Clear();
            characterMasters.Clear();

            foreach(Coroutine coroutine in spawnItemCoroutines)
            {
                StartingItemsGUI.Instance.StopCoroutine(coroutine);
            }
            spawnItemCoroutines.Clear();
        }
    }
}
