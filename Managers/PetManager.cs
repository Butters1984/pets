using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using CreatureManager;
using HarmonyLib;
using ItemManager;
using MountPets.Behaviors;
using pets;
using UnityEngine;

namespace MountPets.Managers;

public class PetManager
{
    private static readonly List<MonsterAI> m_petInstances = new();
    private static readonly List<MonsterAI> m_petsToDespawn = new();
    private static readonly Dictionary<string, Pet> CreaturePetMap = new();
    private static readonly Dictionary<Tameable, ItemDrop.ItemData?> m_tameItemMap = new();
    private static readonly Dictionary<string, Pet> SharedNameItemPetMap = new();

    private static Localization? _english;
    private static Localization english => _english ??= LocalizationCache.ForLanguage("English");

    static PetManager()
    {
        Harmony harmony = new Harmony("org.bepinex.helpers.PetManager");
        harmony.Patch(AccessTools.DeclaredMethod(typeof(ZNetScene), nameof(ZNetScene.Awake)), postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(PetManager), nameof(Patch_ZNetScene_Awake))));
        harmony.Patch(AccessTools.DeclaredMethod(typeof(InventoryGrid), nameof(InventoryGrid.CreateItemTooltip)), prefix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(PetManager), nameof(Patch_CreateItemTooltip))));
        harmony.Patch(AccessTools.DeclaredMethod(typeof(InventoryGui), nameof(InventoryGui.OnOpenTexts)), prefix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(PetManager), nameof(Patch_InventoryGUI_OnOpenTexts))));
        harmony.Patch(AccessTools.DeclaredMethod(typeof(Humanoid), nameof(Humanoid.EquipItem)), postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(PetManager), nameof(Patch_Humanoid_EquipItem))));
        harmony.Patch(AccessTools.DeclaredMethod(typeof(Humanoid), nameof(Humanoid.UnequipItem)), postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(PetManager), nameof(Patch_Humanoid_UnEquipItem))));
        harmony.Patch(AccessTools.DeclaredMethod(typeof(BaseAI), nameof(BaseAI.UpdateAI)), prefix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(PetManager), nameof(Patch_BaseAI_UpdateAI))));
        harmony.Patch(AccessTools.DeclaredMethod(typeof(BaseAI), nameof(BaseAI.OnEnable)), postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(PetManager), nameof(Patch_BaseAI_OnEnable))));
        harmony.Patch(AccessTools.DeclaredMethod(typeof(BaseAI), nameof(BaseAI.OnDisable)), postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(PetManager), nameof(Patch_BaseAI_OnDisable))));
        harmony.Patch(AccessTools.DeclaredMethod(typeof(BaseAI), nameof(BaseAI.IsEnemy)), prefix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(PetManager), nameof(Patch_BaseAI_IsEnemy))));
        harmony.Patch(AccessTools.DeclaredMethod(typeof(Tameable), nameof(Tameable.RPC_SetName)), postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(PetManager), nameof(Patch_RPC_SetName))));
    }

    // Loaded right after Item Manager patch_fejdstartup to make sure the item is registered and has all the variables set
    public static void Patch_FejdStartup()
    {
        foreach(Pet pet in CreaturePetMap.Values) pet.Setup();
    }
    // Loaded after ZNetScene Awake to grab any assets from the scene
    private static void Patch_ZNetScene_Awake()
    {
        foreach(var pet in CreaturePetMap.Values) pet.Load();
    }
    // To make sure the spawn item status effect tooltip represents the correct values multiplied by item level
    private static void Patch_CreateItemTooltip(ItemDrop.ItemData item) => SE_Pet.m_tempCompanionItem = item;
    // To reset temporary item to the current item
    private static void Patch_InventoryGUI_OnOpenTexts() => SE_Pet.m_tempCompanionItem = SE_Pet.m_currentCompanionItem;
    private static void Patch_Humanoid_EquipItem(Humanoid __instance, ItemDrop.ItemData item, bool __result)
    {
        if (__instance is not Player player || !__result || !SharedNameItemPetMap.TryGetValue(item.m_shared.m_name, out Pet pet)) return;
        DespawnCompanions(player);
        // Spawn pet
        GameObject prefab = UnityEngine.Object.Instantiate(pet.creature.Prefab, FindSpawnPoint(player.transform.position, pet.SpawnRange, pet.IsFlying), Quaternion.identity);
        // Create effects
        pet.m_spawnEffects.Create(prefab.transform.position, prefab.transform.rotation);
        if (prefab.TryGetComponent(out Tameable tameable))
        {
            tameable.Tame();
            tameable.Command(__instance, false);
            // Set new creature values based on item data
            if (item.m_customData.TryGetValue("PetName", out string tameName)) tameable.SetText(tameName);
            tameable.m_character.SetLevel(item.m_quality);
            // Record the current item for tooltips
            SE_Pet.m_currentCompanionItem = item;
            // Record the tame and item to record pet name onto item
            m_tameItemMap[tameable] = item;
            // Update the status effect based on the current item quality
            UpdateSE();
        }
    }
    private static void Patch_Humanoid_UnEquipItem(Humanoid __instance, ItemDrop.ItemData? item)
    {
        if (__instance is not Player player || item == null || !SharedNameItemPetMap.ContainsKey(item.m_shared.m_name)) return;
        DespawnCompanions(player);
    }
    private static Vector3 FindSpawnPoint(Vector3 point, float distance, bool isFlying)
    {
        if (point.y > 3000f) return point;
        Vector2 range = UnityEngine.Random.insideUnitCircle * distance;
        Vector3 spawnPoint = point + new Vector3(range.x, 0.0f, range.y);
        if (ZoneSystem.instance.GetSolidHeight(spawnPoint, out float height))
        {
            spawnPoint.y = height;
        }
        if (isFlying) spawnPoint.y += 10f;
        return spawnPoint;
    }
    private static void Patch_RPC_SetName(Tameable __instance, string name)
    {
        if (!IsPet(__instance.name) || !m_tameItemMap.TryGetValue(__instance, out ItemDrop.ItemData? item) || item == null) return;
        item.m_customData["PetName"] = name;
    }
    private static bool Patch_BaseAI_UpdateAI(BaseAI __instance, float dt, ref bool __result)
    {
        if (!m_petsToDespawn.Contains(__instance)) return true;
        __instance.MoveAwayAndDespawn(dt, true);
        __result = false;
        return false;
    }
    private static void DespawnCompanions(Player player)
    {
        m_petsToDespawn.Clear();
        m_tameItemMap.Clear();
        foreach (var pet in m_petInstances)
        {
            if (pet.GetFollowTarget() is {} followTarget && followTarget.TryGetComponent(out Player followPlayer) && followPlayer == player)
            {
                m_petsToDespawn.Add(pet);
            }
        }
    }
    private static void Patch_BaseAI_OnEnable(BaseAI __instance)
    {
        if (__instance is not MonsterAI monsterAI || !IsPet(__instance.name)) return;
        m_petInstances.Add(monsterAI);
    }
    private static void Patch_BaseAI_OnDisable(BaseAI __instance)
    {
        if (__instance is not MonsterAI monsterAI || !IsPet(__instance.name)) return;
        m_petInstances.Remove(monsterAI);
    }
    private static bool IsPet(string prefabID) => CreaturePetMap.ContainsKey(prefabID.Replace("(Clone)", string.Empty));
    private static bool Patch_BaseAI_IsEnemy(Character a, Character b, ref bool __result)
    {
        if (!IsPet(a.name) && !IsPet(b.name)) return true;
        __result = false;
        return false;
    }
    
    private static void UpdateSE()
    {
        if (!Player.m_localPlayer) return;
        foreach (var se in Player.m_localPlayer.GetSEMan().GetStatusEffects())
        {
            if (se is SE_Pet sePet) sePet.SetConfigData();
        }
    }

    private static void UpdateEffects(List<string> effects, ref EffectList effectList)
    {
        if (effects.Count <= 0 || !ZNetScene.instance) return;
        List<EffectList.EffectData> data = effectList.m_effectPrefabs.ToList();
        foreach (string effect in effects)
        {
            if (ZNetScene.instance.GetPrefab(effect) is not { } prefab) continue;
            data.Add(new EffectList.EffectData() { m_prefab = prefab });
        }

        effectList.m_effectPrefabs = data.ToArray();
    }

    public class Pet
    {
        public readonly Creature creature;
        public readonly Item item;
        public float SpawnRange = 50f;
        public bool IsFlying;
        public EffectList m_spawnEffects = new();
        public readonly List<string> SpawnEffects = new();
        public readonly SE_Pet EquipStatusEffect;

        public Pet(string assetBundle, string creatureName, string itemName)
        {
            creature = new Creature(assetBundle, creatureName);
            item = new Item(assetBundle, itemName);
            EquipStatusEffect = ScriptableObject.CreateInstance<SE_Pet>();
            EquipStatusEffect.name = "SE_" + item.Prefab.name;
            item.Prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_equipStatusEffect = EquipStatusEffect;
            CreaturePetMap[creature.Prefab.name] = this;
        }

        public void Load()
        {
            UpdateEffects(SpawnEffects, ref m_spawnEffects);
        }

        public void Setup()
        {
            string nameKey = item.Prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_name;
            string englishName = new Regex(@"[=\n\t\\""\'\[\]]*").Replace(english.Localize(nameKey), "").Trim();
            SharedNameItemPetMap[nameKey] = this;
            SetupSE(englishName);
        }

        private void SetupSE(string englishName)
        {
            EquipStatusEffect.config.ItemQualityMultiplier = petsPlugin.Plugin.config(englishName, "0. Item Quality Multiplier", 0.1f, new ConfigDescription("Set the item quality multiplier", new AcceptableValueRange<float>(0f, 1f)));
	        EquipStatusEffect.config.HealthRegen = petsPlugin.Plugin.config(englishName, "1. Health Regen", 1f, new ConfigDescription("Set health regen multiplier", new AcceptableValueRange<float>(1f, 10f)));
	        EquipStatusEffect.config.StaminaRegen = petsPlugin.Plugin.config(englishName, "2. Stamina Regen", 1f, new ConfigDescription("Set stamina regen multiplier", new AcceptableValueRange<float>(1f, 10f)));
	        EquipStatusEffect.config.EitrRegen = petsPlugin.Plugin.config(englishName, "3. Eitr Regen", 1f, new ConfigDescription("Set eitr regen multiplier", new AcceptableValueRange<float>(1f, 10f)));
	        EquipStatusEffect.config.CarryWeight = petsPlugin.Plugin.config(englishName, "4. Carry Weight", 0f, new ConfigDescription("Set carry weight bonus", new AcceptableValueRange<float>(0f, 500f)));
	        EquipStatusEffect.config.Speed = petsPlugin.Plugin.config(englishName, "5. Speed", 0f, new ConfigDescription("Set speed multiplier", new AcceptableValueRange<float>(0f, 2f)));
	        EquipStatusEffect.config.Jump = petsPlugin.Plugin.config(englishName, "6. Jump", 0f, new ConfigDescription("Set jump modifier", new AcceptableValueRange<float>(0f, 10f)));
	        EquipStatusEffect.config.MaxFallSpeed = petsPlugin.Plugin.config(englishName, "7. Max Fall Speed", 0f, new ConfigDescription("Set max fall speed", new AcceptableValueRange<float>(0f, 20f)));
	        void ConfigChanged(object sender, EventArgs args) => UpdateSE();
	        EquipStatusEffect.config.HealthRegen.SettingChanged += ConfigChanged;
	        EquipStatusEffect.config.StaminaRegen.SettingChanged += ConfigChanged;
	        EquipStatusEffect.config.EitrRegen.SettingChanged += ConfigChanged;
	        EquipStatusEffect.config.CarryWeight.SettingChanged += ConfigChanged;
	        EquipStatusEffect.config.Speed.SettingChanged += ConfigChanged;
	        EquipStatusEffect.config.ItemQualityMultiplier.SettingChanged += ConfigChanged;
	        EquipStatusEffect.config.Jump.SettingChanged += ConfigChanged;
	        EquipStatusEffect.config.MaxFallSpeed.SettingChanged += ConfigChanged;
        }
    }
}