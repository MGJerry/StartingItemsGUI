using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StartingItemsGUI
{
    public class ButtonCreator : MonoBehaviour
    {
        static public GameObject SpawnBlueButton(GameObject parent, Vector2 givenPivot, Vector2 givenSize, string givenLabel, TMPro.TextAlignmentOptions alignment, List<Image> images, bool isFallback = false)
        {
            ColorBlock colourBlock = new();
            colourBlock.disabledColor = new(0.255f, 0.201f, 0.201f, 0.714f);
            colourBlock.highlightedColor = new(0.988f, 1.000f, 0.693f, 0.733f);
            colourBlock.normalColor = new(0.327f, 0.403f, 0.472f, 1.000f);
            colourBlock.pressedColor = new(0.740f, 0.755f, 0.445f, 0.984f);
            colourBlock.colorMultiplier = 1;

            var button = ElementCreator.SpawnButtonSize(parent, Resources.panelTextures[0], colourBlock, givenPivot, givenSize, isFallback);
            ElementCreator.SpawnImageOffset(images, button, Resources.panelTextures[7], new(1, 1, 1, 1), new(0.5f, 0.5f), new Vector2(-6, -6), new(6, 6));
            images[images.Count - 1].gameObject.SetActive(false);
            ElementCreator.SpawnImageOffset(new List<Image>(), button, Resources.panelTextures[1], new(1, 1, 1, 0.286f), new(0.5f, 0.5f), new(0, 0), new(0, 0));
            Image highlightImage = ElementCreator.SpawnImageOffset(new(), button, Resources.panelTextures[2], new(1, 1, 1, 1), new(0.5f, 0.5f), new(-4, -12), new(12, 4));
            button.GetComponent<RoR2.UI.HGButton>().imageOnHover = highlightImage;
            List<TMPro.TextMeshProUGUI> buttonText = new();
            ElementCreator.SpawnTextOffset(buttonText, button, new(1, 1, 1, 1), 24, 0, new(12, 4), new(-12, -4));
            buttonText[0].alignment = alignment;
            buttonText[0].text = givenLabel;
            buttonText[0].lineSpacing = -25;

            return button;
        }

        public static GameObject SpawnBlackButton(GameObject parent, Vector2 givenSize, string givenLabel, List<TMPro.TextMeshProUGUI> texts, bool isFallback = false)
        {
            ColorBlock colourBlockA = new();
            colourBlockA.disabledColor = new(1, 1, 1, 1);
            colourBlockA.highlightedColor = new(1, 1, 1, 1);
            colourBlockA.normalColor = new(1, 1, 1, 1);
            colourBlockA.pressedColor = new(1, 1, 1, 1);
            colourBlockA.colorMultiplier = 1;

            ColorBlock colourBlockB = new();
            colourBlockB.disabledColor = new(0.094f, 0.094f, 0.094f, 0.286f);
            colourBlockB.highlightedColor = new(0.300f, 0.300f, 0.300f, 1.00f);
            colourBlockB.normalColor = new(0.300f, 0.300f, 0.300f, 1.00f);
            colourBlockB.pressedColor = new(0.500f, 0.500f, 0.500f, 1.000f);
            colourBlockB.colorMultiplier = 1;

            var buttonA = ElementCreator.SpawnButtonSize(parent, Resources.panelTextures[4], colourBlockA, new(0, 1), givenSize);
            var buttonB = ElementCreator.SpawnButtonOffset(buttonA, Resources.panelTextures[3], colourBlockB, isFallback);

            buttonA.GetComponent<RoR2.UI.HGButton>().interactable = false;
            var highlightImageA = ElementCreator.SpawnImageOffset(new(), buttonA, Resources.panelTextures[2], new(1, 1, 1, 1), new(0.5f, 0.5f), new(-4, -12), new(12, 4));
            buttonA.GetComponent<RoR2.UI.HGButton>().imageOnHover = buttonB.GetComponent<Image>();
            buttonB.GetComponent<RoR2.UI.HGButton>().imageOnHover = highlightImageA;
            ElementCreator.SpawnTextOffset(texts, buttonA, new(1, 1, 1, 1), 24, 0, new(12, 4), new(-12, -4));
            texts[0].text = givenLabel;
            return buttonB;
        }

        static public RectTransform SpawnItemButton(GameObject root, int textCount, StartingItem startingItem, Dictionary<StartingItem, List<Image>> images, Dictionary<StartingItem, List<TMPro.TextMeshProUGUI>> texts, bool isFallback = false) {
            Log.LogInfo($"Spawning an item button for: {startingItem}");
            images.Add(startingItem, new());
            texts.Add(startingItem, new());

            ColorBlock colourBlockA = new();
            colourBlockA.colorMultiplier = 1;
            colourBlockA.disabledColor = new(0, 0, 0, 0);
            colourBlockA.highlightedColor = new(0, 0, 0, 0);
            colourBlockA.normalColor = new(0, 0, 0, 0);
            colourBlockA.pressedColor = new(0, 0, 0, 0);

            Vector2 size = new();
            size.x = UIConfig.itemButtonWidth + UIConfig.itemPaddingOuter * 2;
            size.y = UIConfig.itemButtonWidth + textCount * UIConfig.itemTextHeight + UIConfig.itemPaddingOuter * 2;
            var item = ElementCreator.SpawnButtonSize(root, null, colourBlockA, new(0, 1), size, isFallback);
            var scaler = ElementCreator.SpawnImageOffset(new(), item, null, new(0, 0, 0, 0), new(0, 1), new(UIConfig.itemPaddingOuter, UIConfig.itemPaddingOuter), new(-UIConfig.itemPaddingOuter, -UIConfig.itemPaddingOuter)).gameObject;

            ElementCreator.SpawnImageOffset(new(), scaler, Resources.panelTextures[1], new(0.286f, 0.286f, 0.286f, 1), new(0, 1), new(-2, textCount * UIConfig.itemTextHeight - 2), new(2, 2));
            for (var textIndex = 0; textIndex < textCount; textIndex++) {
                ElementCreator.SpawnImageOffset(new(), scaler, Resources.panelTextures[1], new(0.286f, 0.286f, 0.286f, 1), new(0, 1), new(-2, textIndex * UIConfig.itemTextHeight - 2), new(2, -UIConfig.itemButtonWidth - (textCount - 1 - textIndex) * UIConfig.itemTextHeight + UIConfig.itemPaddingInner + 2));
                ElementCreator.SpawnImageOffset(new(), scaler, Resources.panelTextures[0], new(0.120f, 0.120f, 0.120f, 1), new(0, 1), new(-2, textIndex * UIConfig.itemTextHeight - 2), new(2, -UIConfig.itemButtonWidth - (textCount - 1 - textIndex) * UIConfig.itemTextHeight + UIConfig.itemPaddingInner + 2));
            }

            ElementCreator.SpawnImageOffset(images[startingItem], scaler, Resources.tierTextures[(int)Data.GetItemTier(startingItem)], new(1, 1, 1), new(0, 1), new(UIConfig.itemPaddingInner, textCount * UIConfig.itemTextHeight + UIConfig.itemPaddingInner), new(-UIConfig.itemPaddingInner, -UIConfig.itemPaddingInner));
            Sprite itemImage = null;
            if (startingItem.IsItemIndex)
            {
                itemImage = RoR2.ItemCatalog.GetItemDef(startingItem.ItemIndex).pickupIconSprite;
            }
            else if (startingItem.IsEquipmentIndex)
            {
                itemImage = RoR2.EquipmentCatalog.GetEquipmentDef(startingItem.EquipmentIndex).pickupIconSprite;
            }
            ElementCreator.SpawnImageOffset(images[startingItem], scaler, itemImage, new(1, 1, 1), new(0, 1), new(UIConfig.itemPaddingInner, textCount * UIConfig.itemTextHeight + UIConfig.itemPaddingInner), new(-UIConfig.itemPaddingInner, -UIConfig.itemPaddingInner));

            ElementCreator.SpawnImageOffset(images[startingItem], scaler, Resources.panelTextures[1], new(0.988f, 1.000f, 0.693f, 0.733f), new(0.5f, 0), new(1, textCount * UIConfig.itemTextHeight + 1), new(-1, -1));
            ElementCreator.SpawnImageSize(images[startingItem], images[startingItem][images[startingItem].Count - 1].gameObject, Resources.panelTextures[6], new(0.988f, 1.000f, 0.693f, 0.733f),new(0.5f, 0.5f), new(20, 20), new(0, 3, 0));

            for (var textIndex = 0; textIndex < textCount; textIndex++)
            {
                ElementCreator.SpawnTextOffset(texts[startingItem], scaler, new(1, 1, 1), 24, 1, new(UIConfig.itemPaddingInner, textIndex * UIConfig.itemTextHeight + UIConfig.itemPaddingInner), new(-UIConfig.itemPaddingInner, -UIConfig.itemButtonWidth - (textCount - 1 - textIndex) * UIConfig.itemTextHeight - UIConfig.itemPaddingInner));
                texts[startingItem][texts[startingItem].Count - 1].text = "";
            }

            ColorBlock colourBlockB = new();
            colourBlockB.colorMultiplier = 1;
            colourBlockB.disabledColor = new(1, 1, 1, 1);
            colourBlockB.highlightedColor = new(1, 1, 1, 1);
            colourBlockB.normalColor = new(1, 1, 1, 1);
            colourBlockB.pressedColor = new(0.8f, 0.8f, 0.8f, 1);

            /*
            GameObject highlight = ElementCreator.SpawnButtonOffset(item, Resources.resources.panelTextures[5], colourBlockB);
            highlight.GetComponent<RoR2.UI.HGButton>().showImageOnHover = false;
            RectTransform highlightTransform = highlight.GetComponent<RectTransform>();
            highlightTransform.offsetMin = new Vector2(-5, -5);
            highlightTransform.offsetMax = new Vector2(5, 5);
            item.GetComponent<RoR2.UI.HGButton>().imageOnHover = highlight.GetComponent<Image>();
            */

            var highlight = ElementCreator.SpawnImageOffset(new(), scaler, Resources.panelTextures[5], new(1, 1, 1, 1), new(0.5f, 0.5f), new(-5, -5), new(5, 5));
            item.GetComponent<RoR2.UI.HGButton>().imageOnHover = highlight.GetComponent<Image>();

            var tooltipProvider = item.AddComponent<RoR2.UI.TooltipProvider>();
            tooltipProvider.titleColor = Resources.colours[(int)Data.GetItemTier(startingItem)];
            tooltipProvider.bodyColor = Resources.colours[6];

            if (startingItem.IsItemIndex)
            {
                var itemDef = RoR2.ItemCatalog.GetItemDef(startingItem.ItemIndex);
                tooltipProvider.titleToken = itemDef.nameToken;
                tooltipProvider.bodyToken = itemDef.descriptionToken;
            }
            else if (startingItem.IsEquipmentIndex)
            {
                var equipmentDef = RoR2.EquipmentCatalog.GetEquipmentDef(startingItem.EquipmentIndex);
                tooltipProvider.titleToken = equipmentDef.nameToken;
                tooltipProvider.bodyToken = equipmentDef.descriptionToken;
            }

            var pointerClick = item.AddComponent<PointerClick>();
            pointerClick.eventSystem = item.GetComponent<RoR2.UI.MPEventSystemLocator>().eventSystem;
            pointerClick.onLeftClick.AddListener(() =>
                {
                    Log.LogDebug($"Click Listener: Left click for item: {startingItem}");
                    Data.LeftClick(startingItem, Data.buyMultiplier);
                });

                pointerClick.onRightClick.AddListener(() =>
                {
                    Log.LogDebug($"Click Listener: Right click for item: {startingItem}");
                    Data.RightClick(startingItem, Data.buyMultiplier);
                }
            );

            return item.GetComponent<RectTransform>();
        }
    }
}
