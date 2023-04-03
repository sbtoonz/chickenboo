using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace ChickenBoo
{
    public class Patches
    {
        
        private static Recipe? _recipeFriedEgg;
        private static Recipe? _recipeBoiledEgg;
        
        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        public static class ZnetAwakePatch
        {
            public static void Prefix(ZNetScene __instance)
            {
                if (__instance.m_prefabs.Count <= 0) return;

                Utilities.AddtoZnet(ChickenBoo.chicklet, __instance);
                Utilities.AddtoZnet(ChickenBoo.GrilledChicken, __instance);
                Utilities.AddtoZnet(ChickenBoo.RawChicken, __instance);
                Utilities.AddtoZnet(ChickenBoo.FriedEgg, __instance);
                Utilities.AddtoZnet(ChickenBoo.BoiledEgg, __instance);
                Utilities.AddtoZnet(ChickenBoo.coolhat, __instance);
                Utilities.AddtoZnet(ChickenBoo.sombrero, __instance);
                if (ChickenBoo.useRKEggs.Value) return;
                Utilities.AddtoZnet(ChickenBoo.RawEgg, __instance);
            }
        }

        [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
        [HarmonyPriority(Priority.Last)]
        public static class ObjectDBAwakePatch
        {

            public static void Prefix(ObjectDB __instance)
            {
                if (__instance.m_items.Count <= 0 || __instance.GetItemPrefab("Wood") == null) return;
                if (ChickenBoo.useRKEggs.Value)
                {
                    ChickenBoo.RK_Egg = __instance.GetItemPrefab("rk_egg");
                    if (ChickenBoo.RK_Egg == null)
                    {
                        Debug.LogError("Failed to load RK_Eggs, Check your BA Install to use this option with ChickenBoo");
                    }
                }
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

                if (ChickenBoo.useRKEggs.Value)
                {
                    _recipeFriedEgg = Utilities.RecipeMaker(1, ChickenBoo.FriedEgg.GetComponent<ItemDrop>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        1,
                        new Piece.Requirement[]
                        {
                            new Piece.Requirement
                            {
                                m_amount = 1,
                                m_amountPerLevel = 0,
                                m_recover = false,
                                m_resItem = ChickenBoo.RK_Egg!.GetComponent<ItemDrop>()
                            }
                        }); 
                }
                else
                {
                    _recipeFriedEgg = Utilities.RecipeMaker(1, ChickenBoo.FriedEgg.GetComponent<ItemDrop>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        1,
                        new Piece.Requirement[]
                        {
                            new Piece.Requirement
                            {
                                m_amount = 1,
                                m_amountPerLevel = 0,
                                m_recover = false,
                                //if BA
                                m_resItem = ChickenBoo.RawEgg.GetComponent<ItemDrop>()
                            }
                        }); 
                }

                if (ChickenBoo.useRKEggs.Value)
                {
                    _recipeBoiledEgg = Utilities.RecipeMaker(1, ChickenBoo.BoiledEgg.GetComponent<ItemDrop>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        1,
                        new Piece.Requirement[]
                        {
                            new Piece.Requirement
                            {
                                m_amount = 1,
                                m_amountPerLevel = 0,
                                m_recover = false,
                                //if BA
                                m_resItem = ChickenBoo.RK_Egg?.GetComponent<ItemDrop>()
                            }
                        });
                }
                else
                {
                    _recipeBoiledEgg = Utilities.RecipeMaker(1, ChickenBoo.BoiledEgg.GetComponent<ItemDrop>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        1,
                        new Piece.Requirement[]
                        {
                            new Piece.Requirement
                            {
                                m_amount = 1,
                                m_amountPerLevel = 0,
                                m_recover = false,
                                //if BA
                                m_resItem = ChickenBoo.RawEgg.GetComponent<ItemDrop>()
                            }
                        });
                }
                __instance.m_recipes.Add(_recipeFriedEgg);
                __instance.m_recipes.Add(_recipeBoiledEgg);
                
            }
        }

        [HarmonyPriority(Priority.Last)]
        [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.CopyOtherDB))]
        public static class CopyOtherDBPatch
        {
            public static void Prefix(ObjectDB __instance)
            {
                if (__instance.m_items.Count <= 0 || __instance.GetItemPrefab("Wood") == null) return;
                if (ChickenBoo.useRKEggs.Value)
                {
                    ChickenBoo.RK_Egg = __instance.GetItemPrefab("rk_egg");
                    if (ChickenBoo.RK_Egg == null)
                    {
                        Debug.LogError("Failed to load RK_Eggs, Check your BA Install to use this option with ChickenBoo");
                    }
                }
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
                
                if (ChickenBoo.useRKEggs.Value)
                {
                    _recipeFriedEgg = Utilities.RecipeMaker(1, ChickenBoo.FriedEgg.GetComponent<ItemDrop>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        1,
                        new Piece.Requirement[]
                        {
                            new Piece.Requirement
                            {
                                m_amount = 1,
                                m_amountPerLevel = 0,
                                m_recover = false,
                                m_resItem = ChickenBoo.RK_Egg!.GetComponent<ItemDrop>()
                            }
                        }); 
                }
                else
                {
                    _recipeFriedEgg = Utilities.RecipeMaker(1, ChickenBoo.FriedEgg.GetComponent<ItemDrop>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        1,
                        new Piece.Requirement[]
                        {
                            new Piece.Requirement
                            {
                                m_amount = 1,
                                m_amountPerLevel = 0,
                                m_recover = false,
                                //if BA
                                m_resItem = ChickenBoo.RawEgg.GetComponent<ItemDrop>()
                            }
                        }); 
                }

                if (ChickenBoo.useRKEggs.Value)
                {
                    _recipeBoiledEgg = Utilities.RecipeMaker(1, ChickenBoo.BoiledEgg.GetComponent<ItemDrop>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        1,
                        new Piece.Requirement[]
                        {
                            new Piece.Requirement
                            {
                                m_amount = 1,
                                m_amountPerLevel = 0,
                                m_recover = false,
                                //if BA
                                m_resItem = ChickenBoo.RK_Egg?.GetComponent<ItemDrop>()
                            }
                        });
                }
                else
                {
                    _recipeBoiledEgg = Utilities.RecipeMaker(1, ChickenBoo.BoiledEgg.GetComponent<ItemDrop>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        ZNetScene.instance.GetPrefab("piece_cauldron").GetComponent<CraftingStation>(),
                        1,
                        new Piece.Requirement[]
                        {
                            new Piece.Requirement
                            {
                                m_amount = 1,
                                m_amountPerLevel = 0,
                                m_recover = false,
                                //if BA
                                m_resItem = ChickenBoo.RawEgg.GetComponent<ItemDrop>()
                            }
                        });
                }
                if(__instance.m_recipes.Count <= 0) return;
                __instance.m_recipes.Add(_recipeFriedEgg);
                __instance.m_recipes.Add(_recipeBoiledEgg);
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
                        ItemDrop.ItemData? tmphat = null;
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
                        ItemDrop.ItemData? tmphat = null;
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

        [HarmonyPatch(typeof(Localize), nameof(Localize.Start))]
        public static class LocalizationPatch
        {
            public static void Prefix()
            {
                Localization.instance.AddWord("enemy_chicken",ChickenBoo.ChickenName.Value);
                Localization.instance.AddWord("enemy_chicklet", ChickenBoo.ChickletName.Value);
                Localization.instance.AddWord("raw_egg",ChickenBoo.RawEggName.Value);
                Localization.instance.AddWord("raw_egg_descrip",ChickenBoo.RawEggDescription.Value);
                Localization.instance.AddWord("raw_chicken",ChickenBoo.RawChickenTranslation.Value);
                Localization.instance.AddWord("raw_chicken_descrip",ChickenBoo.RawChickenDescription.Value);
                Localization.instance.AddWord("boiled_egg",ChickenBoo.BoiledEggName.Value);
                Localization.instance.AddWord("boiled_egg_descrip",ChickenBoo.BoiledEggDescription.Value);
                Localization.instance.AddWord("fried_egg",ChickenBoo.FriedEggName.Value);
                Localization.instance.AddWord("fried_egg_descrip",ChickenBoo.FriedEggDescription.Value);
                Localization.instance.AddWord("cooked_chicken",ChickenBoo.CookedChickenName.Value);
                Localization.instance.AddWord("cooked_chicken_descrip", ChickenBoo.CookedChickenDescription.Value);
                Localization.instance.AddWord("chicken_hat",ChickenBoo.ChickenHat.Value);
                Localization.instance.AddWord("chicken_sombrero",ChickenBoo.Sombrero.Value);
                Localization.instance.AddWord("chicken_sombrero_descrip",ChickenBoo.SombreroDescription.Value);
            }
        }

    }
}