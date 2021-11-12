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
                __instance.m_items.Add(ChickenBoo.coolhat);
                __instance.m_recipes.Add(ChickenBoo.vikinghatrecipe);
                __instance.m_items.Add(ChickenBoo.sombrero);
                __instance.m_recipes.Add(ChickenBoo.sombrerorecipe);
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
                __instance.m_items.Add(ChickenBoo.coolhat);
                __instance.m_recipes.Add(ChickenBoo.vikinghatrecipe);
                __instance.m_items.Add(ChickenBoo.sombrero);
                __instance.m_recipes.Add(ChickenBoo.sombrerorecipe);
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

        [HarmonyPatch(typeof(CookingStation), nameof(CookingStation.Awake))]
        public static class CookingStationPatch
        {
            public static void Prefix(CookingStation __instance)
            {
                switch (__instance.m_name)
                {
                    case "piece_cookingstation_iron":
                        var tmp = __instance.m_conversion;
                        tmp.Add(new CookingStation.ItemConversion
                        {
                           m_from = ZNetScene.instance.GetPrefab("raw_chicken").GetComponent<ItemDrop>(),
                           m_to = ChickenBoo.GrilledChicken.GetComponent<ItemDrop>(),
                           m_cookTime = 15f
                        });
                        break;
                    case "piece_cookingstation":
                        var tmp2 = __instance.m_conversion;
                        tmp2.Add(new CookingStation.ItemConversion
                        {
                            m_from = ZNetScene.instance.GetPrefab("raw_chicken").GetComponent<ItemDrop>(),
                            m_to = ChickenBoo.GrilledChicken.GetComponent<ItemDrop>(),
                            m_cookTime = 15f 
                        });
                        break;
                }
            }
        }
    }
}