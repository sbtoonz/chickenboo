using System;
using System.Collections.Generic;
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
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class ChickenBoo : BaseUnityPlugin
    {
        #region Variables

        public const string PluginGUID = "com.zarboz.chickenboo";
        public const string PluginName = "ChickenBoo";
        public const string PluginVersion = "0.0.4";
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();
        internal RandomEggLayer _eggLayer;
        internal static Harmony _harmony;
        public static GameObject chiken { get; set; }

        #endregion

        #region ConfigVariables

        internal static ConfigEntry<float> MinimumSpawnTimeForEgg;
        internal static ConfigEntry<float> MaximumSpawnTimeForEgg;
        internal static ConfigEntry<int> SpawnVol1;
        internal static ConfigEntry<int> SpawnVol2;
        internal static ConfigEntry<int> SpawnVol3;
        internal static ConfigEntry<float> FeatherChance;
        
        private CustomItem RawEggItem;
        private CustomItem RawChickenItem;
        private CustomItem GrilledChickenItem;
        private CustomItem FriedEggItem;
        private CustomItem BoiledEggItem;
        private AssetBundle assetBundle;

        #endregion

        #region EventMethods

        private void Awake()
        {
            LoadAssets();
            SetupConfigs();
            SetupLocalization();
            ItemManager.OnItemsRegistered += RegisterRockersEggs;
            PrefabManager.OnVanillaPrefabsAvailable += SetupFoods;
            PrefabManager.OnPrefabsRegistered += LoadHat;
            _harmony = new Harmony(Info.Metadata.GUID);
            _harmony.PatchAll();
        }

        #endregion


        #region UtilityMethods

        private void RegisterRockersEggs()
        {
            var RK_egg = PrefabManager.Instance.GetPrefab("raw_egg");
            chiken.GetComponent<RandomEggLayer>().EggObject = RK_egg;
            _eggLayer = chiken.GetComponent<RandomEggLayer>();
            SetupConsumables();
            AddtoCharDrops();
            ItemManager.OnItemsRegistered -= RegisterRockersEggs;
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

        private void AddtoCharDrops()
        {
            var chardrop = chiken.GetComponent<CharacterDrop>();
            chardrop.m_drops.Add(new CharacterDrop.Drop
            {
                m_prefab = RetrieveGO("Feathers"),
                m_chance = FeatherChance.Value,
                m_amountMax = 6,
                m_amountMin = 1,
                m_levelMultiplier = true,
                m_onePerPlayer = true
            });
        }
        private void SetupFoods()
        {

            var RawEgg = assetBundle.LoadAsset<GameObject>("raw_egg");
            var RawChicken = assetBundle.LoadAsset<GameObject>("raw_chicken");
            

            RawEggItem = new CustomItem(RawEgg, true);
            ItemManager.Instance.AddItem(RawEggItem);

            RawChickenItem = new CustomItem(RawChicken, true);
            ItemManager.Instance.AddItem(RawChickenItem);

            SetupStubbornFoods();
            PrefabManager.OnVanillaPrefabsAvailable -= SetupFoods;
        }

        private void SetupStubbornFoods()
        {
            var GrilledChicken = assetBundle.LoadAsset<GameObject>("cooked_chicken");
            var FriedEgg = assetBundle.LoadAsset<GameObject>("fried_egg");
            var BoiledEgg = assetBundle.LoadAsset<GameObject>("boiled_egg");
            
            GrilledChickenItem = new CustomItem(GrilledChicken, true, new ItemConfig
            {
                Amount = 1,
                Description = "Grilled Chicken Meat",
            });

            FriedEggItem = new CustomItem(FriedEgg, true, new ItemConfig
            {
                Amount = 1,
                Description = "A fried egg",
                CraftingStation = "piece_cauldron",
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig
                    {
                        Amount = 1,
                        Item = "raw_egg",
                        Recover = false,
                        AmountPerLevel = 1
                    }
                }
            });

            BoiledEggItem = new CustomItem(BoiledEgg, true, new ItemConfig
            {
                Amount = 1,
                Description = "A boiled egg",
                CraftingStation = "piece_cauldron",
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig
                    {
                        Amount = 1,
                        Item = "raw_egg",
                        AmountPerLevel = 1
                    }
                }
            });




            ItemManager.Instance.AddItem(GrilledChickenItem);
            ItemManager.Instance.AddItem(FriedEggItem);
            ItemManager.Instance.AddItem(BoiledEggItem);
            CookingStationConversion();
        }

        private void CookingStationConversion()
        {
            var ironcovertor = new CustomItemConversion(new CookingConversionConfig
            {
                Station = "piece_cookingstation_iron",
                FromItem = "raw_chicken",
                ToItem = "cooked_chicken",
                CookTime = 2f
            });

            ItemManager.Instance.AddItemConversion(ironcovertor);
        }

        private void LoadHat()
        {
            var coolhat = assetBundle.LoadAsset<GameObject>("helmet");
            CustomItem CI = new CustomItem(coolhat, false, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_workbench",
                RepairStation = "piece_workbench",
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig { Amount = 1, Item = "Wood", Recover = true, AmountPerLevel = 1 }
                }
            });
            ItemManager.Instance.AddItem(CI);
            
            
            var sombrero = assetBundle.LoadAsset<GameObject>("chickensombrero");
            CustomItem somb = new CustomItem(sombrero, true, new ItemConfig
            {
                Amount = 1,
                CraftingStation = "piece_workbench",
                RepairStation = "piece_workbench",
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig { Amount = 1, Item = "Wood", Recover = true, AmountPerLevel = 1 }
                }
            });
            ItemManager.Instance.AddItem(somb);
                
                
            PrefabManager.OnPrefabsRegistered -= LoadHat;

        }

        private void LoadAssets()
        {
            assetBundle = AssetUtils.LoadAssetBundleFromResources("chickenboo", typeof(ChickenBoo).Assembly);
            chiken = assetBundle.LoadAsset<GameObject>("ChickenBoo");
            PrefabManager.Instance.AddPrefab(chiken);
        }


        #endregion

        #region ConfigMethods

        private void SetupConfigs()
        {
            Config.SaveOnConfigSet = true;

            MinimumSpawnTimeForEgg = Config.Bind("Chicken", "Egg Spawn Time Min", 150f, new ConfigDescription(
                "This is the minimum random volume of time in the range of time to select",
                new AcceptableValueRange<float>(15f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));


            MaximumSpawnTimeForEgg = Config.Bind("Chicken", "Egg Spawn Time Max", 450f, new ConfigDescription(
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

            FeatherChance = Config.Bind("Chicken", "Feather Drop Chance", 0.5f,new ConfigDescription(
                "This is a representation of percent chance in number format that feathers will drop from the chicken", null, new ConfigurationManagerAttributes { IsAdminOnly = true }));

        }

        #endregion

        #region Patches

        [HarmonyPatch(typeof(Tameable), nameof(Tameable.Interact))]
        public static class InteractPatch
        {
            public static void Postfix(Tameable __instance)
            {
                if (__instance.gameObject.GetComponent<RandomEggLayer>() != null)
                {
                    if (Input.GetKey(KeyCode.LeftAlt))
                    {
                        if(__instance.gameObject.GetComponent<RandomEggLayer>()._helmetMounter.HelmetMounted)
                            return;
                        try
                        {
                            var userinv = Player.m_localPlayer.GetInventory();
                            foreach (var item in userinv.m_inventory)
                            {
                                if (item.m_shared.m_name == "$chicken_hat")
                                {
                                    var tmphat = userinv.GetItem(item.m_shared.m_name);
                                    if(userinv.RemoveOneItem(tmphat))
                                    {
                                        var hm = __instance.gameObject.GetComponent<RandomEggLayer>()._helmetMounter;
                                        hm.HelmetObject.SetActive(true);
                                        var znv = __instance.GetComponent<ZNetView>();
                                        znv.m_zdo.Set("$chicken_hat", true);
                                        hm.HelmetMounted = true;
                                    }

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            
                        }
                        
                    }

                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        try
                        {
                            if(__instance.gameObject.GetComponent<RandomEggLayer>()._helmetMounter.HelmetMounted)
                                return;
                            var userinv = Player.m_localPlayer.GetInventory();
                            foreach (var item in userinv.m_inventory)
                            {
                                if (item.m_shared.m_name == "$chicken_sombrero")
                                {
                                    var tmphat = userinv.GetItem(item.m_shared.m_name);
                                    if(userinv.RemoveOneItem(tmphat))
                                    {
                                        var hm = __instance.gameObject.GetComponent<RandomEggLayer>()._helmetMounter;
                                        hm.GravesSombrero.SetActive(true);
                                        var znv = __instance.GetComponent<ZNetView>();
                                        znv.m_zdo.Set("$chicken_sombrero", true);
                                        hm.HelmetMounted = true;
                                    }

                                }
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
        }


        [HarmonyPatch(typeof(Tameable), nameof(Tameable.GetHoverText))]

        public static class HoverTextPatch
        {
            public static void Postfix(ref string __result, Tameable __instance)
            {
                if (__instance.gameObject.GetComponent<RandomEggLayer>() == null) return;
                if (!__instance.gameObject.GetComponent<Humanoid>().IsTamed()) return;
                if (!__instance.gameObject.GetComponent<RandomEggLayer>()._helmetMounter.HelmetMounted)
                    __result += global::Localization.instance.Localize(
                        "\n[<b><color=yellow>L-Alt + $KEY_Use</color></b>] To Equip Viking Hat\n[<b><color=yellow>L-Ctrl + $KEY_Use</color></b>] To Equip Sombrero");
            }
        }


        [HarmonyPatch(typeof(SpawnSystem), nameof(SpawnSystem.Awake))]
        public static class SpawnPatch
        {
            private static List<SpawnSystem.SpawnData> _spawnDatas = new List<SpawnSystem.SpawnData>();
            public static void Prefix(SpawnSystem __instance)
            {
                foreach (var spawnData in __instance.m_spawners)
                {
                    if (spawnData.m_prefab.name == "Boar")
                    {
                        var tmp = spawnData.Clone();
                        tmp.m_prefab = chiken;
                        tmp.m_name = "ChickenBoo";
                        tmp.m_spawnAtDay = true;
                        tmp.m_spawnAtNight = true;
                        tmp.m_maxSpawned = 10;
                        tmp.m_enabled = true;
                        _spawnDatas.Add(tmp);
                    }
                }

                foreach (var spawnData in _spawnDatas)
                {
                    __instance.m_spawners.Add(spawnData);
                }
            }
        }

        #endregion

        #region Localization

        private void SetupLocalization()
        {
            Localization.AddTranslation("English", new Dictionary<string, string>
            {
                {"enemy_chicken","Chicken"},
                {"raw_egg","Raw Egg"},
                {"raw_egg_descrip","A fresh egg from your chicken. Great for cooking"},
                {"raw_chicken","Raw Chicken"},
                {"raw_chicken_descrip","Raw chicken meat for cooking."},
                {"boiled_egg","A Boiled Egg"},
                {"boiled_egg_descrip","A hard boiled egg, nutritious and delicious"},
                {"fried_egg","Fried Egg"},
                {"fried_egg_descrip","A fried egg for any time of day!"},
                {"cooked_chicken","Grilled Chicken"},
                {"cooked_chicken_descrip", "A nice grilled chicken leg"},
                {"chicken_hat","Chicken Viking Hat"},
                {"chicken_sombrero","A Sombrero for the chicken"},
                {"chicken_sombrero_descrip","El Pollo Loco"}
            });
        }

        #endregion

    }
}