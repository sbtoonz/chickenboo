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
        internal const string ModVersion = "2.1.5";
        internal const string ModGUID = "com.zarboz.ChickenBoo";
        internal static ConfigSync configSync = new(ModGUID)
        {
            DisplayName = ModName,
            CurrentVersion = ModVersion, 
            MinimumRequiredVersion = ModVersion,
        };
        internal static ConfigEntry<bool> useRKEggs;
        internal static ConfigEntry<float> MinimumSpawnTimeForEgg;
        internal static ConfigEntry<float> MaximumSpawnTimeForEgg;
        internal static ConfigEntry<int> SpawnVol1;
        internal static ConfigEntry<int> SpawnVol2;
        internal static ConfigEntry<int> SpawnVol3;
        private static ConfigEntry<float> FeatherChance;
        private static ConfigEntry<bool> serverConfigLocked;
        
        //translation configentries

        public static ConfigEntry<string> ChickenName;
        public static ConfigEntry<string> ChickletName;
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
        private static ConfigEntry<string> ChickenHatDescription;
        public static ConfigEntry<string> Sombrero;
        public static ConfigEntry<string> SombreroDescription;

#pragma warning disable CS0649
        private static RandomEggLayer? _eggLayer;
        internal static Harmony? _harmony;
#pragma warning restore CS0649
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
        
        internal static Recipe? sombrerorecipe;
        internal static Recipe? vikinghatrecipe;
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
            var test = new Creature("chickenboo", "ChickenBoo")
            {
                Biome = Heightmap.Biome.Meadows,
                SpawnChance = 50f,
                GroupSize = new Range(2f, 3f),
                CheckSpawnInterval = 1400,
                CanSpawn = true,
                Maximum = 5,
                FoodItems = "Dandelion, Blueberries, Raspberry, Acorn, OnionSeeds, CarrotSeeds, TurnipSeeds",
                ForestSpawn = Forest.Yes,
                CanBeTamed = true,
                CanHaveStars = true,
                SpecificSpawnTime = SpawnTime.Day,
            };
            test.Localize().English("ChickenBoo");
            chiken = test.Prefab;
        }
        
        private void LoadAssets()
        {
            assetBundle = LoadAssetBundle("chickenboo");
            chicklet = assetBundle?.LoadAsset<GameObject>("chicklet")!;
            GrilledChicken = assetBundle?.LoadAsset<GameObject>("cooked_chicken")!;
            FriedEgg = assetBundle?.LoadAsset<GameObject>("fried_egg")!;
            BoiledEgg = assetBundle?.LoadAsset<GameObject>("boiled_egg")!;
            RawEgg = assetBundle?.LoadAsset<GameObject>("raw_egg")!;
            RawChicken = assetBundle?.LoadAsset<GameObject>("raw_chicken")!;
            coolhat = assetBundle?.LoadAsset<GameObject>("helmet")!;
            sombrero = assetBundle?.LoadAsset<GameObject>("chickensombrero")!;
            assetBundle?.Unload(false);

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
            
            ChickenName = config("Translations", "Chicken Name",
                "Chicken", new ConfigDescription("This is the in game name for the chicken"));
            
            ChickletName = config("Translations", "Chicklet Name",
                "Chicklet", new ConfigDescription("This is the in game name for the baby chicken"));

            RawEggName = config("Translations","Raw Egg Name", "Raw Egg", new ConfigDescription(""));
            
            RawEggDescription = config("Translations","Raw Egg Description","A fresh egg from your chicken. Great for cooking",new ConfigDescription(""));
            
            FriedEggName = config("Translations","Fried Egg Name","Fried Egg",new ConfigDescription(""));
            
            FriedEggDescription = config("Translations","Fried Egg Description", "A fried egg for any time of day!", new ConfigDescription(""));
            
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
            _eggLayer!._monsterAI.m_consumeItems = new List<ItemDrop>
            {
                ObjectDB.instance.GetItemPrefab("Dandelion").GetComponent<ItemDrop>()
            };
        }
    }
}