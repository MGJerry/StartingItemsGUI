using UnityEngine;

namespace StartingItemsGUI
{
    public class UIDrawerRandom : MonoBehaviour
    {
        public static void DrawUI()
        {
            foreach (var startingItem in UIDrawer.itemTexts.Keys)
            {
                UIDrawer.itemTexts[startingItem][0].text = Data.GetStartingItemPrice(startingItem).ToString();
                for (var imageIndex = 0; imageIndex < 2; imageIndex++)
                {
                    UIDrawer.itemImages[startingItem][imageIndex + 2].gameObject.SetActive(false);
                }

                if (StartingItemsGUI.Instance.CurrentProfile.Credits < Data.GetStartingItemPrice(startingItem))
                {
                    UIDrawer.itemTexts[startingItem][0].color = UIConfig.disabledColor;
                    for (var imageIndex = 0; imageIndex < 2; imageIndex++)
                    {
                        UIDrawer.itemImages[startingItem][imageIndex].color = UIConfig.disabledColor;
                    }
                }

            }

            UIDrawer.pointText.text = $"CREDITS: {StartingItemsGUI.Instance.CurrentProfile.Credits} ¢";
        }
    }
}
