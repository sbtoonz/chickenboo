using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using CreatureManager;
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
        internal const string ModVersion = "2.1.0";
        internal const string ModGUID = "com.zarboz.ChickenBoo";
        public static ServerSync.ConfigSync configSync = new ServerSync.ConfigSync(ModGUID) { DisplayName = ModName, CurrentVersion = ModVersion };

        internal static ConfigEntry<bool> useRKEggs;
        internal static ConfigEntry<float> MinimumSpawnTimeForEgg;
        internal static ConfigEntry<float> MaximumSpawnTimeForEgg;
        internal static ConfigEntry<int> SpawnVol1;
        internal static ConfigEntry<int> SpawnVol2;
        internal static ConfigEntry<int> SpawnVol3;
        internal static ConfigEntry<float> FeatherChance;
        internal static ConfigEntry<float> EncounterChanceMeadows;
        internal static ConfigEntry<float> EncounterChanceBF;
        internal static ConfigEntry<float> EncounterChancePlains;
        internal static ConfigEntry<int> MaxSpawnedChickensInSpawner;
        internal static ConfigEntry<bool> SpawnThatswitch;
        internal static ConfigEntry<bool> serverConfigLocked;
        
        //translation configentries

        public static ConfigEntry<string> ChickenName;
        public static ConfigEntry<string> RawChickenTranslation;
        public static ConfigEntry<string> RawChickenDescription;
        public static ConfigEntry<string> RawEggName;
        public static ConfigEntry<string> RawEggDescription;
        public static ConfigEntry<string> FriedEggName;
        public static ConfigEntry<string> FriedEggDescription;
        public static ConfigEntry<string> BoiledEggName;
        public static ConfigEntry<string> BoiledEggDescription;
        public static ConfigEntry<string> CookedChickenName;
        public static ConfigEntry<string> CookedChickenDescription;
        public static ConfigEntry<string> ChickenHat;
        public static ConfigEntry<string> ChickenHatDescription;
        public static ConfigEntry<string> Sombrero;
        public static ConfigEntry<string> SombreroDescription;


        internal static RandomEggLayer _eggLayer;
        internal static Harmony _harmony;
        public static GameObject chiken { get; internal set; }
        public static GameObject coolhat { get; internal set; }
        public static GameObject sombrero { get; internal set; }
        public static GameObject chicklet { get; internal set; }
        public static GameObject GrilledChicken { get; internal set; }
        public static GameObject FriedEgg { get; internal set; }
        public static GameObject BoiledEgg { get; internal set; }
        internal static GameObject RawEgg { get; set; }
        public static GameObject RawChicken { get; set; }
        
        internal static GameObject? RK_Egg { get; set; }

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
            var test = new Creature(chiken)
            {
                Biome = Heightmap.Biome.Meadows,
                GroupSize = new Range(2,30),
                CheckSpawnInterval = 100,
                ConfigurationEnabled = true,
                CanSpawn = true,
                Maximum = 50,
                ForestSpawn = Forest.Yes,
                CanBeTamed = true,
                CanHaveStars = true,
               
                
            };
            test.Localize().English("ChickenBoo");
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
            coolhat = assetBundle.LoadAsset<GameObject>("helmet");
            sombrero = assetBundle.LoadAsset<GameObject>("chickensombrero");
            

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

            useRKEggs = config("Chicken", "Use RK_Eggs from BoneAppetite", false,
                "set this to true if you want to use the eggs from BoneAppetite instead of the ones that came with the mod");
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

            EncounterChanceMeadows = Config.Bind("Chicken", "Encounter Chance Meadows", 0.25f,
                new ConfigDescription(
                    "This number is show as a decimal. It is interpreted as a percent so 1 = 100% and .25 = 25% chance etc"));
            
            EncounterChancePlains = Config.Bind("Chicken", "Encounter Chance Plains", 0.25f,
                new ConfigDescription(
                    "This number is show as a decimal. It is interpreted as a percent so 1 = 100% and .25 = 25% chance etc"));
            
            EncounterChanceBF = Config.Bind("Chicken", "Encounter Chance Black Forest", 0.25f,
                new ConfigDescription(
                    "This number is show as a decimal. It is interpreted as a percent so 1 = 100% and .25 = 25% chance etc"));
            
            MaxSpawnedChickensInSpawner = Config.Bind("Chicken", "Max Spawned Chickens Per Spawner", 10,
                new ConfigDescription("This is the max number of chickens per spawner that spawn system makes"));

            SpawnThatswitch = Config.Bind("Chicken", "Spawn That", false,
                new ConfigDescription(
                    "Set this to true if you want to disable the spawners built into Chickenboo in favor of using another spawner such as spawnthat"));
            
            ChickenName = config("Translations", "Chicken Name",
                "Chicken", new ConfigDescription("This is the in game name for the chicken"));

            RawEggName = config("Translations","Raw Egg Name", "Raw Egg", new ConfigDescription(""));
            
            RawEggDescription = config("Translations","Raw Egg Description","A fresh egg from your chicken. Great for cooking",new ConfigDescription(""));
            
            FriedEggName = config("Translations","Fried Egg Name","Fried Egg",new ConfigDescription(""));
            
            FriedEggDescription = config("Translations","","",new ConfigDescription(""));
            
            BoiledEggName = config("Translations","Boiled Egg Description","A boiled egg for any time of day!",new ConfigDescription(""));
            
            BoiledEggDescription = config("Translations","Boiled Egg Name","A Boiled Egg",new ConfigDescription(""));
            
            CookedChickenName = config("Translations","Grilled Chicken Name","Grilled Chicken",new ConfigDescription(""));
            
            CookedChickenDescription = config("Translations","Grilled Chicken Description","A nice grilled chicken leg",new ConfigDescription(""));
            
            ChickenHat = config("Translations","Viking Hat Name","Chicken Viking Hat",new ConfigDescription(""));
            
            ChickenHatDescription = config("Translations","Viking Hat description","Chicken Viking Hat",new ConfigDescription(""));
            
            Sombrero = config("Translations","Sombrero Name","A Sombrero for the chicken",new ConfigDescription(""));
            
            SombreroDescription = config("Translations","Sombrero Description","El Pollo Loco",new ConfigDescription(""));

            RawChickenTranslation = config("Translations", "Raw Chicken Name", "Raw Chicken", new ConfigDescription(""));

            RawChickenDescription =
                config("Translations", "Raw Chicken Description", "Raw chicken meat for cooking.", new ConfigDescription(""));
        }

        public static void SetupConsumables()
        {
            _eggLayer._monsterAI.m_consumeItems = new List<ItemDrop>
            {
                ObjectDB.instance.GetItemPrefab("Dandelion").GetComponent<ItemDrop>()
            };
        }
    }
}