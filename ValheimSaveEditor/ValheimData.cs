using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ValheimSaveEditor
{
    public class ValheimData
    {
        public static String[] BeardsUI = { "No beard", "Braided 1", "Braided 2", "Braided 3", "Braided 4", "Long 1", "Long 2", "Short 1", "Short 2", "Short 3", "Thick 1" };
        public static String[] BeardsInternal = { "BeardNone", "Beard5", "Beard6", "Beard9", "Beard10", "Beard1", "Beard2", "Beard3", "Beard4", "Beard7", "Beard8" };
        public static String[] HairsUI = { "No hair", "Braided 1", "Braided 2", "Braided 3", "Braided 4", "Long 1", "Ponytail 1", "Ponytail 2", "Ponytail 3", "Ponytail 4", "Short 1", "Short 2", "Side Swept 1", "Side Swept 2", "Side Swept 3" };
        public static String[] HairsInternal = { "HairNone", "Hair3", "Hair11", "Hair12", "Hair13", "Hair6", "Hair1", "Hair2", "Hair4", "Hair7", "Hair5", "Hair8", "Hair9", "Hair10", "Hair14" };
        public static String[] GendersUI = { "Male", "Female" };
        public static int[] GendersInternal = { 0, 1 };

        public static Regex NameRegex = new Regex(@"/([a-z]{3,15})/i");

        private static readonly int MaxInvWidth = 8;
        private static readonly int MaxInvHeight = 4;

        // https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-5-0
        public class Item
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Stack { get; set; }
            public float Weight { get; set; }
        }

        public class Vector3
        {
            public float X;
            public float Y;
            public float Z;
        }

        public static bool IsInventoryPositionValid(int x, int y)
        {
            if (x < 0 || y < 0 || x >= MaxInvWidth || y >= MaxInvHeight)
            {
                return false;
            }
            return true;
        }

        public static String ConvertSkillEnum(Character.SkillName skill)
        {
            return (int)skill switch
            {
                1 => "Swords",
                2 => "Knives",
                3 => "Clubs",
                4 => "Polearms",
                5 => "Spears",
                6 => "Blocking",
                7 => "Axes",
                8 => "Bows",
                11 => "Unarmed",
                12 => "Pickaxes",
                13 => "Woodcutting",
                100 => "Jumping",
                101 => "Sneaking",
                102 => "Running",
                103 => "Swimming",
                _ => "Unknown skill",
            };
        }

        public static Character.SkillName ConvertSkillEnum(string skill)
        {
            return skill switch
            {
                "Swords" => (Character.SkillName)1,
                "Knives" => (Character.SkillName)2,
                "Clubs" => (Character.SkillName)3,
                "Polearms" => (Character.SkillName)4,
                "Spears" => (Character.SkillName)5,
                "Blocking" => (Character.SkillName)6,
                "Axes" => (Character.SkillName)7,
                "Bows" => (Character.SkillName)8,
                "Unarmed" => (Character.SkillName)11,
                "Pickaxes" => (Character.SkillName)12,
                "Woodcutting" => (Character.SkillName)13,
                "Jumping" => (Character.SkillName)100,
                "Sneaking" => (Character.SkillName)101,
                "Running" => (Character.SkillName)102,
                "Swimming" => (Character.SkillName)103,
                _ => 0,
            };
        }

        public class Character
        {
            public byte[] Hash;
            public bool isOldCharacter;
            public string Beard = "";
            public HashSet<Biome> Biomes;
            public int Builds;
            public int Crafts;
            public int Deaths;
            public List<Food> Foods;    // digesting foods
            public string GuardianPower;
            public float GuardianPowerCooldown;
            public string Hair = "";
            public Vector3 HairColor;
            public float Hp;
            public long Id;
            public List<Item> Inventory;
            public bool IsFirstSpawn;
            public int Kills;
            public HashSet<string> KnownMaterials;
            public float MaxHp;
            public int Gender;  // 0 - male, 1 - female
            public string Name = "";
            public HashSet<string> Recipes;
            public HashSet<string> ShownTutorials;
            public Dictionary<SkillName, Skill> Skills;
            public Vector3 SkinColor;
            public float Stamina;
            public string StartSeed = "";
            public Dictionary<string, int> Stations = new Dictionary<string, int>();
            public Dictionary<string, string> Texts = new Dictionary<string, string>();
            public float TimeSinceDeath;
            public HashSet<string> Trophies;
            public HashSet<string> Uniques;
            public Dictionary<long, World> WorldsData = new Dictionary<long, World>();
            public int DataVersion = 0;
            public int SkillsVersion;
            public int InventoryVersion;
            public int CharacterVersion;
            public enum Biome
            {
                None,
                Meadows,
                Swamp,
                Mountain = 4,
                BlackForest = 8,
                Plains = 16,
                AshLands = 32,
                DeepNorth = 64,
                Ocean = 256,
                Mistlands = 512,
                BiomesMax
            }
            public enum SkillName
            {
                None,
                Swords,
                Knives,
                Clubs,
                Polearms,
                Spears,
                Blocking,
                Axes,
                Bows,
                FireMagic,
                FrostMagic,
                Unarmed,
                Pickaxes,
                WoodCutting,
                Jump = 100,
                Sneak,
                Run,
                Swim,
                All = 999
            }
            public class World
            {
                public Vector3 DeathPoint = new Vector3();
                public bool HasCustomSpawnPoint;
                public bool HasDeathPoint;
                public bool HasLogoutPoint;
                public Vector3 HomePoint = new Vector3();
                public Vector3 LogoutPoint = new Vector3();
                public byte[] MapData;
                public Vector3 SpawnPoint = new Vector3();
            }
            public class Item
            {
                public long CrafterId;
                public string CrafterName;
                public float Durability;
                public bool Equipped;
                public string Name;
                public Tuple<int, int> Pos;
                public int Quality;
                public int Stack;
                public int Variant;
            }
            public class Food
            {
                public float HpLeft;
                public string Name;
                public float StaminaLeft;
            }

            public class Skill
            {
                public float Level;
                public SkillName SkillName;
                public float Accumulator;
            }
        }
    }
}
