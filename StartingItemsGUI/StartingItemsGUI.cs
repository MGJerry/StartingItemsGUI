using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace StartingItemsGUI
{
    [BepInEx.BepInDependency(R2API.R2API.PluginGUID)]
    [BepInEx.BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2API.Utils.R2APISubmoduleDependency(nameof(R2API.ItemAPI), nameof(R2API.PrefabAPI), nameof(R2API.Networking.NetworkingAPI), "ResourcesAPI")]
    public class StartingItemsGUI : BaseUnityPlugin
    {
        public const string PluginAuthor = "szymonj99";
        public const string PluginName = "StartingItemsGUI";
        public const string PluginVersion = "0.0.1";
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public BepInEx.PluginInfo PInfo { get; protected set; } = null;

        static public StartingItemsGUI Instance { get; protected set; } = null;

        List<Coroutine> characterMasterCoroutines = new List<Coroutine>();

        void OnAwake()
        {
            Data.UpdateConfigLocations();
            Instance.gameObject.AddComponent<Util>();
            R2API.Networking.NetworkingAPI.RegisterMessageType<Connection>();
            R2API.Networking.NetworkingAPI.RegisterMessageType<ItemPurchased>();
            R2API.Networking.NetworkingAPI.RegisterMessageType<SpawnItems>();
        }

        void SceneLoadSetup()
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
            Resources.LoadResources();
            Instance.SetupHooks();
            Instance.OnAwake();
        }

        // It might look better if we were to move these to a dedicated `EventManager` class.
        private void SetupHooks()
        {
            SetupPreGameHook();
            SetupUIHooks();
            SetupRunStartGlobalHook();
            SetupPointsHooks();
            SetupItemCatalogueHook();
        }

        private void SetupPreGameHook()
        {
            // Between runs, do some cleaning.
            On.RoR2.PreGameController.OnEnable += (orig, preGameController) =>
            {
                Data.localUsers.Clear();
                Data.SetForcedMode(-1);
                GameManager.ClearItems();

                orig(preGameController);
            };
        }

        // I think this can get reworked soon.
        private void SetupRunStartGlobalHook()
        {
            RoR2.Run.onRunStartGlobal += (run) =>
            {
                Data.RefreshInfo();
                Data.SetForcedMode(Data.mode);
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
                        GameManager.status.Add(networkUser.netId.Value, new List<bool>() { false, false, false });
                        GameManager.items.Add(networkUser.netId.Value, new Dictionary<int, int>());
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
                            Data.localUsers.Add(networkUser.localUser.userProfile.fileName);
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
                if (scene.name == "title")
                {
                    SceneLoadSetup();
                    Data.localUsers.Clear();
                    Data.SetForcedMode(-1);
                    GameManager.ClearItems();
                }
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
                DataEarntConsumable.UpdateUserPointsStages(run, runReport);
                DataEarntPersistent.UpdateUserPointsStages(run, runReport);
            };

            On.RoR2.CharacterBody.OnDeathStart += (orig, characterBody) =>
            {
                DataEarntConsumable.UpdateUserPointsBoss(characterBody.name);
                DataEarntPersistent.UpdateUserPointsBoss(characterBody.name);
                orig(characterBody);
            };
        }

        private void SetupItemCatalogueHook()
        {
            RoR2.RoR2Application.onLoad += () =>
            {
                Log.LogInfo("OnLoad Called.");

                // We could further delay this until we are sure all mods are initialised.
                Data.PopulateItemCatalogues();
            };
        }

        IEnumerator<float> GetMasterController(NetworkUser networkUser)
        {
            PlayerCharacterMasterController masterController = networkUser.masterController;
            while (masterController == null)
            {
                masterController = networkUser.masterController;
                yield return 0;
            }
            GameManager.SetCharacterMaster(networkUser.netId.Value, networkUser.masterController.master);
        }
    }
}
 