using UnityEngine;

namespace StartingItemsGUI
    {
    public class DataFree : MonoBehaviour
    {
        public static void BuyItem(StartingItem startingItem, uint quantity)
        {
            Log.LogDebug($"Buying item: {startingItem} for Free mode.");
            StartingItemsGUI.Instance.CurrentProfile.PurchaseItem(startingItem, quantity);
            UIDrawer.Refresh();
        }

        public static void SellItem(StartingItem startingItem, uint quantity)
        {
            bool soldItem = false;
            uint counter;
            for (counter = 0; counter < quantity; counter++)
            {
                if (!StartingItemsGUI.Instance.CurrentProfile.GetStartingItems().ContainsKey(startingItem))
                {
                    break;
                }

                if (StartingItemsGUI.Instance.CurrentProfile.GetStartingItems()[startingItem] <= counter)
                {
                    break;
                }

                soldItem = true;
            }
            if (soldItem)
            {
                StartingItemsGUI.Instance.CurrentProfile.SellItem(startingItem, counter);
                UIDrawer.Refresh();
            }
        }
    }
}
