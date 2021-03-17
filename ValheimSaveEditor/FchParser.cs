using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;

namespace ValheimSaveEditor
{
    class FchParser
    {
        public static ByteReader ReadSaveFile(string path)
        {
            using(var fileStream = File.OpenRead(path))
            {
                var reader = new BinaryReader(fileStream);
                var dataLength = reader.ReadInt32();
                var data = reader.ReadBytes(dataLength);
                return new ByteReader(data);
            }
        }

        public static ValheimData.Character ReadCharData(string path)
        {
            var character = new ValheimData.Character();
            var byteReader = ReadSaveFile(path);
            if(byteReader.Length() == 0)
            {
                MessageBox.Show("Could not read character save file", "Error", MessageBoxButton.OK);
            }

            //header
            character.CharacterVersion = byteReader.ReadInt32();
            character.Kills = byteReader.ReadInt32();
            character.Deaths = byteReader.ReadInt32();
            character.Crafts = byteReader.ReadInt32();
            character.Builds = byteReader.ReadInt32();

            //world info
            var numOfWorlds = byteReader.ReadInt32();
            for(var i = 0; i < numOfWorlds; i++)
            {
                var worldId = byteReader.ReadInt64();
                var world = new ValheimData.Character.World
                {
                    HasCustomSpawnPoint = byteReader.ReadBoolean(),
                    SpawnPoint = byteReader.ReadVector3(),
                    HasLogoutPoint = byteReader.ReadBoolean(),
                    LogoutPoint = byteReader.ReadVector3(),
                    HasDeathPoint = byteReader.ReadBoolean(),
                    DeathPoint = byteReader.ReadVector3(),
                    HomePoint = byteReader.ReadVector3()
                };
                if(byteReader.ReadBoolean())
                {
                    world.MapData = byteReader.ReadBytes();
                }
            }

            //character info
            character.Name = byteReader.ReadString();
            character.Id = byteReader.ReadInt64();
            character.StartSeed = byteReader.ReadString();
            //The next byte is a bool which is true if there is more data
            //This will be false for new characters
            if (!byteReader.ReadBoolean()) return character;

            var dataLength = byteReader.ReadInt32();
            character.DataVersion = byteReader.ReadInt32();
            character.MaxHp = byteReader.ReadSingle();
            character.Hp = byteReader.ReadSingle();
            character.Stamina = byteReader.ReadSingle();
            character.IsFirstSpawn = byteReader.ReadBoolean();
            character.TimeSinceDeath = byteReader.ReadSingle();
            character.GuardianPower = byteReader.ReadString();
            character.GuardianPowerCooldown = byteReader.ReadSingle();

            //inventory info
            character.Inventory = new List<ValheimData.Character.Item>();
            character.InventoryVersion = byteReader.ReadInt32();
            var numberOfItems = byteReader.ReadInt32();
            for(var i = 0; i < numberOfItems; i++)
            {
                var item = new ValheimData.Character.Item
                {
                    Name = byteReader.ReadString(),
                    Stack = byteReader.ReadInt32(),
                    Durability = byteReader.ReadSingle(),
                    Pos = new Tuple<int, int>(byteReader.ReadInt32(), byteReader.ReadInt32()),
                    Equipped = byteReader.ReadBoolean(),
                    Quality = byteReader.ReadInt32(),
                    Variant = byteReader.ReadInt32(),
                    CrafterId = byteReader.ReadInt64(),
                    CrafterName = byteReader.ReadString()
                };
                if (item.Name != "") character.Inventory.Add(item);
            }

            //"known" character info like recipes and tutorials
            character.Recipes = new HashSet<string>();
            character.KnownMaterials = new HashSet<string>();
            character.ShownTutorials = new HashSet<string>();
            character.Uniques = new HashSet<string>();
            character.Trophies = new HashSet<string>();
            character.Biomes = new HashSet<ValheimData.Character.Biome>();

            var numOfRecipes = byteReader.ReadInt32();
            for(var i = 0; i < numOfRecipes; i++)
            {
                character.Recipes.Add(byteReader.ReadString());
            }
        }
    }
}
