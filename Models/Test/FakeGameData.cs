namespace MMOngo.Models
{
    public static class FakeGameData
    {
        public static List<Player> Players = new List<Player>
        {
            new Player
            {
                PlayerName = "Eli Walker",
                UserName = "EliTheMage",
                Age = 21,
                PasswordHash = "hashed_password_123",
                Characters = new List<string> { "Man Man", "Dr. Destruction" },
                MemberSince = "05-06-2014"
            }
        };

        public static List<PlayerCharacter> Characters = new List<PlayerCharacter>
        {
            new PlayerCharacter
            {
                CharacterId = 1,
                CharacterName = "Man Man",
                UserName = "Eli Walker",
                CurrentLevel = 36,
                Allies = new List<string> { "Mr. Accordion", "The Amazing Shoe" },
                Equipment = new EquipmentSet
                {
                    Weapons = new List<string> { "Sword of Searing", "Enchanted Dagger" },
                    Armor = new List<string> { "Parameter Helm" },
                    Tools = new List<string> { "Pickaxe" }
                },
                CurrentMissions = new List<string> { "Quest for Nestor", "A Blue Bird" },
                CompletedMissions = new List<string> { "Mushroom Collecting", "Defeat 20 Slimes" },
                XP = 291512,
                KnownSpells = new List<string> { "Whirl, O' Flame!", "Death Lightning" },
                GuildMemberships = new List<string> { "Oath of Blue" }
            },
            new PlayerCharacter
            {
                CharacterId = 2,
                CharacterName = "Dr. Destruction",
                UserName = "Eli Walker",
                CurrentLevel = 24,
                Allies = new List<string>(),
                Equipment = new EquipmentSet(),
                CurrentMissions = new List<string>(),
                CompletedMissions = new List<string> { "Goblin Sweep" },
                XP = 120000,
                KnownSpells = new List<string> { "Death Lightning" },
                GuildMemberships = new List<string>()
            }
        };

        public static List<Weapon> Weapons = new List<Weapon>
        {
            new Weapon
            {
                WeaponId = 1,
                WeaponName = "Sword of Searing",
                Weight = 5,
                Price = 250,
                Damage = 45,
                SpecialAttributes = new List<string> { "Fire", "Burn Chance" }
            },
            new Weapon
            {
                WeaponId = 2,
                WeaponName = "Enchanted Dagger",
                Weight = 2,
                Price = 140,
                Damage = 22,
                SpecialAttributes = new List<string> { "Magic" }
            }
        };

        public static List<Armor> Armors = new List<Armor>
        {
            new Armor
            {
                ArmorId = 1,
                ArmorName = "Parameter Helm",
                Weight = 4,
                Price = 260,
                Defense = 40,
                SpecialAttributes = new List<string> { "Fire Resist", "Magic" }
            }
        };

        public static List<ToolItem> Tools = new List<ToolItem>
        {
            new ToolItem
            {
                ToolId = 1,
                ToolName = "Pickaxe",
                Weight = 3,
                Price = 25,
                Usage = new List<string> { "Material Collection" }
            }
        };

        public static List<Guild> Guilds = new List<Guild>
        {
            new Guild
            {
                GuildName = "Oath of Blue",
                Creator = "The Amazing Shoe",
                MemberCount = 2,
                Members = new List<string> { "Dr. Destruction", "Man Man" },
                CreationDate = "04-01-2018",
                Benefits = new List<string> { "Protection from invaders", "Magic attack +2" }
            }
        };

        public static List<Mission> Missions = new List<Mission>
        {
            new Mission
            {
                MissionName = "Quest for Nestor",
                MissionType = "Fetch Quest",
                QuestGiver = "Nestor",
                Reward = 200
            },
            new Mission
            {
                MissionName = "A Blue Bird",
                MissionType = "Exploration",
                QuestGiver = "Nestor",
                Reward = 150
            },
            new Mission
            {
                MissionName = "Mushroom Collecting",
                MissionType = "Gathering",
                QuestGiver = "Nestor",
                Reward = 75
            },
            new Mission
            {
                MissionName = "Defeat 20 Slimes",
                MissionType = "Combat",
                QuestGiver = "Nestor",
                Reward = 120
            },
            new Mission
            {
                MissionName = "Goblin Sweep",
                MissionType = "Combat",
                QuestGiver = "Nestor",
                Reward = 180
            }
        };

        public static List<Npc> Npcs = new List<Npc>
        {
            new Npc
            {
                NpcName = "Nestor",
                Locations = new List<string> { "Nestor's Nest", "The Forge" },
                WaresSold = new List<string> { "Pickaxe", "Sword of Searing" },
                Quests = new List<string> { "Quest for Nestor" },
                Equipment = new EquipmentSet
                {
                    Weapons = new List<string> { "Sword of Searing" },
                    Armor = new List<string> { "Parameter Helm" },
                    Tools = new List<string> { "Pickaxe" }
                },
                XP = 291512,
                KnownSpells = new List<string> { "Whirl, O' Flame!", "Death Lightning" }
            }
        };

        public static List<Spell> Spells = new List<Spell>
        {
            new Spell
            {
                SpellId = 12,
                SpellName = "Death Lightning",
                SpellDamage = 400,
                SpellHealing = 0,
                MpCost = 120,
                GoldCost = 280,
                Range = 300
            },
            new Spell
            {
                SpellId = 13,
                SpellName = "Whirl, O' Flame!",
                SpellDamage = 250,
                SpellHealing = 0,
                MpCost = 80,
                GoldCost = 200,
                Range = 180
            }
        };

        public static List<Transaction> Transactions = new List<Transaction>
        {
            new Transaction
            {
                TransactionId = 17,
                UserName = "EliTheMage",
                ItemType = "Weapon",
                ItemId = 1,
                Total = 250,
                TransactionDate = "02-14-2026"
            }
        };
    }
}