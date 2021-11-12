using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using UnityEngine;
using static ChickenBoo.Utilities;

namespace ChickenBoo
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class ChickenBoo : BaseUnityPlugin
    {
        internal const string ModName = "ChickenBoo";
        internal const string ModVersion = "1.0";
        internal const string ModGUID = "com.zarboz.ChickenBoo";
        public static ServerSync.ConfigSync configSync = new ServerSync.ConfigSync(ModGUID) { DisplayName = ModName, CurrentVersion = ModVersion };
        
        internal static ConfigEntry<float> MinimumSpawnTimeForEgg;
        internal static ConfigEntry<float> MaximumSpawnTimeForEgg;
        internal static ConfigEntry<int> SpawnVol1;
        internal static ConfigEntry<int> SpawnVol2;
        internal static ConfigEntry<int> SpawnVol3;
        private static ConfigEntry<float> FeatherChance;
        private static ConfigEntry<float> EncounterChanceMeadows;
        private static ConfigEntry<float> EncounterChanceBF;
        private static ConfigEntry<float> EncounterChancePlains;
        private static ConfigEntry<int> MaxSpawnedChickensInSpawner;
        private static ConfigEntry<bool> SpawnThatswitch;
        public static ConfigEntry<bool> serverConfigLocked;
        
        
        internal RandomEggLayer _eggLayer;
        internal static Harmony _harmony;
        public static GameObject chiken { get; internal set; }
        public static GameObject coolhat { get; internal set; }
        public static GameObject sombrero { get; internal set; }
        public static GameObject chicklet { get; internal set; }
        public static GameObject GrilledChicken { get; internal set; }
        public static GameObject FriedEgg { get; internal set; }
        public static GameObject BoiledEgg { get; internal set; }
        internal static GameObject RawEgg { get; set; }
        public GameObject RawChicken { get; set; }

        private static AssetBundle? assetBundle;
        
        internal static Recipe sombrerorecipe;
        internal static Recipe vikinghatrecipe;
        
        ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        ConfigEntry<T> config<T>(string group, string name, T value, string description, bool synchronizedSetting = true) => config(group, name, value, new ConfigDescription(description), synchronizedSetting);

        public void Awake()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Harmony harmony = new(ModGUID);
            harmony.PatchAll(assembly);
            LoadAssets();
            SetupConfigs();
        }
        
        private void LoadAssets()
        {
            assetBundle = LoadAssetBundle("chickenboo");
            chiken = assetBundle.LoadAsset<GameObject>("ChickenBoo");
            chicklet = assetBundle.LoadAsset<GameObject>("chicklet");
            GrilledChicken = assetBundle.LoadAsset<GameObject>("cooked_chicken");
            FriedEgg = assetBundle.LoadAsset<GameObject>("fried_egg");
            BoiledEgg = assetBundle.LoadAsset<GameObject>("boiled_egg");
            RawEgg = assetBundle.LoadAsset<GameObject>("raw_egg");
            RawChicken = assetBundle.LoadAsset<GameObject>("raw_chicken");
        }

        internal static void AddtoCharDrops()
        {
            var chardrop = chiken.GetComponent<CharacterDrop>();
            chardrop.m_drops.Add(new CharacterDrop.Drop
            {
                m_prefab = ZNetScene.instance.GetPrefab("Feathers"),
                m_chance = FeatherChance.Value,
                m_amountMax = 6,
                m_amountMin = 1,
                m_levelMultiplier = true,
                m_onePerPlayer = true
            });
        }

        internal static void LoadHats()
        {
            coolhat = assetBundle.LoadAsset<GameObject>("helmet");
            vikinghatrecipe = RecipeMaker(1, coolhat.GetComponent<ItemDrop>(), Station("piece_workbench"),
                Station("piece_workbench"), 0,
                new Piece.Requirement[]
                {
                    new()
                    {
                        m_amount = 1,
                        m_recover = false,
                        m_resItem = ZNetScene.instance.GetPrefab("Bronze").GetComponent<ItemDrop>(),
                        m_amountPerLevel = 0,
                    },
                    new ()
                    {
                        m_amount = 1,
                        m_recover = false,
                        m_resItem = ZNetScene.instance.GetPrefab("LeatherScraps").GetComponent<ItemDrop>(),
                        m_amountPerLevel = 0,
                    }
                });

            sombrero = assetBundle.LoadAsset<GameObject>("chickensombrero");
            sombrerorecipe = RecipeMaker(1, sombrero.GetComponent<ItemDrop>(),
                Station("piece_workbench"), Station("piece_workbench"), 0, new Piece.Requirement[]
                {
                    new()
                    {
                        m_amount = 1,
                        m_recover = false,
                        m_resItem = ZNetScene.instance.GetPrefab("LeatherScraps").GetComponent<ItemDrop>(),
                        m_amountPerLevel = 0
                    }
                });
        }
         private void SetupConfigs()
        {
            Config.SaveOnConfigSet = true;

            serverConfigLocked = config("General", "Lock Configuration", false, "Lock Configuration");
            configSync.AddLockingConfigEntry<bool>(serverConfigLocked);
            
            MinimumSpawnTimeForEgg = config("Chicken", "Egg Spawn Time Min", 150f, new ConfigDescription(
                "This is the minimum random volume of time in the range of time to select",
                new AcceptableValueRange<float>(15f, 1000f)));


            MaximumSpawnTimeForEgg = config("Chicken", "Egg Spawn Time Max", 450f, new ConfigDescription(
                "This is the maximum random volume of time in the range of time to select",
                new AcceptableValueRange<float>(15f, 1000f)));


            SpawnVol1 = config("Chicken", "Egg Spawn Count 1", 1, new ConfigDescription(
                "This is the volume of eggs that will be laid when random selection chooses a value < .45 which in theory is 45% of the time",
                new AcceptableValueRange<int>(1, 1000)));


            SpawnVol2 = config("Chicken", "Egg Spawn Count 2", 6, new ConfigDescription(
                "This is the volume of eggs that will be laid when random selection chooses a value < .9 which in theory is 45% of the time",
                new AcceptableValueRange<int>(1, 1000)));


            SpawnVol3 = config("Chicken", "Egg Spawn Count 3", 12, new ConfigDescription(
                "This is the volume of eggs that will be laid the remaining 10% of the selection ranges",
                new AcceptableValueRange<int>(1, 1000)));

            FeatherChance = config("Chicken", "Feather Drop Chance", 0.5f,new ConfigDescription(
                "This is a representation of percent chance in number format that feathers will drop from the chicken", null));

        }
    }
}