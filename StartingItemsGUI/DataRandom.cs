using System.Linq;
using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace StartingItemsGUI
{
    public class DataRandom : MonoBehaviour
    {
        // Rework this.
        static private List<int> itemCountRange = new() { 10, 5, 2, 4, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static Dictionary<StartingItem, uint> GenerateRandomItemList()
        {
            List<StartingItem> storeItems = UIDrawer.GetStoreItems();
            var itemPrices = new Dictionary<StartingItem, uint>();
            List<uint> allPrices = new()
            {
                ConfigManager.TierOnePrice.Value,
                ConfigManager.TierTwoPrice.Value,
                ConfigManager.TierThreePrice.Value,
                ConfigManager.BossPrice.Value,
                ConfigManager.LunarPrice.Value,
                ConfigManager.EquipmentPrice.Value,
                ConfigManager.LunarEquipmentPrice.Value,
                ConfigManager.EliteEquipmentPrice.Value
            };
            List<uint> equipmentPrices = new()
            {
                ConfigManager.EquipmentPrice.Value,
                ConfigManager.LunarEquipmentPrice.Value,
                ConfigManager.EliteEquipmentPrice.Value
            };

            foreach (var startingItem in storeItems)
            {
                var itemPrice = Data.GetStartingItemPrice(startingItem);
                itemPrices.Add(startingItem, itemPrice);
            }

            uint points = 1000; // Let's change this in the future.
            Dictionary<StartingItem, uint> itemsPurchased = new();
            bool equipmentGiven = false;
            var random = new System.Random();

            ReduceItemList(points, equipmentGiven, allPrices, equipmentPrices, itemPrices);
            while (allPrices.Count > 0) {
                var availableItems = itemPrices.Keys.ToList();
                var nextItem = availableItems[random.Next(availableItems.Count)];
                if (!itemsPurchased.ContainsKey(nextItem)) {
                    itemsPurchased.Add(nextItem, 0);
                }
                uint itemPrice = Data.GetStartingItemPrice(nextItem);
                uint itemsGiven = (uint)random.Next(1, Mathf.Min(Mathf.FloorToInt(points / itemPrice) + 1 , GetCountRange(nextItem) + 1));
                itemsPurchased[nextItem] += itemsGiven;
                points -= itemPrice * itemsGiven;
                if (nextItem.IsEquipmentIndex)
                {
                    equipmentGiven = true;
                }
                ReduceItemList(points, equipmentGiven, allPrices, equipmentPrices, itemPrices);
            }

            return itemsPurchased;
        }

        /// <summary>
        /// ? TODO: Figure out wtf this mystery function does.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="equipmentGiven"></param>
        /// <param name="allPrices"></param>
        /// <param name="equipmentPrices"></param>
        /// <param name="itemPrices"></param>
        static void ReduceItemList(uint points, bool equipmentGiven, List<uint> allPrices, List<uint> equipmentPrices, Dictionary<StartingItem, uint> itemPrices) {
            bool cullList = false;
            List<int> indexesToRemove = new List<int>();
            for (int priceIndex = 0; priceIndex < allPrices.Count; priceIndex++) {
                if (points < allPrices[priceIndex]) {
                    indexesToRemove.Add(priceIndex);
                    cullList = true;
                }
            }

            indexesToRemove.Reverse();
            foreach (int indexToRemove in indexesToRemove) {
                allPrices.RemoveAt(indexToRemove);
            }
            if (equipmentGiven) {
                foreach (var equipmentPrice in equipmentPrices) {
                    if (allPrices.Contains(equipmentPrice)) {
                        allPrices.Remove(equipmentPrice);
                        cullList = true;
                    }
                }
            }

            if (cullList) {
                List<StartingItem> itemIDsOld = new();
                foreach (var itemID in itemPrices.Keys)
                {
                    itemIDsOld.Add(itemID);
                }
                foreach (var itemID in itemIDsOld)
                {
                    if (points < itemPrices[itemID] || (itemID.IsEquipmentIndex && equipmentGiven))
                    {
                        itemPrices.Remove(itemID);
                    }
                }
            }
        }

        static int GetCountRange(StartingItem startingItem)
        {
            if (startingItem.IsEquipmentIndex)
            {
                return itemCountRange[5];
            }
            else
            {
                return itemCountRange[(int)Data.GetItemTier(startingItem)];
            }
        }
    }
}
