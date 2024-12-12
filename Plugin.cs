using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CreatureManager;
using HarmonyLib;
using ItemManager;
using LocationManager;
using PieceManager;
using ServerSync;
using SkillManager;
using StatusEffectManager;
using UnityEngine;
using PrefabManager = ItemManager.PrefabManager;
using Range = LocationManager.Range;

namespace pets
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class petsPlugin : BaseUnityPlugin
    {
        internal const string ModName = "MountPets";
        internal const string ModVersion = "1.0.0";
        internal const string Author = "Yggdrah";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);

        public static readonly ManualLogSource petsLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync ConfigSync = new(ModGUID)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };
        
        // Location Manager variables
        public Texture2D tex;
        private Sprite mySprite;
        private SpriteRenderer sr;

        public void Awake()
        {
            _serverConfigLocked = config("General", "Force Server Config", true, "Force Server Config");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

            

            #region 



            #region Location Notes

            // MapIcon                      Sets the map icon for the location.
            // ShowMapIcon                  When to show the map icon of the location. Requires an icon to be set. Use "Never" to not show a map icon for the location. Use "Always" to always show a map icon for the location. Use "Explored" to start showing a map icon for the location as soon as a player has explored the area.
            // MapIconSprite                Sets the map icon for the location.
            // CanSpawn                     Can the location spawn at all.
            // SpawnArea                    If the location should spawn more towards the edge of the biome or towards the center. Use "Edge" to make it spawn towards the edge. Use "Median" to make it spawn towards the center. Use "Everything" if it doesn't matter.</para>
            // Prioritize                   If set to true, this location will be prioritized over other locations, if they would spawn in the same area.
            // PreferCenter                 If set to true, Valheim will try to spawn your location as close to the center of the map as possible.
            // Rotation                     How to rotate the location. Use "Fixed" to use the rotation of the prefab. Use "Random" to randomize the rotation. Use "Slope" to rotate the location along a possible slope.
            // HeightDelta                  The minimum and maximum height difference of the terrain below the location.
            // SnapToWater                  If the location should spawn near water.
            // ForestThreshold              If the location should spawn in a forest. Everything above 1.15 is considered a forest by Valheim. 2.19 is considered a thick forest by Valheim.
            // Biome
            // SpawnDistance                Minimum and maximum range from the center of the map for the location.
            // SpawnAltitude                Minimum and maximum altitude for the location.
            // MinimumDistanceFromGroup     Locations in the same group will keep at least this much distance between each other.
            // GroupName                    The name of the group of the location, used by the minimum distance from group setting.
            // Count                        Maximum number of locations to spawn in. Does not mean that this many locations will spawn. But Valheim will try its best to spawn this many, if there is space.
            // Unique                       If set to true, all other locations will be deleted, once the first one has been discovered by a player.

            #endregion

            #region Pets

            

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
            







            //Items//

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
            




            Item Pet_Orb_Boar1_Ygg = new("pets", "Pet_Orb_Boar1_Ygg");
            Pet_Orb_Boar1_Ygg.Name.English("Boar Orb");
            Pet_Orb_Boar1_Ygg.Description.English("Summon Boar");




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

            





            //FX, VFX and SFX//

            GameObject vfx_petblood_Ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "vfx_petblood_Ygg");
            GameObject fx_startsummon_petmount_Ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "fx_startsummon_petmount_Ygg");
            GameObject fx_summonend_petmount_ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "fx_summonend_petmount_ygg");
            GameObject Orb_ProjectilePetMount_Ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "Orb_ProjectilePetMount_Ygg");

            GameObject Projectile_Pet_Ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "Projectile_Pet_Ygg");

            GameObject sfx_Duck_alerted_Ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "sfx_Duck_alerted_Ygg");
            GameObject sfx_duck_love_Ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "sfx_duck_love_Ygg");
            GameObject vfx_love_petmount_ygg = ItemManager.PrefabManager.RegisterPrefab("pets", "vfx_love_petmount_ygg");

            #endregion





            #endregion


            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private void OnDestroy()
        {
            Config.Save();
        }

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


        #region ConfigOptions

        private static ConfigEntry<bool>? _serverConfigLocked;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
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

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            public bool? Browsable = false;
        }
        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", KeyboardShortcut.AllKeyCodes);
        }

        #endregion
    }
}