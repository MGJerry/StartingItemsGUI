using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

// SecurityPermision set to minimum and SkipVerification set to true
// for skipping access modifiers check from the mono JIT
// The same attributes are added to the assembly when ticking
// Unsafe Code in the Project settings
// This is done here to allow an explanation of the trick and
// not in an outside source you could potentially miss.
// https://github.com/risk-of-thunder/R2API/blob/master/R2API/AssemblyInfo.cs#L4-L14

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[module: UnverifiableCode]
namespace StartingItemsGUI
{
    [BepInEx.BepInDependency(R2API.R2API.PluginGUID)]
    [BepInEx.BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2API.Utils.R2APISubmoduleDependency(nameof(R2API.ItemAPI), nameof(R2API.Networking.NetworkingAPI), "ResourcesAPI")]
    public class StartingItemsGUI : BaseUnityPlugin
    {
        public const string PluginAuthor = "szymonj99";
        public const string PluginName = "StartingItemsGUI";
        public const string PluginVersion = "2.0.0";
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public BepInEx.PluginInfo PInfo { get; protected set; } = null;

        /// <summary>
        /// The current instance of the mod.
        /// This stores information such as the current StartingItemsGUI Profile (not to be confused with loadout, or RoR2 profile.)
        /// </summary>
        public static StartingItemsGUI Instance { get; protected set; } = null;

        // If we move the SetupHooks function away from here, this might have to be made public. We'll see!
        public Profile CurrentProfile { get; protected set; } = null;

        public bool ModEnabled { get { return ConfigManager.ModEnabled.Value; } set { ConfigManager.ModEnabled.Value = value; } }

        // In the future, it might be nice adding in a toggle for this at runtime (maybe in debug build only).
        public bool ShowAllItems { get { return ConfigManager.ShowAllItems.Value; } }

        public System.Text.Json.JsonSerializerOptions JsonFactory = new System.Text.Json.JsonSerializerOptions();

        List<Coroutine> characterMasterCoroutines = new();

        private void SceneLoadSetup()
        {
            UIDrawer.CreateCanvas();
            UIVanilla.GetObjectsFromScene();
            UIVanilla.CreateMenuButton();
        }

        void Awake()
        {
            Log.Init(Logger);

            Instance = this;
            Instance.PInfo = Info;
            Instance.JsonFactory.Converters.Add(new StartingItemsJsonFactory());
            Resources.LoadResources();
            Instance.SetupHooks();

            ConfigManager.InitConfig();

            // I wonder, is this even necessary?
            Instance.gameObject.AddComponent<Util>();
            R2API.Networking.NetworkingAPI.RegisterMessageType<Connection>();
            R2API.Networking.NetworkingAPI.RegisterMessageType<ItemPurchased>();
            R2API.Networking.NetworkingAPI.RegisterMessageType<SpawnItems>();
        }

        // It might look better if we were to move these to a dedicated `EventManager` class.
        private void SetupHooks()
        {
            SetupPreGameHook();
            SetupUIHooks();
            SetupRunStartGlobalHook();
            SetupPointsHooks();
        }

        private void SetupPreGameHook()
        {
            // Between runs, do some cleaning.
            On.RoR2.PreGameController.OnEnable += (orig, preGameController) =>
            {
                Log.LogInfo("OnEnable called");

                GameManager.ClearItems();

                orig(preGameController);
            };
        }

        // I think this can get reworked soon.
        // Additional info: https://github.com/szymonj99/StartingItemsGUI/issues/18#issuecomment-1100785233
        private void SetupRunStartGlobalHook()
        {
            RoR2.Run.onRunStartGlobal += (run) =>
            {
                Log.LogInfo("onRunStartGlobal called");

                foreach (var coroutine in characterMasterCoroutines)
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                }
                characterMasterCoroutines.Clear();
                if (UnityEngine.Networking.NetworkServer.active)
                {
                    foreach (NetworkUser networkUser in RoR2.NetworkUser.readOnlyInstancesList)
                    {
                        GameManager.status.Add(networkUser.netId.Value, new() { false, false, false });
                        GameManager.items.Add(networkUser.netId.Value, new());
                        GameManager.modes.Add(networkUser.netId.Value, -1);
                        characterMasterCoroutines.Add(StartCoroutine(GetMasterController(networkUser)));
                    }
                }
                if (NetworkClient.active)
                {
                    foreach (NetworkUser networkUser in RoR2.NetworkUser.readOnlyInstancesList)
                    {
                        if (networkUser.isLocalPlayer)
                        {
                            GameManager.SendItems(networkUser);
                        }
                    }
                }
            };
        }
        private void SetupUIHooks()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, mode) =>
            {
                Log.LogInfo("sceneLoaded called");

                if (scene.name != "title")
                {
                    return;
                }

                
                if (RoR2.PlatformSystems.saveSystem.loggedInProfiles.Count > 0)
                {
                    var playerName = RoR2.PlatformSystems.saveSystem.loggedInProfiles[0];
                    StartingItemsGUI.Instance.CurrentProfile = new Profile(playerName);
                }

                SceneLoadSetup();
                GameManager.ClearItems();
            };

            On.RoR2.UI.ScrollToSelection.ScrollToRect += (scrollToRect, scrollToSelection, transform) =>
            {
                scrollToRect(scrollToSelection, transform);

                var scrollRect = scrollToSelection.GetComponent<UnityEngine.UI.ScrollRect>();
                if (!scrollRect.horizontal || scrollRect.horizontalScrollbar == null)
                {
                    return;
                }

                var targetWorldCorners = new UnityEngine.Vector3[4];
                var viewPortWorldCorners = new UnityEngine.Vector3[4];
                var contentWorldCorners = new UnityEngine.Vector3[4];
                scrollToSelection.GetComponent<RoR2.UI.MPEventSystemLocator>().eventSystem.currentSelectedGameObject.GetComponent<UnityEngine.RectTransform>().GetWorldCorners(targetWorldCorners);
                scrollRect.viewport.GetWorldCorners(viewPortWorldCorners);
                scrollRect.content.GetWorldCorners(contentWorldCorners);
                float x5 = targetWorldCorners[2].x;
                var x6 = (double)targetWorldCorners[0].x;
                float x7 = viewPortWorldCorners[2].x;
                float x8 = viewPortWorldCorners[0].x;
                float x9 = contentWorldCorners[2].x;
                float x10 = contentWorldCorners[0].x;
                float num5 = x5 - x7;
                var num6 = (double)x8;
                var num7 = (float)(x6 - num6);
                float num8 = (x9 - x10) - (x7 - x8);
                if ((double)num5 > 0.0)
                    scrollRect.horizontalScrollbar.value += num5 / num8;
                if ((double)num7 >= 0.0)
                    return;
                scrollRect.horizontalScrollbar.value += num7 / num8;
            };
        }
        private void SetupPointsHooks()
        {
            RoR2.Run.onClientGameOverGlobal += (run, runReport) =>
            {
                Log.LogInfo("onClientGameOverGlobal called");

                DataEarnedConsumable.UpdateUserPointsStages(run, runReport);
                DataEarnedPersistent.UpdateUserPointsStages(run, runReport);
            };

            On.RoR2.CharacterBody.OnDeathStart += (orig, characterBody) =>
            {
                Log.LogInfo("OnDeathStart called");

                DataEarnedConsumable.UpdateUserPointsBoss(characterBody.name);
                DataEarnedPersistent.UpdateUserPointsBoss(characterBody.name);
                orig(characterBody);
            };
        }

        System.Collections.IEnumerator GetMasterController(RoR2.NetworkUser networkUser)
        {
            yield return new UnityEngine.WaitUntil(() => networkUser.masterController != null);

            GameManager.SetCharacterMaster(networkUser.netId.Value, networkUser.masterController.master);
        }

        public void ToggleEnabled()
        {
            ModEnabled = !ModEnabled;
            UIDrawer.Refresh();
        }
    }
}
 
