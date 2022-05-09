using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace StartingItemsGUI
{
    public class Data : MonoBehaviour
    {
        public static readonly Dictionary<Enums.ShopMode, bool> ShopModes = new()
        {
            { Enums.ShopMode.EarnedConsumable, ConfigManager.EarnedConsumableModeEnabled.Value },
            { Enums.ShopMode.EarnedPersistent, ConfigManager.EarnedPersistentModeEnabled.Value },
            { Enums.ShopMode.Free, ConfigManager.FreeModeEnabled.Value },
            { Enums.ShopMode.Random, ConfigManager.RandomModeEnabled.Value }
        };

        public static Dictionary<Enums.ShopMode, string> ShopModeNames = new()
        {
            { Enums.ShopMode.EarnedConsumable, "Earned Consumable" },
            { Enums.ShopMode.EarnedPersistent, "Earned Persistent" },
            { Enums.ShopMode.Free, "Free" },
            { Enums.ShopMode.Random, "Random" }
        };

        public static uint buyMultiplier = 1;
        public static uint buyMultiplierMax = 1000;
        
        // Let's bring this back in the future so that we have correct split-screen support.
        //static public List<string> localUsers = new List<string>();

        public static bool ItemExists(StartingItem startingItem)
        {
            if (startingItem.IsItemIndex)
            {
                var itemDef = RoR2.ItemCatalog.GetItemDef(startingItem.ItemIndex);
                return (itemDef.pickupIconSprite != null && itemDef.pickupIconSprite.name != "texNullIcon" && itemDef.tier != ItemTier.NoTier);
            }
            else if (startingItem.IsEquipmentIndex)
            {
                var equipmentDef = RoR2.EquipmentCatalog.GetEquipmentDef(startingItem.EquipmentIndex);
                return (equipmentDef.pickupIconSprite != null && equipmentDef.pickupIconSprite.name != "texNullIcon");
            }

            return false;
        }

        public static bool UnlockedItem(StartingItem startingItem)
        {
            if (!ItemExists(startingItem))
            {
                if (startingItem.IsItemIndex)
                {
                    Log.LogDebug($"Item with ID {startingItem.ItemIndex} doesn't exist.");
                }
                else if (startingItem.IsEquipmentIndex)
                {
                    Log.LogDebug($"Equipment with ID {startingItem.EquipmentIndex} doesn't exist.");
                }
                else
                {
                    Log.LogDebug($"Something went wrong. This code in theroy, should never be reached. Incorrect StartingItem ID given.");
                }
                return false;
            }

            if (StartingItemsGUI.Instance.ShowAllItems)
            {
                return true;
            }

            if (startingItem.IsItemIndex)
            {
                var unlockableDef = RoR2.ItemCatalog.GetItemDef(startingItem.ItemIndex).unlockableDef;
                var pickup = RoR2.PickupCatalog.FindPickupIndex(startingItem.ItemIndex);

                return StartingItemsGUI.Instance.CurrentProfile.UserProfile.HasUnlockable(unlockableDef) && StartingItemsGUI.Instance.CurrentProfile.UserProfile.HasDiscoveredPickup(pickup);
            }
            else if (startingItem.IsEquipmentIndex)
            {
                var unlockableDef = RoR2.EquipmentCatalog.GetEquipmentDef(startingItem.EquipmentIndex).unlockableDef;
                var pickup = RoR2.PickupCatalog.FindPickupIndex(startingItem.EquipmentIndex);

                return StartingItemsGUI.Instance.CurrentProfile.UserProfile.HasUnlockable(unlockableDef) && StartingItemsGUI.Instance.CurrentProfile.UserProfile.HasDiscoveredPickup(pickup);
            }
            return false;
        }

        public static uint GetStartingItemPrice(StartingItem startingItem)
        {
            if (startingItem.IsItemIndex)
            {
                if (RoR2.ItemCatalog.tier1ItemList.Contains(startingItem.ItemIndex))
                {
                    return ConfigManager.TierOnePrice.Value;
                }
                else if (RoR2.ItemCatalog.tier2ItemList.Contains(startingItem.ItemIndex))
                {
                    return ConfigManager.TierTwoPrice.Value;
                }
                else if (RoR2.ItemCatalog.tier3ItemList.Contains(startingItem.ItemIndex))
                {
                    return ConfigManager.TierThreePrice.Value;
                }
                else if (RoR2.ItemCatalog.lunarItemList.Contains(startingItem.ItemIndex))
                {
                    return ConfigManager.LunarPrice.Value;
                }
                else
                {
                    // For now, a Boss Item is defined as an item that exists, but is not present in any of the above lists.
                    return ConfigManager.BossPrice.Value;
                }
            }
            else if (startingItem.IsEquipmentIndex)
            {
                if (RoR2.EquipmentCatalog.GetEquipmentDef(startingItem.EquipmentIndex).isLunar)
                {
                    return ConfigManager.LunarEquipmentPrice.Value;
                }
                else if (!RoR2.EquipmentCatalog.equipmentList.Contains(startingItem.EquipmentIndex))
                {
                    return ConfigManager.EliteEquipmentPrice.Value;
                }
                else if (RoR2.EquipmentCatalog.equipmentList.Contains(startingItem.EquipmentIndex))
                {
                    return ConfigManager.EquipmentPrice.Value;
                }
            }
            return ConfigManager.DefaultItemPrice.Value;
        }

        public static uint GetItemTier(StartingItem startingItem)
        {
            if (startingItem.IsItemIndex)
            {
                return ((uint)RoR2.ItemCatalog.GetItemDef(startingItem.ItemIndex).tier);
            }
            else if (startingItem.IsEquipmentIndex)
            {
                var equipmentDef = RoR2.EquipmentCatalog.GetEquipmentDef(startingItem.EquipmentIndex);
                if (equipmentDef.isLunar)
                {
                    return 5;
                }
                else if (equipmentDef.isBoss)
                {
                    return 6;
                }
                return 4;
            }
            else
            {
                return 7;
            }
        }

        public static void LeftClick(StartingItem startingItem, uint quantity)
        {
            Log.LogDebug($"Left clicking item with raw ID: {startingItem}");
            switch(StartingItemsGUI.Instance.CurrentProfile.ShopMode)
            {
                case Enums.ShopMode.EarnedConsumable:
                    //DataEarnedConsumable.BuyItem(startingItem, quantity);
                    break;
                case Enums.ShopMode.EarnedPersistent:
                    //DataEarnedPersistent.BuyItem(startingItem, quantity);
                    break;
                case Enums.ShopMode.Free:
                    DataFree.BuyItem(startingItem, quantity);
                    break;
            }
        }

        public static void RightClick(StartingItem startingItem, uint quantity)
        {
            Log.LogDebug($"Right clicking item with raw ID: {startingItem}");
            switch (StartingItemsGUI.Instance.CurrentProfile.ShopMode)
            {
                case Enums.ShopMode.EarnedConsumable:
                    DataEarnedConsumable.SellItem(startingItem, quantity);
                    break;
                case Enums.ShopMode.EarnedPersistent:
                    DataEarnedPersistent.SellItem(startingItem, quantity);
                    break;
                case Enums.ShopMode.Free:
                    DataFree.SellItem(startingItem, quantity);
                    break;
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static List<StartingItem> SortItems(List<StartingItem> givenItems)
        {
            // TODO: Change this.
            givenItems.Sort((a, b) =>
            {
                return a.ID >= b.ID ? 1 : 0;
            });
            return givenItems;
        }

        public static void ToggleBuyMultiplier()
        {
            buyMultiplier *= 10;
            if (buyMultiplier > buyMultiplierMax)
            {
                buyMultiplier = 1;
            }
            UIDrawer.Refresh();
        }

        public static void SetMode(Enums.ShopMode mode)
        {
            if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == mode)
            {
                return;
            }
            var oldMode = StartingItemsGUI.Instance.CurrentProfile.ShopMode;
            StartingItemsGUI.Instance.CurrentProfile.ShopMode = mode;
            ChangeMenu(oldMode, mode);
        }

        static void ChangeMenu(Enums.ShopMode oldMode, Enums.ShopMode newMode)
        {
            if ((UIConfig.storeRows[(int)oldMode] == UIConfig.storeRows[(int)newMode]) && (UIConfig.textCount[(int)oldMode] == UIConfig.textCount[(int)newMode]) && (oldMode != Enums.ShopMode.Random) && (newMode != Enums.ShopMode.Random))
            {
                UIDrawer.Refresh();
            }
            else
            {
                UIDrawer.DrawUI();
            }
        }

        public static float GetEclipseScalingValueAdd(Run run)
        {
            if ((int)run.selectedDifficulty < (int)RoR2.DifficultyIndex.Eclipse1)
            {
                return 0.0f;
            }

            var eclipse = (uint)run.selectedDifficulty - ((uint)RoR2.DifficultyIndex.Eclipse1 - 1);
            // Let's default this to IDK... 1.5 for now.
            // TODO: Add config option for this.
            //return eclipse / 8f * eclipse8ScalingValue;
            return (eclipse / 8f) * 1.5f;
        }
    }
}
