﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RoR2;

namespace StartingItemsGUI
    {
    public class UIDrawer : MonoBehaviour
        {
        static public RectTransform rootTransform;
        static public List<GameObject> shopInterfaces = new();
        static public RoR2.UI.MainMenu.BaseMainMenuScreen startingItems;

        static List<List<Image>> modeImages = new();
        static List<List<Image>> loadoutImages = new();
        static public TMPro.TextMeshProUGUI pointText;
        static public Dictionary<StartingItem, List<Image>> itemImages = new();
        static public Dictionary<StartingItem, List<TMPro.TextMeshProUGUI>> itemTexts = new();
        static List<TMPro.TextMeshProUGUI> statusTexts = new();
        static public TMPro.TextMeshProUGUI multiplierText;
        static float storeHeight = 0;
        static public bool destroyButton = false;
        static GameObject backButton;
        static Dictionary<int, RoR2.UI.HGButton> modeButtons = new();
        static List<TMPro.TextMeshProUGUI> instructionsText = new();
        static List<string> instructions = new() {
            "LEFT CLICK to add - RIGHT CLICK to remove",
            "LEFT CLICK to buy - RIGHT CLICK to refund\nDEFEAT BOSSES to earn credits",
            "LEFT CLICK to buy - RIGHT CLICK to refund\nCLEAR STAGES to earn credits",
            "LEFT CLICK to buy - RIGHT CLICK to refund\nFINISH GAMES to earn credits",
            "Items will be randomly assigned to you\nwhen you are deployed",
        };

        static public void DrawUI() {
            ResetRootPivot();
            ClearShopInterfaces();
            CalculateVerticalOffset();
            DrawShading();
            DrawBlueButtons();
            DrawPoints();
            DrawShop();
            DrawBlackButtons();
            DrawInstructions();
            DrawModName();
            if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.EarnedConsumable) {
                UIDrawerEarnedConsumable.DrawUI();
            } else if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.EarnedPersistent) {
                UIDrawerEarnedPersistent.DrawUI();
            } else if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.Random) {
                UIDrawerRandom.DrawUI();
            }
            Refresh();
        }

        static void CalculateVerticalOffset() {
            storeHeight = (UIConfig.itemButtonWidth + UIConfig.textCount[(int)StartingItemsGUI.Instance.CurrentProfile.ShopMode] * UIConfig.itemTextHeight + UIConfig.itemPaddingOuter * 2) * UIConfig.storeRows[(int)StartingItemsGUI.Instance.CurrentProfile.ShopMode] + UIConfig.scrollPadding * 2 + UIConfig.panelPadding * 2;
            float totalHeight = UIConfig.blueButtonHeight + UIConfig.spacingVertical + storeHeight + UIConfig.spacingVertical + UIConfig.blackButtonHeight;
            UIConfig.offsetVertical = (rootTransform.rect.height - totalHeight) / 2f;
            //UIConfig.offsetVertical = 75;
        }

        static public void ResetRootPivot() {
            rootTransform.pivot = new Vector2(0, 1);
            rootTransform.localScale = new Vector3(1, 1, 1);
        }

        static public void CreateCanvas() {
            if (rootTransform != null) {
                DestroyImmediate(rootTransform.transform.parent.gameObject);
            }
            GameObject root = new GameObject();
            root.name = "Canvas";
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = -1;
            //CanvasScalerFixed canvasScaler = root.AddComponent<CanvasScalerFixed>();
            CanvasScaler canvasScaler = root.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.matchWidthOrHeight = 1;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            //canvasScaler.ForcedUpdate();
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;

            root.AddComponent<GraphicRaycaster>();
            root.AddComponent<RoR2.UI.MPEventSystemProvider>();
            Image background = ElementCreator.SpawnImageOffset(new List<Image>(), root, null, new Color(0, 0, 0, 0), new Vector2(0, 1), new Vector2(0, 0), new Vector2(0, 0));
            background.raycastTarget = true;

            GameObject rootTransformObject = new GameObject("Base");
            rootTransformObject.transform.parent = root.transform;
            rootTransform = rootTransformObject.AddComponent<RectTransform>();
            rootTransform.pivot = new Vector2(0, 0);
            rootTransform.anchorMin = Vector2.zero;
            rootTransform.anchorMax = Vector2.one;
            rootTransform.offsetMin = Vector2.zero;
            rootTransform.offsetMax = Vector2.zero;
            rootTransform.localScale = Vector3.one;
            CanvasGroup canvasGroupChild = rootTransformObject.AddComponent<CanvasGroup>();
            canvasGroupChild.blocksRaycasts = true;

            startingItems = rootTransform.gameObject.AddComponent<RoR2.UI.MainMenu.BaseMainMenuScreen>();
            startingItems.desiredCameraTransform = new GameObject().transform;
            startingItems.desiredCameraTransform.position = new Vector3(100, 15, 0);
            startingItems.desiredCameraTransform.eulerAngles = new Vector3(0, 40, 0);
            startingItems.desiredCameraTransform.localScale = new Vector3(1, 1, 1);
            startingItems.onEnter = new UnityEngine.Events.UnityEvent();
            startingItems.onEnter.AddListener(OpenStartingItems);
            startingItems.onExit = new UnityEngine.Events.UnityEvent();
            startingItems.onExit.AddListener(CloseStartingItems);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        static void DrawShading() {
            float shadingHeight = 270;
            Image shadingA = ElementCreator.SpawnImageOffset(new List<Image>(), rootTransform.gameObject, Resources.panelTextures[8], new Color(0, 0, 0, 1), new Vector2(0, 1), new Vector2(0, 0), new Vector2(0, -(rootTransform.rect.height - shadingHeight)));
            Image shadingB = ElementCreator.SpawnImageOffset(new List<Image>(), rootTransform.gameObject, Resources.panelTextures[9], new Color(0, 0, 0, 1), new Vector2(0, 1), new Vector2(0, rootTransform.rect.height - shadingHeight), new Vector2(0, 0));
            shopInterfaces.Add(shadingA.gameObject);
            shopInterfaces.Add(shadingB.gameObject);
        }

        static void DrawBlueButtons() {
            modeButtons.Clear();
            int modesPresent = 0;
            for (int modeIndex = 0; modeIndex < Data.ShopModes.Count; modeIndex++) {
                modeImages.Add(new List<Image>());
                if (Data.ShopModes[(Enums.ShopMode)modeIndex]) {
                    GameObject button = ButtonCreator.SpawnBlueButton(rootTransform.gameObject, new Vector2(0, 1), new Vector2(UIConfig.blueButtonWidth, UIConfig.blueButtonHeight), Data.ShopModeNames[(Enums.ShopMode)modeIndex], TMPro.TextAlignmentOptions.Center, modeImages[modeIndex]);
                    button.GetComponent<RectTransform>().localPosition = new Vector3(UIConfig.offsetHorizontal + (UIConfig.blueButtonWidth + UIConfig.spacingHorizontal) * modesPresent, -UIConfig.offsetVertical, 0);
                    modeButtons.Add(modeIndex, button.GetComponent<RoR2.UI.HGButton>());
                    int mode = modeIndex;
                    GameObject buttonObject = button;
                    button.GetComponent<RoR2.UI.HGButton>().onClick.AddListener(() => {
                        buttonObject.GetComponent<RoR2.UI.MPEventSystemLocator>().eventSystem.SetSelectedGameObject(null);
                        Data.SetMode((Enums.ShopMode)mode);
                        SelectModeButton();
                        SetInstructionsText();
                    });
                    shopInterfaces.Add(button);
                    modesPresent += 1;
                }
            }

            if (StartingItemsGUI.Instance.CurrentProfile.ShopMode != Enums.ShopMode.Random)
            {
                for (uint loadoutIndex = 0; loadoutIndex < StartingItemsGUI.Instance.CurrentProfile.LoadoutCount; loadoutIndex++)
                {
                    loadoutImages.Add(new List<Image>());
                    GameObject button = ButtonCreator.SpawnBlueButton(rootTransform.gameObject, new Vector2(1, 1), new Vector2(UIConfig.blueButtonWidth, UIConfig.blueButtonHeight), $"Profile: {loadoutIndex}", TMPro.TextAlignmentOptions.Center, loadoutImages[(int)loadoutIndex]);
                    button.GetComponent<RectTransform>().localPosition = new Vector3(rootTransform.rect.width - UIConfig.offsetHorizontal - (UIConfig.blueButtonWidth + UIConfig.spacingHorizontal) * (StartingItemsGUI.Instance.CurrentProfile.LoadoutCount - 1 - loadoutIndex), -UIConfig.offsetVertical, 0);
                    var loadout = loadoutIndex;
                    button.GetComponent<RoR2.UI.HGButton>().onClick.AddListener(() => {
                        StartingItemsGUI.Instance.CurrentProfile.CurrentLoadoutIndex = loadout;
                    });
                    shopInterfaces.Add(button);
                }
            }
        }

        static void DrawPoints() {
            List<TMPro.TextMeshProUGUI> pointsText = new();
            Vector2 pivot = new();
            pivot.y = 1;
            Vector3 position = new();
            if (StartingItemsGUI.Instance.CurrentProfile.ShopMode != Enums.ShopMode.Random) {
                int modesDisabled = 0;
                foreach (var modeEnabled in Data.ShopModes)
                {
                    if (!modeEnabled.Value)
                    {
                        modesDisabled += 1;
                    }
                }
                float offset = 0;
                if (modesDisabled <= 0) {
                    offset = (Data.ShopModes.Count - StartingItemsGUI.Instance.CurrentProfile.LoadoutCount) * (UIConfig.blueButtonWidth + UIConfig.spacingHorizontal) * 0.5f;
                }
                position.x = rootTransform.rect.width / 2f + offset;
                pivot.x = 0.5f;
            } else {
                position.x = rootTransform.rect.width - UIConfig.offsetHorizontal;
                pivot.x = 1;
            }
            position.y = -UIConfig.offsetVertical;
            ElementCreator.SpawnTextSize(pointsText, rootTransform.gameObject, new Color(1, 1, 1, 1), 40, 0, pivot, new Vector2(400, UIConfig.blueButtonHeight), position);
            pointText = pointsText[0];
            pointText.text = "Credits: 400¢";
            if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.Random) {
                pointText.alignment = TMPro.TextAlignmentOptions.Right;
            }
            shopInterfaces.Add(pointsText[0].gameObject);
        }

        static void DrawBlackButtons() {
            backButton = ButtonCreator.SpawnBlackButton(rootTransform.gameObject, new Vector2(UIConfig.blackButtonWidth, UIConfig.blackButtonHeight), "Back", new List<TMPro.TextMeshProUGUI>(), true);
            backButton.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(UIConfig.offsetHorizontal, -UIConfig.offsetVertical - UIConfig.blueButtonHeight - UIConfig.spacingVertical - storeHeight - UIConfig.spacingVertical, 0);
            backButton.GetComponent<RoR2.UI.HGButton>().onClick.AddListener(() => {
                SetMenuTitle();
            });
            backButton.GetComponent<RoR2.UI.HGButton>().Select();
            //backButton.GetComponent<RoR2.UI.MPEventSystemLocator>().eventSystem.SetSelectedGameObject(backButton);
            shopInterfaces.Add(backButton.transform.parent.gameObject);

            GameObject statusButton = ButtonCreator.SpawnBlackButton(rootTransform.gameObject, new Vector2(UIConfig.blackButtonWidth, UIConfig.blackButtonHeight), "Potato", statusTexts);
            statusButton.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(UIConfig.offsetHorizontal + UIConfig.blackButtonWidth + UIConfig.spacingHorizontal, -UIConfig.offsetVertical - UIConfig.blueButtonHeight - UIConfig.spacingVertical - storeHeight - UIConfig.spacingVertical, 0);
            statusButton.GetComponent<RoR2.UI.HGButton>().onClick.AddListener(() => {
                StartingItemsGUI.Instance.ToggleEnabled();
            });
            shopInterfaces.Add(statusButton.transform.parent.gameObject);

            if (StartingItemsGUI.Instance.CurrentProfile.ShopMode != Enums.ShopMode.Random) {
                List<TMPro.TextMeshProUGUI> multiplierTexts = new List<TMPro.TextMeshProUGUI>();
                GameObject multiplierButton = ButtonCreator.SpawnBlackButton(rootTransform.gameObject, new Vector2(UIConfig.blackButtonWidth / 2f, UIConfig.blackButtonHeight), "X " + Data.buyMultiplier.ToString(), multiplierTexts);
                multiplierButton.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(rootTransform.rect.width - UIConfig.offsetHorizontal - UIConfig.blackButtonWidth / 2f - UIConfig.blackButtons[(int)StartingItemsGUI.Instance.CurrentProfile.ShopMode] * (UIConfig.spacingHorizontal + UIConfig.blackButtonWidth / 2f), -UIConfig.offsetVertical - UIConfig.blueButtonHeight - UIConfig.spacingVertical - storeHeight - UIConfig.spacingVertical, 0);
                multiplierButton.GetComponent<RoR2.UI.HGButton>().onClick.AddListener(() => {
                    Data.ToggleBuyMultiplier();
                });
                multiplierText = multiplierTexts[0];
                shopInterfaces.Add(multiplierButton.transform.parent.gameObject);
            }

            if (UIConfig.blackButtons[(int)StartingItemsGUI.Instance.CurrentProfile.ShopMode] > 0) {
                GameObject infoButton = ButtonCreator.SpawnBlackButton(UIDrawer.rootTransform.gameObject, new Vector2(UIConfig.blackButtonWidth / 2f, UIConfig.blackButtonHeight), "?", new List<TMPro.TextMeshProUGUI>());
                infoButton.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(UIDrawer.rootTransform.rect.width - UIConfig.offsetHorizontal - UIConfig.blackButtonWidth / 2f, -UIConfig.offsetVertical - UIConfig.blueButtonHeight - UIConfig.spacingVertical - UIDrawer.storeHeight - UIConfig.spacingVertical, 0);

                infoButton.GetComponent<RoR2.UI.HGButton>().onClick.AddListener(() => {
                    DrawInfoPanel();
                });
                UIDrawer.shopInterfaces.Add(infoButton.transform.parent.gameObject);
            }
        }

        static void DrawInstructions() {
            instructionsText.Clear();
            Vector3 position = new();
            position.x = rootTransform.rect.width / 2f;
            position.y = -UIConfig.offsetVertical - UIConfig.blueButtonHeight - UIConfig.spacingVertical - storeHeight - UIConfig.spacingVertical;
            ElementCreator.SpawnTextSize(instructionsText, rootTransform.gameObject, new Color(1, 1, 1, 1), 24, 0, new Vector2(0.5f, 1), new Vector2(500, UIConfig.blackButtonHeight), position);
            SetInstructionsText();
            shopInterfaces.Add(instructionsText[0].gameObject);
        }

        static void SetInstructionsText()
        {
            if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.Free)
            {
                instructionsText[0].text = instructions[0];
            }
            else if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.Random)
            {
                instructionsText[0].text = instructions[4];
            }
            else
            {
                if (StartingItemsGUI.Instance.CurrentProfile.EarningMode == Enums.EarningMode.Bosses)
                {
                    instructionsText[0].text = instructions[1];
                }
                else if (StartingItemsGUI.Instance.CurrentProfile.EarningMode == Enums.EarningMode.Stages)
                {
                    instructionsText[0].text = instructions[2];
                }
                else if (StartingItemsGUI.Instance.CurrentProfile.EarningMode == Enums.EarningMode.GameEnding)
                {
                    instructionsText[0].text = instructions[3];
                } 
            }
        }

        static void DrawModName() {
            List<TMPro.TextMeshProUGUI> modText = new();
            Vector3 position = new();
            position.x = rootTransform.rect.width - UIConfig.offsetHorizontal - (1 + UIConfig.blackButtons[(int)StartingItemsGUI.Instance.CurrentProfile.ShopMode]) * (UIConfig.blackButtonWidth / 2 + UIConfig.spacingHorizontal);
            position.y = -UIConfig.offsetVertical - UIConfig.blueButtonHeight - UIConfig.spacingVertical - storeHeight - UIConfig.spacingVertical;
            ElementCreator.SpawnTextSize(modText, rootTransform.gameObject, new Color(1, 1, 1, 0.025f), 24, 0, new Vector2(1, 1), new Vector2(300, UIConfig.blackButtonHeight), position);
            modText[0].text = $"{StartingItemsGUI.PluginAuthor}:\n{StartingItemsGUI.PluginName}";
            modText[0].alignment = TMPro.TextAlignmentOptions.Right;
            shopInterfaces.Add(modText[0].gameObject);
        }

        static void DrawShop()
        {
            var storeItems = GetStoreItems();
            shopInterfaces.Add(ScrollCreator.CreateScroll(rootTransform, UIConfig.storeRows[(int)StartingItemsGUI.Instance.CurrentProfile.ShopMode], UIConfig.textCount[(int)StartingItemsGUI.Instance.CurrentProfile.ShopMode], storeItems, rootTransform.rect.width - UIConfig.offsetHorizontal * 2, new Vector3(UIConfig.offsetHorizontal, -UIConfig.offsetVertical - UIConfig.blueButtonHeight - UIConfig.spacingVertical, 0), itemImages, itemTexts));
        }

        public static List<StartingItem> GetStoreItems()
        {
            Log.LogDebug("We are getting store items.");

            var storeItems = new List<StartingItem>
            {
                new StartingItem(10)
            };

            Log.LogDebug($"Total items in store: {storeItems.Count}");

            return storeItems;

            //foreach(var itemDef in RoR2.ItemCatalog.allItemDefs)
            //{
            //    var startingItem = new StartingItem(itemDef.itemIndex);

            //    if (Data.UnlockedItem(startingItem) && !storeItems.Contains(startingItem))
            //    {
            //        storeItems.Add(startingItem);
            //    }
            //}

            //foreach (var equipmentDef in RoR2.EquipmentCatalog.equipmentDefs)
            //{
            //    var startingItem = new StartingItem(equipmentDef.equipmentIndex);

            //    if (Data.UnlockedItem(startingItem) && !storeItems.Contains(startingItem))
            //    {
            //        storeItems.Add(startingItem);
            //    }
            //}

            //return storeItems;
        }

        static void DrawInfoPanel() {
            rootTransform.GetComponent<CanvasGroup>().interactable = false;

            Transform background = ElementCreator.SpawnImageOffset(new List<Image>(), UIDrawer.rootTransform.transform.parent.gameObject, null, new Color(0, 0, 0, 0.95f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero).transform;
            background.GetComponent<Image>().raycastTarget = true;

            GameObject panelOutline = PanelCreator.CreatePanelSize(background);
            RectTransform panelTransform = panelOutline.GetComponent<RectTransform>();
            float panelWidth = 700 + UIConfig.panelPadding * 2 + 10;
            float panelHeight = 650 + UIConfig.panelPadding * 2 + 10;
            if (StartingItemsGUI.Instance.CurrentProfile.EarningMode == Enums.EarningMode.GameEnding || StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.Free)
            {
                panelHeight = 525 + UIConfig.panelPadding * 2 + 10;
            }
            panelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
            panelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelHeight);
            panelTransform.localPosition = new Vector3(-panelWidth / 2f, panelHeight / 2f, 0);
            RectTransform panelChildTransform = panelTransform.GetChild(0).GetComponent<RectTransform>();

            List<TMPro.TextMeshProUGUI> text = new List<TMPro.TextMeshProUGUI>();
            ElementCreator.SpawnTextOffset(text, panelChildTransform.gameObject, new Color(1, 1, 1), 24, 0, new Vector2(UIConfig.spacingHorizontal, UIConfig.spacingVertical + UIConfig.blackButtonHeight + UIConfig.spacingVertical), new Vector2(-UIConfig.spacingHorizontal, -UIConfig.spacingVertical));
            if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.Free)
            {
                text[0].text = "UES EMPLOYMENT CONTRACT";
                text[0].text += "\nSUBSECTION 45b: REQUSITION";
                text[0].text += "\n";
                text[0].text += "\nUES always strives to ensure that all its employees are as equipped as necessary to complete their assigned duties. However, UES has finite resources and must determine in who’s hands those resources can be put to the best use. A thorough analysis of employee aptitude concluded that employees with a close familial relation on the Board of Directors (3b) are most able to use the resources of UES to their fullest capacity. To this end, any such employee is afforded the right to requisition any items or equipment they deem, in their own judgement, to be of a benefit in fulfilling their duties. UES will take aggressive legal action against any employee alleging bias or nepotism in regards to this matter. The employee agrees to waive their right to a lawyer (42a) should they be sued in regards to this matter.";
            }
            else if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.Random)
            {
                text[0].text = "UES EMPLOYMENT CONTRACT";
                text[0].text += "\nSUBSECTION 50a: EQUIPPING";
                text[0].text += "\n";
                text[0].text += "\nUES always strives to ensure that all its employees are as equipped as necessary to complete their assigned duties. However, UES has finite resources and must determine in who’s hands those resources can be put to the best use. To this end, a thorough analysis is conducted prior to any deployment to determine which, if any, equipment will give the employee the greatest chance of success. The employee agrees that this analysis its conclusions are infallible and that the manner in which the employee is equipped cannot be the basis for any suit against UES. UES will take aggressive legal action against any employee alleging that equipment is, instead, assigned arbitrarily. The employee agrees to waive their right to a lawyer (42a) should they be sued in regards to this matter.";
            }
            else if (StartingItemsGUI.Instance.CurrentProfile.EarningMode == Enums.EarningMode.Bosses)
            {
                text[0].text = "UES EMPLOYMENT CONTRACT";
                text[0].text += "\nSUBSECTION 18a: BOUNTIES";
                text[0].text += "\n";
                if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.EarnedPersistent)
                {
                    text[0].text += "\nWhile deployed, UES employees should at all times endeavour to improve the relative safety of any environment they find themselves in. To this end, all employees are equipped a UES Threat Level Detection system which will quantify the threat of hostile organisms in the employee’s vicinity. If the organism is deemed to pose an imminent threat to UES operations, as part of our vertical integration policies (38b), the employee will be rewarded with supplemental UES Credits (10a) for eliminating the threat. Credits awarded is dependent upon the threat level of the organism. Credits can be exchanged for any items currently available in the UES Catalogue. Should the employee perish during deployment (41a), a drone will be dispatched to recover any items that were in their possession. Any Credits owing will be accredited to the employee’s next of kin, should they also be an employee of UES, as well as any items they owned.";
                }
                else if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.EarnedConsumable)
                {
                    text[0].text += "\nWhile deployed, UES employees should at all times endeavour to improve the relative safety of any environment they find themselves in. To this end, all employees are equipped a UES Threat Level Detection system which will quantify the threat of hostile organisms in the employee’s vicinity. If the organism is deemed to pose an imminent threat to UES operations, as part of our vertical integration policies (38b), the employee will be rewarded with supplemental UES Credits (10a) for eliminating the threat. Credits awarded is dependent upon the threat level of the organism. Credits can be exchanged for the loan of any items currently available in the UES Catalogue. Any items in the possession of an employee returning from a deployment will be reclaimed for refurbishment, the employee will not be reimbursed. Should the employee perish during deployment (41a), any Credits owing will be accredited to the employee’s next of kin, should they also be an employee of UES.";
                }
            }
            else if (StartingItemsGUI.Instance.CurrentProfile.EarningMode == Enums.EarningMode.Stages)
            {
                text[0].text = "UES EMPLOYMENT CONTRACT";
                text[0].text += "\nSUBSECTION 23a: TELEPORTATION";
                text[0].text += "\n";
                if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.EarnedPersistent)
                {
                    text[0].text += "\nThe employee agrees that primitive teleportation technology has no adverse, health related side effects and that any claims otherwise are unfounded. Should the employee be required to utilize primitive teleporters in the fulfilment of their duties while deployed, the employee agrees that UES is released from all responsibility and liability, and as part of our vertical integration policies (38b), will receive supplemental UES Credits (10a). Credits awarded is dependent upon the number of teleporter journeys the employee made during deployment and the effectiveness of the employee in carrying out the duties outlined in this contract. Credits can be exchanged for any items currently available in the UES Catalogue. Should the employee perish during deployment (41a), a drone will be dispatched to recover any items that were in their possession. Any Credits owing will be accredited to the employee’s next of kin, should they also be an employee of UES, as well as any items they owned.";
                }
                else if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.EarnedConsumable)
                {
                    text[0].text += "\nThe employee agrees that primitive teleportation technology has no adverse, health related side effects and that any claims otherwise are unfounded. Should the employee be required to utilize primitive teleporters in the fulfilment of their duties while deployed, the employee agrees that UES is released from all responsibility and liability, and as part of our vertical integration policies (38b), will receive supplemental UES Credits (10a). Credits awarded is dependent upon the number of teleporter journeys the employee made during deployment and the effectiveness of the employee in carrying out the duties outlined in this contract. Credits can be exchanged for the loan of any items currently available in the UES Catalogue. Any items in the possession of an employee returning from a deployment will be reclaimed for refurbishment, the employee will not be reimbursed. Should the employee perish during deployment (41a), any Credits owing will be accredited to the employee’s next of kin, should they also be an employee of UES.";
                }
            }
            else if (StartingItemsGUI.Instance.CurrentProfile.EarningMode == Enums.EarningMode.GameEnding)
            {
                text[0].text = "UES EMPLOYMENT CONTRACT";
                text[0].text += "\nSUBSECTION 14a: PERFORMANCE BONUSES";
                text[0].text += "\n";
                if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.EarnedPersistent)
                {
                    text[0].text += "\nIt is UES policy that the ends always justify the means (40d). To this end, as part of our vertical integration policies (38b), employees may earn an allotment of supplemental UES Credits (10a) when returning from deployment. Credits awarded is dependent upon the outcome of the deployment and is in no way impacted by the manner in which that outcome was achieved. Credits can be exchanged for any items currently available in the UES Catalogue. Should the employee perish during deployment (41a), a drone will be dispatched to recover any items that were in their possession. Any Credits owing will be accredited to the employee’s next of kin, should they also be an employee of UES, as well as any items they owned.";
                } else if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.EarnedConsumable)
                {
                    text[0].text += "\nIt is UES policy that the ends always justify the means (40d). To this end, as part of our vertical integration policies (38b), employees may earn an allotment of supplemental UES Credits (10a) when returning from deployment. Credits awarded is dependent upon the outcome of the deployment and is in no way impacted by the manner in which that outcome was achieved. Credits can be exchanged for the loan of any items currently available in the UES Catalogue. Any items in the possession of an employee returning from a deployment will be reclaimed for refurbishment, the employee will not be reimbursed. Should the employee perish during deployment (41a), any Credits owing will be accredited to the employee’s next of kin, should they also be an employee of UES.";
                }
            }

            GameObject backButton = ButtonCreator.SpawnBlackButton(panelChildTransform.gameObject, new Vector2(UIConfig.blackButtonWidth, UIConfig.blackButtonHeight), "Back", new List<TMPro.TextMeshProUGUI>());
            RectTransform backButtonTransform = backButton.transform.parent.GetComponent<RectTransform>();
            backButtonTransform.localPosition = new Vector3(panelWidth / 2f - UIConfig.blackButtonWidth / 2f, -panelHeight + UIConfig.spacingVertical + UIConfig.blackButtonHeight, backButtonTransform.localPosition.z);

            RoR2.UI.HGButton previousSelectable = backButton.GetComponent<RoR2.UI.MPEventSystemLocator>().eventSystem.currentSelectedGameObject.GetComponent<RoR2.UI.HGButton>();
            Button backButtonButton = backButton.GetComponent<RoR2.UI.HGButton>();
            backButtonButton.onClick.AddListener(() => {
                rootTransform.GetComponent<CanvasGroup>().interactable = true;
                if (backButtonButton.GetComponent<RoR2.UI.MPEventSystemLocator>().eventSystem.currentInputSource == RoR2.UI.MPEventSystem.InputSource.Gamepad) {
                    previousSelectable.Select();
                } else {
                    previousSelectable.enabled = false;
                    previousSelectable.enabled = true;
                }
                Destroy(background.gameObject);
            });
            backButtonButton.Select();
        }

        static void SelectModeButton()
        {
            modeButtons[(int)StartingItemsGUI.Instance.CurrentProfile.ShopMode].Select();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        static void ClearShopInterfaces() {
            foreach (GameObject gameObject in shopInterfaces) {
                Destroy(gameObject);
            }
            shopInterfaces.Clear();
            modeImages.Clear();
            loadoutImages.Clear();
            itemImages.Clear();
            itemTexts.Clear();
            statusTexts.Clear();
            pointText = null;
            multiplierText = null;
        }

        static public void Refresh() {
            for (int modeIndex = 0; modeIndex < Data.ShopModes.Count; modeIndex++) {
                foreach (Image image in modeImages[modeIndex]) {
                    image.gameObject.SetActive(modeIndex == (int)StartingItemsGUI.Instance.CurrentProfile.ShopMode);
                }
            }
            for (uint loadoutIndex = 0; loadoutIndex < loadoutImages.Count; loadoutIndex++) {
                foreach (Image image in loadoutImages[(int)loadoutIndex]) {
                    image.gameObject.SetActive(StartingItemsGUI.Instance.CurrentProfile.CurrentLoadoutIndex == loadoutIndex);
                }
            }

            if (StartingItemsGUI.Instance.ModEnabled)
            {
                //statusTexts[0].faceColor = UIConfig.enabledColor;
                statusTexts[0].color = UIConfig.enabledColor;
                statusTexts[0].text = "Enabled";
            } else {
                //statusTexts[0].faceColor = UIConfig.disabledColor;
                statusTexts[0].color = UIConfig.disabledColor;
                statusTexts[0].text = "Disabled";
            }

            if (StartingItemsGUI.Instance.CurrentProfile.ShopMode != Enums.ShopMode.Random) {
                multiplierText.text = "X " + Data.buyMultiplier.ToString();
            }

            if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.EarnedConsumable) {
                UIDrawerEarnedConsumable.Refresh();
            } else if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.EarnedPersistent) {
                UIDrawerEarnedPersistent.Refresh();
            } else if (StartingItemsGUI.Instance.CurrentProfile.ShopMode == Enums.ShopMode.Free) {
                UIDrawerFree.Refresh();
            }
        }


        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        static public void SetMenuStartingItems() {
            UIVanilla.menuController.SetDesiredMenuScreen(startingItems);
        }

        static public void SetMenuTitle()
        {
            UIVanilla.menuController.SetDesiredMenuScreen(UIVanilla.mainMenu);
        }

        static public void OpenStartingItems()
        {
            Data.buyMultiplier = 1;
            UIDrawer.DrawUI();
            rootTransform.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        static public void CloseStartingItems() {
            rootTransform.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
}
