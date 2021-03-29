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
        private static ByteAccess LoadSaveData(string path)
        {
            using(var fileStream = File.OpenRead(path))
            {
                var reader = new BinaryReader(fileStream);
                var dataLength = reader.ReadInt32();
                var data = reader.ReadBytes(dataLength);
                return new ByteAccess(data);
            }
        }

        public static ValheimData.Character ReadCharacterData(string path)
        {
            var character = new ValheimData.Character();
            var byteReader = LoadSaveData(path);
            if(byteReader.Length() == 0)
            {
                MessageBox.Show("Could not read character save file", "Error", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }

            //header
            character.CharacterVersion = byteReader.ReadInt32();
            if(character.CharacterVersion < 30)
            {
                MessageBox.Show("Character version is too old.", "ERROR", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }
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
                character.WorldsData.Add(worldId, world);
            }

            //character info
            character.Name = byteReader.ReadString();
            character.Id = byteReader.ReadInt64();
            character.StartSeed = byteReader.ReadString();
            //The next byte is a bool which is true if there is more data
            //This will be false for new characters
            character.isOldCharacter = byteReader.ReadBoolean();
            if (!character.isOldCharacter) return character;

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

            var numberOfRecipes = byteReader.ReadInt32();
            for (var i = 0; i < numberOfRecipes; i++)
                character.Recipes.Add(byteReader.ReadString());

            var numberOfStations = byteReader.ReadInt32();
            for (var i = 0; i < numberOfStations; i++)
                character.Stations.Add(byteReader.ReadString(), byteReader.ReadInt32());

            var numberOfKnownMaterials = byteReader.ReadInt32();
            for (var i = 0; i < numberOfKnownMaterials; i++)
                character.KnownMaterials.Add(byteReader.ReadString());

            var numberOfShownTutorials = byteReader.ReadInt32();
            for (var i = 0; i < numberOfShownTutorials; i++)
                character.ShownTutorials.Add(byteReader.ReadString());

            var numberOfUniques = byteReader.ReadInt32();
            for (var i = 0; i < numberOfUniques; i++)
                character.Uniques.Add(byteReader.ReadString());

            var numberOfTrophies = byteReader.ReadInt32();
            for (var i = 0; i < numberOfTrophies; i++)
                character.Trophies.Add(byteReader.ReadString());

            var numberOfBiomes = byteReader.ReadInt32();
            for (var i = 0; i < numberOfBiomes; i++)
                character.Biomes.Add((ValheimData.Character.Biome)byteReader.ReadInt32());

            var numberOfTexts = byteReader.ReadInt32();
            for (var i = 0; i < numberOfTexts; i++)
                character.Texts.Add(byteReader.ReadString(), byteReader.ReadString());

            //character appearance
            character.Beard = byteReader.ReadString();
            character.Hair = byteReader.ReadString();
            character.SkinColor = byteReader.ReadVector3();
            character.HairColor = byteReader.ReadVector3();
            character.Gender = byteReader.ReadInt32();

            //food consumed
            var numOfFoodConsumed = byteReader.ReadInt32();
            character.Foods = new List<ValheimData.Character.Food>();
            for(var i = 0; i < numOfFoodConsumed; i++)
            {
                var food = new ValheimData.Character.Food
                {
                    Name = byteReader.ReadString(),
                    HpLeft = byteReader.ReadSingle(),
                    StaminaLeft = byteReader.ReadSingle()
                };
                character.Foods.Add(food);
            }

            //skills
            character.SkillsVersion = byteReader.ReadInt32();
            var numOfSkills = byteReader.ReadInt32();
            character.Skills = new Dictionary<ValheimData.Character.SkillName, ValheimData.Character.Skill>();
            for(var i = 0; i < numOfSkills; i++)
            {
                var skill = new ValheimData.Character.Skill
                {
                    SkillName = (ValheimData.Character.SkillName)byteReader.ReadInt32(),
                    Level = byteReader.ReadSingle(),
                    Accumulator = byteReader.ReadSingle()
                };
                character.Skills.Add(skill.SkillName, skill);
            }

            //character hash should be the final info, written after data array
            //I think this is a useless value but I'm saving it anyway
            //It likely exists so a valheim save file is guarenteed to have a different file hash every save, poking steam cloud sync
            //Valheim changes the end file hash value on every save so even if no changes were made to the character,
            // the file will have a different hash due to this changed data

            //Reading this right now gives a "cannot read past end of file" error but the read seems good up until this point. What's causing it?

            //character.Hash = byteReader.ReadBytes();

            return character;
        }

        public static byte[] CharacterToArray(ValheimData.Character character)
        {
            var byteWriter = new ByteAccess();

            byteWriter.Write(character.CharacterVersion);
            byteWriter.Write(character.Kills);
            byteWriter.Write(character.Deaths);
            byteWriter.Write(character.Crafts);
            byteWriter.Write(character.Builds);
            byteWriter.Write(character.WorldsData.Count);
            foreach (var world in character.WorldsData)
            {
                byteWriter.Write(world.Key);
                byteWriter.Write(world.Value.HasCustomSpawnPoint);
                byteWriter.Write(world.Value.SpawnPoint);
                byteWriter.Write(world.Value.HasLogoutPoint);
                byteWriter.Write(world.Value.LogoutPoint);
                byteWriter.Write(world.Value.HasDeathPoint);
                byteWriter.Write(world.Value.DeathPoint);
                byteWriter.Write(world.Value.HomePoint);
                byteWriter.Write(world.Value.MapData != null);
                if (world.Value.MapData != null)
                    byteWriter.Write(world.Value.MapData);
            }
            byteWriter.Write(character.Name);
            byteWriter.Write(character.Id);
            byteWriter.Write(character.StartSeed);
            byteWriter.Write(character.isOldCharacter);
            var byteWriter2 = new ByteAccess();
            if(character.isOldCharacter)
            {
                byteWriter2.Write(character.DataVersion);
                byteWriter2.Write(character.MaxHp);
                byteWriter2.Write(character.Hp);
                byteWriter2.Write(character.Stamina);
                byteWriter2.Write(character.IsFirstSpawn);
                byteWriter2.Write(character.TimeSinceDeath);
                byteWriter2.Write(character.GuardianPower);
                byteWriter2.Write(character.GuardianPowerCooldown);
                byteWriter2.Write(character.InventoryVersion);
                byteWriter2.Write(character.Inventory.Count);

                foreach (var item in character.Inventory)
                {
                    byteWriter2.Write(item.Name);
                    byteWriter2.Write(item.Stack);
                    byteWriter2.Write(item.Durability);
                    byteWriter2.Write(item.Pos.Item1);
                    byteWriter2.Write(item.Pos.Item2);
                    byteWriter2.Write(item.Equipped);
                    byteWriter2.Write(item.Quality);
                    byteWriter2.Write(item.Variant);
                    byteWriter2.Write(item.CrafterId);
                    byteWriter2.Write(item.CrafterName);
                }

                byteWriter2.Write(character.Recipes.Count);
                foreach (var recipe in character.Recipes)
                    byteWriter2.Write(recipe);

                byteWriter2.Write(character.Stations.Count);
                foreach (var station in character.Stations)
                {
                    byteWriter2.Write(station.Key);
                    byteWriter2.Write(station.Value);
                }

                byteWriter2.Write(character.KnownMaterials.Count);
                foreach (var material in character.KnownMaterials)
                    byteWriter2.Write(material);

                byteWriter2.Write(character.ShownTutorials.Count);
                foreach (var tutorial in character.ShownTutorials)
                    byteWriter2.Write(tutorial);

                byteWriter2.Write(character.Uniques.Count);
                foreach (var unique in character.Uniques)
                    byteWriter2.Write(unique);

                byteWriter2.Write(character.Trophies.Count);
                foreach (var trophy in character.Trophies)
                    byteWriter2.Write(trophy);

                byteWriter2.Write(character.Biomes.Count);
                foreach (var biome in character.Biomes)
                    byteWriter2.Write((int)biome);

                byteWriter2.Write(character.Texts.Count);
                foreach (var text in character.Texts)
                {
                    byteWriter2.Write(text.Key);
                    byteWriter2.Write(text.Value);
                }

                byteWriter2.Write(character.Beard);
                byteWriter2.Write(character.Hair);
                byteWriter2.Write(character.SkinColor);
                byteWriter2.Write(character.HairColor);
                byteWriter2.Write(character.Gender);
                byteWriter2.Write(character.Foods.Count);
                foreach (var food in character.Foods)
                {
                    byteWriter2.Write(food.Name);
                    byteWriter2.Write(food.HpLeft);
                    byteWriter2.Write(food.StaminaLeft);
                }

                byteWriter2.Write(character.SkillsVersion);
                byteWriter2.Write(character.Skills.Count);
                foreach (var skill in character.Skills.Values)
                {
                    byteWriter2.Write((int)skill.SkillName);
                    byteWriter2.Write(skill.Level);
                    byteWriter2.Write(skill.Accumulator);
                }
            }
            byte[] playerData = byteWriter2.ToArray();

            byteWriter.Write(playerData.Length);
            byte[] data = byteWriter.ToArray();
            byte[] final = data.Concat(playerData).ToArray();
            byte[] length = BitConverter.GetBytes(final.Length);
            byte[] hashLength = BitConverter.GetBytes(64);
            byte[] hash = SHA512.Create().ComputeHash(final);
            byteWriter.Clear();
            byteWriter2.Clear();

            byte[] characterAsArray = length.Concat(final).ToArray().Concat(hashLength).ToArray().Concat(hash).ToArray();

            return characterAsArray;
        }
    }
}
