using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CreatureManager;
using HarmonyLib;
using ItemManager;
using JetBrains.Annotations;
using MountPets.Managers;
using ServerSync;
using UnityEngine;
using CraftingTable = ItemManager.CraftingTable;
namespace pets
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class petsPlugin : BaseUnityPlugin
    {
        internal const string ModName = "MountPets";
        internal const string ModVersion = "1.0.0";
        internal const string Author = "Yggdrah";
        private const string ModGUID = Author + "." + ModName;
        private static readonly string ConfigFileName = ModGUID + ".cfg";
        private static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);
        public static readonly ManualLogSource petsLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);
        private static readonly ConfigSync ConfigSync = new(ModGUID) { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };
        public static petsPlugin Plugin = null!;
        
        public enum Toggle {On, Off}
        
        private static ConfigEntry<Toggle> _serverConfigLocked = null!;

        private void InitConfigs()
        {
            _serverConfigLocked = config("General", "Force Server Config", Toggle.On, "Force Server Config");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);
        }

        public void Awake()
        {
            Plugin = this;
            InitConfigs();

            // Example
            // Use the Pet Manager, to load both Creature and Item within the same class
            // Define your creature using [variable].creature...
            // Define your item using [variable].item...
            // Setup the SE base values of equipping item using: [variable].EquipStatusEffect.config...
            
            PetManager.Pet MountLoxYgg0 = new PetManager.Pet("pets", "Mount_Lox_Ygg0", "Mount_Lox_Ygg0_Item");
            MountLoxYgg0.creature.CanBeTamed = true;
            MountLoxYgg0.creature.FoodItems = "Raspberry";
            MountLoxYgg0.creature.CanSpawn = false;
            MountLoxYgg0.item.Name.English("Lox Token");
            MountLoxYgg0.item.Description.English("Spawns a loyal lox that follows you around");
            MountLoxYgg0.item.Crafting.Add(CraftingTable.Workbench, 1);
            MountLoxYgg0.item.RequiredItems.Add("Coins", 10);
            MountLoxYgg0.EquipStatusEffect.config.CarryWeight.Value = 100f;
            MountLoxYgg0.SpawnEffects.Add("vfx_spawn");
            MountLoxYgg0.SpawnEffects.Add("sfx_spawn");
            
            // LoadCreatures(); // Any creatures that are not pets can be loaded using classic Creature Manager
            LoadItems();
            RegisterEffects();
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private static void LoadCreatures()
        {
            Creature Mount_Lox_Ygg0 = new("pets", "Mount_Lox_Ygg0")
            {
                CanBeTamed = true,
                FoodItems = ("Raspberry"),
                CanSpawn = false,
            };

            Creature Mount_Lox_Ygg1 = new("pets", "Mount_Lox_Ygg1")
            {
                CanBeTamed = true,
                FoodItems = ("Raspberry"),
                CanSpawn = false,
            };

            Creature Mount_Lox_Ygg2 = new("pets", "Mount_Lox_Ygg2")
            {
                CanBeTamed = true,
                FoodItems = ("Raspberry"),
                CanSpawn = false,
            };

            Creature Mount_Lox_Ygg3 = new("pets", "Mount_Lox_Ygg3")
            {
                CanBeTamed = true,
                FoodItems = ("Raspberry"),
                CanSpawn = false,
            };

            Creature Mount_Lox_Ygg4 = new("pets", "Mount_Lox_Ygg4")
            {
                CanBeTamed = true,
                FoodItems = ("Raspberry"),
                CanSpawn = false,
            };


            //Boar//


            Creature Mount_Boar_Ygg = new("pets", "Mount_Boar_Ygg")
            {
                CanBeTamed = true,
                FoodItems = ("Raspberry"),
                CanSpawn = false,
            };

            Creature Mount_Boar_Ygg1 = new("pets", "Mount_Boar_Ygg1")
            {
                CanBeTamed = true,
                FoodItems = ("Raspberry"),
                CanSpawn = false,
            };

            Creature Mount_Boar_Ygg2 = new("pets", "Mount_Boar_Ygg2")
            {
                CanBeTamed = true,
                FoodItems = ("Raspberry"),
                CanSpawn = false,
            };


            //Duck//

            Creature Mount_Duck_Ygg = new("pets", "Mount_Duck_Ygg")
            {
                CanBeTamed = true,
                FoodItems = ("Raspberry"),
                CanSpawn = false,
            };

            Creature Mount_Duck_Ygg2 = new("pets", "Mount_Duck_Ygg2")
            {
                CanBeTamed = true,
                FoodItems = ("Raspberry"),
                CanSpawn = false,
            };

            //Special Mounts//

            Creature Mount_Lox_YggSpecial = new("pets", "Mount_Lox_YggSpecial")
            {
                CanBeTamed = true,
                FoodItems = ("Raspberry"),
                CanSpawn = false,
            };
        }
        private static void RegisterEffects()
        {
            //FX, VFX and SFX//

            GameObject vfx_petblood_Ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "vfx_petblood_Ygg");
            GameObject fx_startsummon_petmount_Ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "fx_startsummon_petmount_Ygg");
            GameObject fx_summonend_petmount_ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "fx_summonend_petmount_ygg");
            GameObject Orb_ProjectilePetMount_Ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "Orb_ProjectilePetMount_Ygg");

            GameObject Projectile_Pet_Ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "Projectile_Pet_Ygg");

            GameObject sfx_Duck_alerted_Ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "sfx_Duck_alerted_Ygg");
            GameObject sfx_duck_love_Ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "sfx_duck_love_Ygg");
            GameObject vfx_love_petmount_ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "vfx_love_petmount_ygg");
        }
        private static void LoadItems()
        {
            LoadSaddles();
            LoadCreatureAttackItems();
            
            Item Pet_Orb_Boar1_Ygg = new("pets", "Pet_Orb_Boar1_Ygg");
            Pet_Orb_Boar1_Ygg.Name.English("Boar Orb");
            Pet_Orb_Boar1_Ygg.Description.English("Summon Boar");
        }
        private static void LoadCreatureAttackItems()
        {
            //Item Atk, Projectile or Sleep//

            Item Boar_Chill_Ygg = new("pets", "Boar_Chill_Ygg");
            Boar_Chill_Ygg.Name.English("Chill Boar");
            Boar_Chill_Ygg.Description.English("Chilly Boar.");

            Item Boar_Sleep_Ygg = new("pets", "Boar_Sleep_Ygg");
            Boar_Sleep_Ygg.Name.English("Sleep Boar");
            Boar_Sleep_Ygg.Description.English("Sleepy Boar.");

            Item Boar_Tusk = new("pets", "Boar_Tusk");
            Boar_Tusk.Name.English("Tusk Boar");
            Boar_Tusk.Description.English("Tusk Boar.");

            Item Duck_Sleep_Ygg = new("pets", "Duck_Sleep_Ygg");
            Duck_Sleep_Ygg.Name.English("Duck Chills");
            Duck_Sleep_Ygg.Description.English("Duck Chills.");
        }
        private static void LoadSaddles()
        {
            Item Saddle_Pet_Ygg1 = new("pets", "Saddle_Pet_Ygg1");
            Saddle_Pet_Ygg1.Name.English("Hide Pet Saddle");
            Saddle_Pet_Ygg1.Description.English("Saddle for a Boar pet.");
            Saddle_Pet_Ygg1.Crafting.Add(ItemManager.CraftingTable.Workbench, 1);
            Saddle_Pet_Ygg1.RequiredItems.Add("Stone", 2);
            Saddle_Pet_Ygg1.RequiredItems.Add("Resin", 2);
            Saddle_Pet_Ygg1.RequiredItems.Add("Honey", 1);
            Saddle_Pet_Ygg1.CraftAmount = 1;

            Item Saddle_Pet_Ygg2 = new("pets", "Saddle_Pet_Ygg2");
            Saddle_Pet_Ygg2.Name.English("Silver Pet Saddle");
            Saddle_Pet_Ygg2.Description.English("Saddle for a Boar pet.");
            Saddle_Pet_Ygg2.Crafting.Add(ItemManager.CraftingTable.Workbench, 1);
            Saddle_Pet_Ygg2.RequiredItems.Add("Stone", 2);
            Saddle_Pet_Ygg2.RequiredItems.Add("Resin", 2);
            Saddle_Pet_Ygg2.RequiredItems.Add("Honey", 1);
            Saddle_Pet_Ygg2.CraftAmount = 1;

            Item Saddle_Pet_Lox = new("pets", "Saddle_Pet_Lox");
            Saddle_Pet_Lox.Name.English("Lox Pet Saddle");
            Saddle_Pet_Lox.Description.English("Saddle for a Lox pet.");
            Saddle_Pet_Lox.Crafting.Add(ItemManager.CraftingTable.Workbench, 1);
            Saddle_Pet_Lox.RequiredItems.Add("Stone", 2);
            Saddle_Pet_Lox.RequiredItems.Add("Resin", 2);
            Saddle_Pet_Lox.RequiredItems.Add("Honey", 1);
            Saddle_Pet_Lox.CraftAmount = 1;

            Item Saddle_Pet_Lox2 = new("pets", "Saddle_Pet_Lox2");
            Saddle_Pet_Lox2.Name.English("Silver Lox Pet Saddle");
            Saddle_Pet_Lox2.Description.English("Saddle for a Lox pet.");
            Saddle_Pet_Lox2.Crafting.Add(ItemManager.CraftingTable.Workbench, 1);
            Saddle_Pet_Lox2.RequiredItems.Add("Stone", 2);
            Saddle_Pet_Lox2.RequiredItems.Add("Resin", 2);
            Saddle_Pet_Lox2.RequiredItems.Add("Honey", 1);
            Saddle_Pet_Lox2.CraftAmount = 1;
        }
        private void OnDestroy() => Config.Save();
        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }
        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                petsLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                petsLogger.LogError($"There was an issue loading your {ConfigFileName}");
                petsLogger.LogError("Please check your config entries for spelling and format!");
            }
        }
        public ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }
        private ConfigEntry<T> config<T>(string group, string name, T value, string description, bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }
        public class ConfigurationManagerAttributes
        {
            [UsedImplicitly] public int? Order;
            [UsedImplicitly] public bool? Browsable;
            [UsedImplicitly] public string? Category;
            [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer;
        }
    }
    public static class KeyboardExtensions
    {
        public static bool IsKeyDown(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKeyDown(shortcut.MainKey) &&
                   shortcut.Modifiers.All(Input.GetKey);
        }

        public static bool IsKeyHeld(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKey(shortcut.MainKey) &&
                   shortcut.Modifiers.All(Input.GetKey);
        }
    }
}