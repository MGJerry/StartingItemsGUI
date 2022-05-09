﻿using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace StartingItemsGUI
{
    public class DataEarnedConsumable : MonoBehaviour
    {
        public static void BuyItem(StartingItem startingItem, uint quantity)
        {
            bool boughtItem = false;
            uint counter;
            for (counter = 1; counter <= quantity; counter++)
            {
                if (StartingItemsGUI.Instance.CurrentProfile.Credits < (counter * Data.GetStartingItemPrice(startingItem)))
                {
                    break;
                }
                boughtItem = true;
            }
            if (boughtItem)
            {
                StartingItemsGUI.Instance.CurrentProfile.PurchaseItem(startingItem, counter);
                StartingItemsGUI.Instance.CurrentProfile.RemoveCredits(counter * Data.GetStartingItemPrice(startingItem));
                UIDrawer.Refresh();
            }
        }

        public static void SellItem(StartingItem startingItem, uint quantity)
        {
            bool soldItem = false;
            uint counter;
            for (counter = 1; counter <= quantity; counter++)
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
                StartingItemsGUI.Instance.CurrentProfile.AddCredits(counter * Data.GetStartingItemPrice(startingItem));
                UIDrawer.Refresh();
            }
        }

        // This method does magical things that I do not want to look into.
        static float GetDifficultyMultiplier(Run run)
        {
            var easyMultiplier = ConfigManager.EasyMultiplierConsumable.Value;
            var normalMultiplier = ConfigManager.NormalMultiplierConsumable.Value;
            var hardMultiplier = ConfigManager.HardMultiplierConsumable.Value;
            var eclipseMultiplier = ConfigManager.EclipseMultiplierConsumable.Value;
            List<float> functionValues = Util.GetDifficultyParabola(easyMultiplier, normalMultiplier, hardMultiplier, eclipseMultiplier);
            float scalingValue = DifficultyCatalog.GetDifficultyDef(run.selectedDifficulty).scalingValue;
            scalingValue += Data.GetEclipseScalingValueAdd(run);

            return Mathf.Max(functionValues[4], Mathf.Min(functionValues[3], functionValues[0] * Mathf.Pow(scalingValue, 2) + functionValues[1] * scalingValue + functionValues[2]));
        }

        public static void UpdateUserPointsStages(Run run, RunReport runReport)
        {
            if (StartingItemsGUI.Instance.CurrentProfile.EarningMode == Enums.EarningMode.Stages || StartingItemsGUI.Instance.CurrentProfile.EarningMode == Enums.EarningMode.GameEnding)
            {
                float pointsMultiplier = 1;
                if (runReport.gameEnding.gameEndingIndex == RoR2.RoR2Content.GameEndings.MainEnding.gameEndingIndex)
                {
                    pointsMultiplier *= ConfigManager.WinMultiplierConsumable.Value;
                }
                else if (runReport.gameEnding.gameEndingIndex == RoR2.RoR2Content.GameEndings.StandardLoss.gameEndingIndex)
                {
                    pointsMultiplier *= ConfigManager.LoseMultiplierConsumable.Value;
                }
                else if (runReport.gameEnding.gameEndingIndex == RoR2.RoR2Content.GameEndings.ObliterationEnding.gameEndingIndex)
                {
                    pointsMultiplier *= ConfigManager.ObliterationMultiplierConsumable.Value;
                }
                else if (runReport.gameEnding.gameEndingIndex == RoR2.RoR2Content.GameEndings.LimboEnding.gameEndingIndex)
                {
                    pointsMultiplier *= ConfigManager.LimboMultiplierConsumable.Value;
                }
                pointsMultiplier *= GetDifficultyMultiplier(run);

                if (StartingItemsGUI.Instance.CurrentProfile.EarningMode == Enums.EarningMode.Stages)
                {
                    pointsMultiplier *= run.stageClearCount;
                }

                //foreach (var userID in Data.localUsers)
                //{
                //    Data.RefreshInfo(userID);
                //}
            }
        }

        public static void UpdateUserPointsBoss(string givenName)
        {
            if (StartingItemsGUI.Instance.CurrentProfile.EarningMode == Enums.EarningMode.Bosses)
            {
                if (givenName.Contains("ScavLunar") || givenName.Contains("BrotherHurt"))
                {
                    //float creditsEarned = GetDifficultyMultiplier(Run.instance) * Data.lunarScavPoints;
                    float creditsEarned = GetDifficultyMultiplier(Run.instance) * 50;
                    
                    //foreach (string userID in Data.localUsers)
                    //{
                    //    Data.RefreshInfo(userID);
                    //}
                }
            }
        }
    }
}

