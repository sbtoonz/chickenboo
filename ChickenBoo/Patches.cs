using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace ChickenBoo
{
    public class Patches
    {
        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        public static class ZnetAwakePatch
        {
            public static void Prefix(ZNetScene __instance)
            {
                if (__instance.m_prefabs.Count <= 0 ) return;
                
                Utilities.AddtoZnet(ChickenBoo.chicklet, __instance);
                Utilities.AddtoZnet(ChickenBoo.chiken, __instance);
                Utilities.AddtoZnet(ChickenBoo.GrilledChicken, __instance);
                Utilities.AddtoZnet(ChickenBoo.FriedEgg, __instance);
                Utilities.AddtoZnet(ChickenBoo.BoiledEgg, __instance);
                Utilities.AddtoZnet(ChickenBoo.RawEgg, __instance);
            }
        }

        [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
        public static class ObjectDBAwakePatch
        {
            public static void Prefix(ObjectDB __instance)
            {
                if (__instance.m_items.Count <= 0 || __instance.GetItemPrefab("Wood") == null) return;
                ChickenBoo.LoadHats();
                ChickenBoo.AddtoCharDrops();
                __instance.m_items.Add(ChickenBoo.GrilledChicken);
                __instance.m_items.Add(ChickenBoo.RawEgg);
                __instance.m_items.Add(ChickenBoo.BoiledEgg);
                __instance.m_items.Add(ChickenBoo.FriedEgg);
                __instance.m_items.Add(ChickenBoo.RawChicken);
                
                __instance.m_items.Add(ChickenBoo.coolhat);
                __instance.m_recipes.Add(ChickenBoo.vikinghatrecipe);
                __instance.m_items.Add(ChickenBoo.sombrero);
                __instance.m_recipes.Add(ChickenBoo.sombrerorecipe);
                
                var station1 = ZNetScene.instance.GetPrefab("piece_cookingstation");
                var conversion1 = station1.GetComponent<CookingStation>();
                conversion1.m_conversion.Add(
                    new()
                    {
                        m_from = __instance.GetItemPrefab("raw_chicken").GetComponent<ItemDrop>(),
                        m_to = __instance.GetItemPrefab("cooked_chicken").GetComponent<ItemDrop>(),
                        m_cookTime = 15f
                    });

                var station2 = ZNetScene.instance.GetPrefab("piece_cookingstation_iron");
                var conversion2 = station2.GetComponent<CookingStation>();
                conversion2.m_conversion.Add(
                    new CookingStation.ItemConversion
                    {
                        m_from = __instance.GetItemPrefab("raw_chicken").GetComponent<ItemDrop>(),
                        m_to = __instance.GetItemPrefab("cooked_chicken").GetComponent<ItemDrop>(),
                        m_cookTime = 15f
                    });
            }
        }

        [HarmonyPriority(Priority.HigherThanNormal)]
        [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.CopyOtherDB))]
        public static class CopyOtherDBPatch
        {
            public static void Prefix(ObjectDB __instance)
            {
                if (__instance.m_items.Count <= 0 || __instance.GetItemPrefab("Wood") == null) return;
                ChickenBoo.LoadHats();
                ChickenBoo.AddtoCharDrops();
                __instance.m_items.Add(ChickenBoo.GrilledChicken);
                __instance.m_items.Add(ChickenBoo.RawEgg);
                __instance.m_items.Add(ChickenBoo.BoiledEgg);
                __instance.m_items.Add(ChickenBoo.FriedEgg);
                __instance.m_items.Add(ChickenBoo.RawChicken);

                __instance.m_items.Add(ChickenBoo.coolhat);
                __instance.m_recipes.Add(ChickenBoo.vikinghatrecipe);
                __instance.m_items.Add(ChickenBoo.sombrero);
                __instance.m_recipes.Add(ChickenBoo.sombrerorecipe);
                
                var station1 = ZNetScene.instance.GetPrefab("piece_cookingstation");
                var conversion1 = station1.GetComponent<CookingStation>();
                conversion1.m_conversion.Add(
                    new()
                    {
                        m_from = __instance.GetItemPrefab("raw_chicken").GetComponent<ItemDrop>(),
                        m_to = __instance.GetItemPrefab("cooked_chicken").GetComponent<ItemDrop>(),
                        m_cookTime = 15f
                    });

                var station2 = ZNetScene.instance.GetPrefab("piece_cookingstation_iron");
                var conversion2 = station2.GetComponent<CookingStation>();
                conversion2.m_conversion.Add(
                    new CookingStation.ItemConversion
                    {
                        m_from = __instance.GetItemPrefab("raw_chicken").GetComponent<ItemDrop>(),
                        m_to = __instance.GetItemPrefab("cooked_chicken").GetComponent<ItemDrop>(),
                        m_cookTime = 15f
                    });

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
                        tmp.m_prefab = ChickenBoo.chiken;
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
        
        [HarmonyPatch(typeof(Tameable), nameof(Tameable.Interact))]
        public static class InteractPatch
        {
            public static bool Prefix(Tameable __instance)
            {
                if (__instance.gameObject.GetComponent<RandomEggLayer>() != null)
                {
                    if (Input.GetKey(KeyCode.LeftAlt))
                    {
                        if(__instance.gameObject.GetComponent<RandomEggLayer>()._helmetMounter.HelmetMounted)
                            return false;
                        var userinv = Player.m_localPlayer.GetInventory();
                        ItemDrop.ItemData tmphat = null;
                        foreach (var item in userinv.m_inventory)
                        {
                            if (item.m_shared.m_name == "$chicken_hat")
                            {
                                tmphat = userinv.GetItem("$chicken_hat");
                            }
                        }
                        if(userinv.RemoveOneItem(tmphat))
                        {
                            var hm = __instance.gameObject.GetComponent<RandomEggLayer>()._helmetMounter;
                            hm.HelmetObject.SetActive(true);
                            var znv = __instance.GetComponent<ZNetView>();
                            znv.m_zdo.Set("$chicken_hat", true);
                            hm.HelmetMounted = true;
                            ZInput.ResetButtonStatus("Use");
                        }
                        return false;
                    }

                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        if(__instance.gameObject.GetComponent<RandomEggLayer>()._helmetMounter.HelmetMounted)
                                return false;
                        var userinv = Player.m_localPlayer.GetInventory();
                        ItemDrop.ItemData tmphat = null;
                        foreach (var item in userinv.m_inventory)
                        {
                            if (item.m_shared.m_name == "$chicken_sombrero")
                            {
                                tmphat = userinv.GetItem("$chicken_sombrero");
                            }
                            
                        }
                        
                        if (userinv.RemoveOneItem(tmphat))
                        {
                            var hm = __instance.gameObject.GetComponent<RandomEggLayer>()._helmetMounter;
                            hm.GravesSombrero.SetActive(true);
                            var znv = __instance.GetComponent<ZNetView>();
                            znv.m_zdo.Set("$chicken_sombrero", true);
                            hm.HelmetMounted = true;
                            ZInput.ResetButtonStatus("Use");
                        }
                        return false;
                        
                    }
                }

                return true;
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

        [HarmonyPatch(typeof(Localize), nameof(Localize.Awake))]
        public static class LocalizationPatch
        {
            public static void Prefix()
            {
                Localization.instance.AddWord("enemy_chicken","Chicken");
                Localization.instance.AddWord("raw_egg","Raw Egg");
                Localization.instance.AddWord("raw_egg_descrip","A fresh egg from your chicken. Great for cooking");
                Localization.instance.AddWord("raw_chicken","Raw Chicken");
                Localization.instance.AddWord("raw_chicken_descrip","Raw chicken meat for cooking.");
                Localization.instance.AddWord("boiled_egg","A Boiled Egg");
                Localization.instance.AddWord("boiled_egg_descrip","A hard boiled egg, nutritious and delicious");
                Localization.instance.AddWord("fried_egg","Fried Egg");
                Localization.instance.AddWord("fried_egg_descrip","A fried egg for any time of day!");
                Localization.instance.AddWord("cooked_chicken","Grilled Chicken");
                Localization.instance.AddWord("cooked_chicken_descrip", "A nice grilled chicken leg");
                Localization.instance.AddWord("chicken_hat","Chicken Viking Hat");
                Localization.instance.AddWord("chicken_sombrero","A Sombrero for the chicken");
                Localization.instance.AddWord("chicken_sombrero_descrip","El Pollo Loco");
            }
        }

    }
}