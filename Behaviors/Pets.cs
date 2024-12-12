using System.Collections.Generic;
using HarmonyLib;

namespace pets.Behaviors;

public static class Pets
{
    public static readonly List<string> RegisteredPets = new();
    private static bool IsPet(string name) => RegisteredPets.Contains(name.Replace("(Clone)", string.Empty));
    
    [HarmonyPatch(typeof(BaseAI), nameof(BaseAI.IsEnemy),typeof(Character))]
    private static class BaseAI_IsEnemy_Patch
    {
        private static bool Prefix(Character other, ref bool __result)
        {
            if (!IsPet(other.name)) return true;
            __result = false;
            return false;
        }
    }
    
    
}