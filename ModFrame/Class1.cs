using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace ModFrame
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class NewMod : BaseUnityPlugin
    {
        private const string ModName = "New Mod";
        private const string ModVersion = "1.0";
        private const string ModGUID = "some.new.guid";
    
        public void Awake()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Harmony harmony = new(ModGUID);
            harmony.PatchAll(assembly);
        }
    }
}