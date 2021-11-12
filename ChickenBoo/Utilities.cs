using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChickenBoo
{
    public class Utilities
    {
        internal static AssetBundle? LoadAssetBundle(string bundleName)
        {
            var resource = typeof(ChickenBoo).Assembly.GetManifestResourceNames().Single
                (s => s.EndsWith(bundleName));
            using var stream = typeof(ChickenBoo).Assembly.GetManifestResourceStream(resource);
            return AssetBundle.LoadFromStream(stream);
        }

        internal static void LoadAssets(AssetBundle? bundle, ZNetScene zNetScene)
        {
            var tmp = bundle?.LoadAllAssets();
            if (zNetScene.m_prefabs.Count <= 0) return;
            if (tmp == null) return;
            foreach (var o in tmp)
            {
                var obj = (GameObject)o;
                zNetScene.m_prefabs.Add(obj);
                var hashcode = obj.GetHashCode();
                zNetScene.m_namedPrefabs.Add(hashcode, obj);
            }
        }

        internal static Recipe RecipeMaker(int ammount, ItemDrop item, CraftingStation craftingStation, 
            CraftingStation repair, int level, Piece.Requirement[] resources)
        {
            Recipe temp = ScriptableObject.CreateInstance<Recipe>();
            //temp = Recipe.CreateInstance<Recipe>();
            temp.m_amount = ammount;
            temp.m_enabled = true;
            temp.m_item = item;
            temp.m_craftingStation = craftingStation;
            temp.m_repairStation = repair;
            temp.m_minStationLevel = level;
            temp.m_resources = resources;


            return temp;
        }

        internal static CraftingStation Station(string name)
        {
            var tmp = ZNetScene.instance.GetPrefab(name);
            return tmp.GetComponent<CraftingStation>();
        }

        internal static void AddtoZnet(GameObject GO, ZNetScene scene)
        {
            var hash = GO.GetHashCode();
            scene.m_prefabs.Add(GO);
            scene.m_namedPrefabs.Add(hash, GO);
        }
    }
}