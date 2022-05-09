﻿using UnityEngine;

namespace StartingItemsGUI
{
    public class UIDrawerFree : MonoBehaviour
    {
        public static void Refresh()
        {
            UIDrawer.pointText.text = "";

            foreach (var startingItem in UIDrawer.itemTexts.Keys)
            {
                if (StartingItemsGUI.Instance.CurrentProfile.GetStartingItems().ContainsKey(startingItem))
                {
                    UIDrawer.itemTexts[startingItem][0].text = StartingItemsGUI.Instance.CurrentProfile.GetStartingItems()[startingItem].ToString();

                    for (var imageIndex = 0; imageIndex < 2; imageIndex++)
                    {
                        UIDrawer.itemImages[startingItem][imageIndex].color = UIConfig.enabledColor;
                    }
                    for (var imageIndex = 0; imageIndex < 2; imageIndex++)
                    {
                        UIDrawer.itemImages[startingItem][imageIndex + 2].gameObject.SetActive(true);
                    }
                    foreach (var text in UIDrawer.itemTexts[startingItem])
                    {
                        text.color = UIConfig.enabledColor;
                    }
                }
                else
                {
                    UIDrawer.itemTexts[startingItem][0].text = "0";
                    for (var imageIndex = 0; imageIndex < 2; imageIndex++)
                    {
                        UIDrawer.itemImages[startingItem][imageIndex].color = UIConfig.disabledColor;
                    }
                    for (var imageIndex = 0; imageIndex < 2; imageIndex++)
                    {
                        UIDrawer.itemImages[startingItem][imageIndex + 2].gameObject.SetActive(false);
                    }
                    foreach (var text in UIDrawer.itemTexts[startingItem])
                    {
                        text.color = UIConfig.disabledColor;
                    }
                }
            }
        }
    }
}
