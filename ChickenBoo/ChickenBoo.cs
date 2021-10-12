// JotunnModStub
// a Valheim mod skeleton using Jötunn
// 
// File:    JotunnModStub.cs
// Project: JotunnModStub

using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;

namespace ChickenBoo
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInDependency("com.rockerkitten.boneappetit")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class ChickenBoo : BaseUnityPlugin
    {
        #region Variables

        public const string PluginGUID = "com.zarboz.chickenboo";
        public const string PluginName = "ChickenBoo";
        public const string PluginVersion = "0.0.1";
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();
        internal RandomEggLayer _eggLayer;
        internal static Harmony _harmony;
        public GameObject chiken { get; set; }

        #endregion

        #region ConfigVariables

        internal static ConfigEntry<float> MinimumSpawnTimeForEgg;
        internal static ConfigEntry<float> MaximumSpawnTimeForEgg;
        internal static ConfigEntry<int> SpawnVol1;
        internal static ConfigEntry<int> SpawnVol2;
        internal static ConfigEntry<int> SpawnVol3;
        

        #endregion

        #region  EventMethods

        private void Awake()
        {
            LoadAssets();
            SetupConfigs();
            ItemManager.OnItemsRegistered += RegisterRockersEggs;
            _harmony = new Harmony(Info.Metadata.GUID);
            _harmony.PatchAll();
        }

        #endregion


        #region  UtilityMethods

        private void RegisterRockersEggs()
        {
            var RK_egg = PrefabManager.Instance.GetPrefab("rk_egg");
            chiken.GetComponent<RandomEggLayer>().EggObject = RK_egg;
            _eggLayer = chiken.GetComponent<RandomEggLayer>();
            SetupConsumables();
        }

        private GameObject RetrieveGO(string name)
        {
            var fab = ZNetScene.instance.GetPrefab(name);
            return fab;
        }

        private ItemDrop ReturnItemDrop(GameObject gameObject)
        {
            var drop = gameObject.GetComponent<ItemDrop>();
            
            return drop;
        }
        private void SetupConsumables()
        {
            _eggLayer._monsterAI.m_consumeItems = new List<ItemDrop>
            {
                ReturnItemDrop(RetrieveGO("Dandelion")),
                ReturnItemDrop(RetrieveGO("Blueberries")),
                ReturnItemDrop(RetrieveGO("Raspberry")),
                ReturnItemDrop(RetrieveGO("Acorn")),
                ReturnItemDrop(RetrieveGO("OnionSeeds"))
            };
        }

        private void LoadAssets()
        {
            AssetBundle assetBundle =
                AssetUtils.LoadAssetBundleFromResources("chickenboo", typeof(ChickenBoo).Assembly);
            chiken = assetBundle.LoadAsset<GameObject>("ChickenBoo");
            var coolhat = assetBundle.LoadAsset<GameObject>("helmet");
            PrefabManager.Instance.AddPrefab(chiken);
            CustomItem CI = new CustomItem(coolhat, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_workbench",
                RepairStation = "piece_workbench",
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig{ Amount = 1, Item = "Wood", Recover = true, AmountPerLevel = 1}
                }
            });
            ItemManager.Instance.AddItem(CI);
        }

        #endregion

        #region ConfigMethods

        private void SetupConfigs()
        {
            Config.SaveOnConfigSet = true;
            
            MinimumSpawnTimeForEgg = Config.Bind("Chicken", "Egg Spawn Time Min", 15f ,new ConfigDescription(
                "This is the minimum random volume of time in the range of time to select", 
                new AcceptableValueRange<float>(15f, 1000f), 
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            
            MaximumSpawnTimeForEgg = Config.Bind("Chicken", "Egg Spawn Time Max", 45f ,new ConfigDescription(
                "This is the maximum random volume of time in the range of time to select", 
                new AcceptableValueRange<float>(15f, 1000f), 
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            
            SpawnVol1 = Config.Bind("Chicken", "Egg Spawn Count 1", 1, new ConfigDescription(
                "This is the volume of eggs that will be laid when random selection chooses a value < .45 which in theory is 45% of the time", 
                new AcceptableValueRange<int>(1, 1000), 
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            
            SpawnVol2 = Config.Bind("Chicken", "Egg Spawn Count 2", 6, new ConfigDescription(
                "This is the volume of eggs that will be laid when random selection chooses a value < .9 which in theory is 45% of the time", 
                new AcceptableValueRange<int>(1, 1000), 
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
            
            SpawnVol3 = Config.Bind("Chicken", "Egg Spawn Count 3", 12, new ConfigDescription(
                "This is the volume of eggs that will be laid the remaining 10% of the selection ranges", 
                new AcceptableValueRange<int>(1, 1000), 
                new ConfigurationManagerAttributes { IsAdminOnly = true }));
            
        }

        #endregion

        #region  Patches

        [HarmonyPatch(typeof(Tameable), nameof(Tameable.Interact))]
        public static class InteractPatch
        {
            public static bool Prefix(Humanoid user, bool hold, bool alt, Tameable __instance)
            {
                if (__instance.gameObject.GetComponent<RandomEggLayer>() != null)
                {
                    Debug.Log("ChickenFound");
                    var userinv = user.GetInventory();
                    foreach (var hat in from item in userinv.m_inventory
                        where item.m_shared.m_name == "$chicken_hat"
                        select item
                        into hat
                        let tmphat = user.m_inventory.GetItem(hat.m_shared.m_name)
                        let removedhat = user.m_inventory.RemoveOneItem(tmphat)
                        where removedhat
                        select hat)
                    {
                        Instantiate(hat.m_dropPrefab);
                        hat.m_dropPrefab.transform.SetParent(__instance.gameObject.GetComponent<HelmetMounter>()
                            .HelmetMountPoint);
                        var car = __instance.GetComponentInParent<Humanoid>();
                        car.m_nview.GetZDO().Set(hat.GetHashCode(), true);
                        Debug.Log("I put a hat on lelz");
                        hat.m_dropPrefab.transform.position = Vector3.zero;
                        hat.m_dropPrefab.transform.localPosition = Vector3.zero;
                        var rb = hat.m_dropPrefab.GetComponent<Rigidbody>();
                        rb.isKinematic = true;
                        return true;
                    }
                }

                if (!__instance.m_nview.IsValid())
                {
                    return false;
                }
                if (hold)
                {
                    return false;
                }
                if (alt)
                {
                    __instance.SetName();
                    return true;
                }
                string hoverName = __instance.m_character.GetHoverName();
                if (__instance.m_character.IsTamed())
                {
                    if (Time.time - __instance.m_lastPetTime > 1f)
                    {
                        __instance.m_lastPetTime = Time.time;
                        __instance.m_petEffect.Create(__instance.transform.position, __instance.transform.rotation);
                        if (__instance.m_commandable)
                        {
                            __instance.Command(user);
                        }
                        else
                        {
                            user.Message(MessageHud.MessageType.Center, hoverName + " $hud_tamelove");
                        }
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        #endregion
  

    }
}