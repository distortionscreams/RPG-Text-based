using System;
using System.Collections.Generic;
using System.Linq;

namespace RPG_Text_base
{
    // ══════════════════════════════════════════════════════════════════════════════
    // UI
    // ══════════════════════════════════════════════════════════════════════════════
    public static class UI
    {
        public static void Print(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void PrintHeader(string title, ConsoleColor color = ConsoleColor.Yellow)
        {
            Console.Clear();
            Print("╔" + new string('═', title.Length + 4) + "╗", color);
            Print($"║ {title} ║", color);
            Print("╚" + new string('═', title.Length + 4) + "╝", color);
            Console.WriteLine();
        }

        public static void Pause(string message = "\n Press any key to continue...")
        {
            Print(message, ConsoleColor.DarkGray);
            Console.ReadKey(true);
        }

        public static string ReadChoice(string prompt, string[] validOptions)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim().ToUpper() ?? "";
                if (Array.Exists(validOptions, v => v == input))
                    return input;
                Print($" ⚠️ Please enter one of: {string.Join(", ", validOptions)}", ConsoleColor.DarkYellow);
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════════════════
    // STATS
    // ══════════════════════════════════════════════════════════════════════════════
    public static class Stats
    {
        public static int totalScore = 0;
        public static int monstersKilled = 0;
        public static int totalTurns = 0;
        public static int playerHP = 100;
        public static int playerMaxHP = 100;
        public static int playerStamina = 100;
        public static int playerMaxStamina = 100;
        public static int playerAtkBonus = 0;
        public static int extraPotions = 0;
        public static int staminaRegen = 10;

        public enum MonsterRarity { OneStar = 1, TwoStar = 2, ThreeStar = 3, FourStar = 4, FiveStar = 5 }

        public record DropInfo(string ItemId, string ItemName);
        public record MonsterStats(string Name, int MaxHP, int AtkMin, int AtkMax, int DefMin, int DefMax,
                                   int Score, DropInfo Drop);

        public static MonsterStats GetMonsterStats(MonsterRarity rarity)
        {
            MonsterStats[] pool = rarity switch
            {
                MonsterRarity.OneStar => OneStarMonsters,
                MonsterRarity.TwoStar => TwoStarMonsters,
                MonsterRarity.ThreeStar => ThreeStarMonsters,
                MonsterRarity.FourStar => FourStarMonsters,
                MonsterRarity.FiveStar => FiveStarMonsters,
                _ => OneStarMonsters
            };
            return pool[Program.rand.Next(pool.Length)];
        }

        public static readonly MonsterStats[] OneStarMonsters =
        {
            new("Zombie", 78, 10, 18, 3, 7, 78, new("rotten_flesh", "Rotten Flesh")),
            new("Skeleton", 72, 12, 20, 2, 6, 88, new("bone_shard", "Bone Shard")),
            new("Slime", 65, 8, 15, 6, 12, 68, new("slime_gel", "Slime Gel")),
            new("Wolf", 83, 13, 22, 3, 8, 93, new("wolf_fang", "Wolf Fang")),
            new("Ghost", 60, 11, 19, 2, 5, 83, new("ectoplasm", "Ectoplasm")),
            new("Sinner", 90, 14, 23, 4, 9, 86, new("cursed_coin", "Cursed Coin")),
            new("Corpse", 95, 15, 24, 5, 10, 95, new("grave_dust", "Grave Dust")),
        };

        public static readonly MonsterStats[] TwoStarMonsters =
        {
            new("Giant Bat", 102, 16, 25, 5, 9, 132, new("bat_wing", "Bat Wing")),
            new("Lizardman", 113, 17, 27, 7, 13, 152, new("lizard_scale", "Lizard Scale")),
            new("Nightmare", 107, 18, 29, 4, 8, 162, new("dark_essence", "Dark Essence")),
            new("Flesh", 119, 15, 24, 8, 14, 142, new("flesh_chunk", "Flesh Chunk")),
            new("Cursed Skull", 96, 19, 30, 3, 7, 157, new("cursed_eye", "Cursed Eye")),
            new("Crusader", 127, 21, 31, 6, 12, 167, new("iron_emblem", "Iron Emblem")),
            new("Imp", 132, 22, 33, 7, 14, 175, new("imp_horn", "Imp Horn")),
        };

        public static readonly MonsterStats[] ThreeStarMonsters =
        {
            new("Skeleton Warrior", 145, 23, 34, 9, 16, 230, new("war_blade_shard", "War Blade Shard")),
            new("Werewolf", 156, 25, 38, 7, 14, 250, new("silver_claw", "Silver Claw")),
            new("Wyvern", 162, 27, 40, 8, 15, 270, new("wyvern_talon", "Wyvern Talon")),
            new("Man-Eater Ghoul", 150, 24, 35, 6, 12, 240, new("ghoul_tooth", "Ghoul Tooth")),
            new("Phantom Duelist", 139, 26, 39, 5, 11, 260, new("phantom_blade", "Phantom Blade")),
            new("Holy Knight", 174, 29, 42, 10, 17, 265, new("holy_crest", "Holy Crest")),
            new("Cerberus", 180, 30, 44, 11, 18, 285, new("hellhound_collar", "Hellhound Collar")),
        };

        public static readonly MonsterStats[] FourStarMonsters =
        {
            new("Vampire", 160, 26, 39, 8, 15, 320, new("blood_vial", "Blood Vial")),
            new("Demon", 168, 28, 41, 9, 16, 345, new("demon_heart", "Demon Heart")),
            new("Headless Knight", 178, 25, 38, 12, 19, 335, new("hollow_helm", "Hollow Helm")),
            new("Medusa", 153, 29, 43, 7, 14, 325, new("stone_gaze_shard", "Stone Gaze Shard")),
            new("Fallen Angel", 164, 31, 46, 6, 13, 360, new("broken_halo", "Broken Halo")),
            new("Devil Hunter", 200, 34, 50, 10, 18, 410, new("hellfire_core", "Hellfire Core")),
            new("Nightstalker", 210, 36, 53, 11, 20, 430, new("shadow_fragment", "Shadow Fragment")),
        };

        public static readonly MonsterStats[] FiveStarMonsters =
        {
            new("Dragon", 235, 36, 54, 14, 22, 540, new("dragon_scale", "Dragon Scale")),
            new("Leviathan", 265, 34, 52, 12, 20, 580, new("abyssal_pearl", "Abyssal Pearl")),
            new("Hydra", 252, 31, 49, 16, 25, 555, new("hydra_venom", "Hydra Venom")),
            new("Death", 212, 41, 60, 10, 18, 620, new("death_scythe_tip", "Death Scythe Tip")),
            new("Seraph of Death", 225, 39, 56, 13, 21, 660, new("fallen_feather", "Fallen Feather")),
            new("Seraphim", 285, 46, 66, 16, 24, 720, new("divine_core", "Divine Core")),
            new("Lucifer", 300, 48, 70, 17, 26, 770, new("lucifer_sigil", "Lucifer's Sigil")),
        };
    }

    // ══════════════════════════════════════════════════════════════════════════════
    // INVENTORY
    // ══════════════════════════════════════════════════════════════════════════════
    public static class Inventory
    {
        public enum ItemType { Consumable, Weapon, Armor, Relic, Misc }

        public record Item(string Id, string Name, ItemType Type, string Description);

        public static readonly Dictionary<string, Item> AllItems = new();
        internal static readonly Dictionary<string, int> _bag = new();

        public static int MaxSlots { get; private set; } = 30;
        public static int TotalSlotsUsed => _bag.Count;

        public static bool AddItem(string itemId, int amount = 1)
        {
            if (!AllItems.ContainsKey(itemId))
            {
                UI.Print($" ❌ Unknown item: {itemId}", ConsoleColor.Red);
                return false;
            }
            if (TotalSlotsUsed >= MaxSlots && !_bag.ContainsKey(itemId))
            {
                UI.Print(" ⚠️ Inventory full!", ConsoleColor.DarkYellow);
                return false;
            }
            _bag.TryGetValue(itemId, out int current);
            _bag[itemId] = current + amount;
            return true;
        }

        public static bool RemoveItem(string itemId, int amount = 1)
        {
            if (!_bag.TryGetValue(itemId, out int count) || count < amount) return false;
            _bag[itemId] = count - amount;
            if (_bag[itemId] <= 0) _bag.Remove(itemId);
            return true;
        }

        public static int GetCount(string itemId) => _bag.TryGetValue(itemId, out int n) ? n : 0;

        public static void ShowInventory()
        {
            while (true)
            {
                UI.PrintHeader("INVENTORY");
                Console.WriteLine($" Slots used: {TotalSlotsUsed}/{MaxSlots}");
                Console.WriteLine(" ─────────────────────────────────────────");

                if (_bag.Count == 0)
                {
                    UI.Print(" (Your inventory is empty)", ConsoleColor.DarkGray);
                    if (UI.ReadChoice(" [B] Back: ", new[] { "B" }) == "B") return;
                    continue;
                }

                var itemList = _bag.ToList();

                for (int i = 0; i < itemList.Count; i++)
                {
                    var (id, qty) = itemList[i];
                    var itemInfo = AllItems[id];                    // Changed name
                    Console.WriteLine($" [{i + 1}] {itemInfo.Name} x{qty}");
                    UI.Print($" {itemInfo.Description}", ConsoleColor.DarkGray);
                }
                Console.WriteLine(" ─────────────────────────────────────────");

                string input = UI.ReadChoice(" Enter number or [B] Back: ",
                                             Enumerable.Range(1, itemList.Count).Select(x => x.ToString()).Append("B").ToArray());

                if (input == "B") return;

                int choice = int.Parse(input) - 1;
                var selected = itemList[choice];
                var selectedItem = AllItems[selected.Key];         // Changed name

                UI.PrintHeader("ITEM DETAILS");
                Console.WriteLine($" {selectedItem.Name} x{selected.Value}");
                UI.Print(selectedItem.Description);
                UI.Pause();
            }
        }

        static Inventory()
        {
            void Reg(string id, string name, string desc) =>
            AllItems[id] = new Item(id, name, ItemType.Misc, desc);

            Reg("rotten_flesh", "Rotten Flesh", "Flesh from a zombie. Smells terrible.");
            Reg("bone_shard", "Bone Shard", "Sharp bone fragment from a skeleton.");
            Reg("slime_gel", "Slime Gel", "Sticky gel from a slime.");
            Reg("wolf_fang", "Wolf Fang", "Sharp fang from a wild wolf.");
            Reg("ectoplasm", "Ectoplasm", "Ghostly residue.");
            Reg("cursed_coin", "Cursed Coin", "Coin that brings bad luck.");
            Reg("grave_dust", "Grave Dust", "Dust from an old grave.");
            Reg("bat_wing", "Bat Wing", "Leathery wing of a giant bat.");
            Reg("lizard_scale", "Lizard Scale", "Tough scale from a lizardman.");
            Reg("dark_essence", "Dark Essence", "Pure darkness in liquid form.");
            Reg("flesh_chunk", "Flesh Chunk", "A chunk of rotten flesh.");
            Reg("cursed_eye", "Cursed Eye", "An eye that still stares.");
            Reg("iron_emblem", "Iron Emblem", "Emblem of a fallen crusader.");
            Reg("imp_horn", "Imp Horn", "Small demonic horn.");
            Reg("war_blade_shard", "War Blade Shard", "Broken blade of a skeleton warrior.");
            Reg("silver_claw", "Silver Claw", "Claw from a werewolf.");
            Reg("wyvern_talon", "Wyvern Talon", "Sharp talon of a wyvern.");
            Reg("ghoul_tooth", "Ghoul Tooth", "Tooth from a man-eater ghoul.");
            Reg("phantom_blade", "Phantom Blade", "Ghostly sword fragment.");
            Reg("holy_crest", "Holy Crest", "Crest of a holy knight.");
            Reg("hellhound_collar", "Hellhound Collar", "Collar from Cerberus.");
            Reg("blood_vial", "Blood Vial", "Vial of vampire blood.");
            Reg("demon_heart", "Demon Heart", "Still beating demon heart.");
            Reg("hollow_helm", "Hollow Helm", "Empty helmet of a headless knight.");
            Reg("stone_gaze_shard", "Stone Gaze Shard", "Shard from Medusa's gaze.");
            Reg("broken_halo", "Broken Halo", "Fallen angel's broken halo.");
            Reg("hellfire_core", "Hellfire Core", "Core of pure hellfire.");
            Reg("shadow_fragment", "Shadow Fragment", "Piece of pure shadow.");
            Reg("dragon_scale", "Dragon Scale", "Legendary dragon scale.");
            Reg("abyssal_pearl", "Abyssal Pearl", "Pearl from the Leviathan.");
            Reg("hydra_venom", "Hydra Venom", "Deadly venom of the Hydra.");
            Reg("death_scythe_tip", "Death Scythe Tip", "Tip of Death's scythe.");
            Reg("fallen_feather", "Fallen Feather", "Feather from a Seraph of Death.");
            Reg("divine_core", "Divine Core", "Core of divine energy.");
            Reg("lucifer_sigil", "Lucifer's Sigil", "Sigil of the fallen angel.");
        }
    }

    // ══════════════════════════════════════════════════════════════════════════════
    // CLASS SYSTEM
    // ══════════════════════════════════════════════════════════════════════════════
    public static class ClassSystem
    {
        public enum PlayerClass { Knight, Assassin, Mage, Archer, Berserker, Dragoon }

        public record ClassData(string Name, string Lore, int BaseHP, int BaseAtk, int BaseStamina);

        public static readonly Dictionary<PlayerClass, ClassData> Classes = new()
        {
            [PlayerClass.Knight]     = new("Knight",     "A balanced warrior with good defense.", 130, 8, 100),
            [PlayerClass.Assassin]   = new("Assassin",   "High damage, fragile.", 100, 10, 90),
            [PlayerClass.Mage]       = new("Mage",       "Powerful magic attacks but low health.", 80, 15, 80),
            [PlayerClass.Archer]     = new("Archer",     "Ranged specialist with good stamina.", 115, 9, 110),
            [PlayerClass.Berserker]  = new("Berserker",  "Becomes stronger when injured.", 120, 7, 100),
            [PlayerClass.Dragoon]    = new("Dragoon",    "Strong all-rounder.", 125, 10, 130)
        };

        public static PlayerClass SelectedClass { get; private set; } = PlayerClass.Knight;

        public static void ChooseClass()
        {
            UI.PrintHeader("CHOOSE YOUR CLASS");
            int i = 1;
            foreach (var (cls, data) in Classes)
            {
                Console.WriteLine($" [{i}] {data.Name}");
                UI.Print($" HP: {data.BaseHP} | ATK: +{data.BaseAtk} | STA: {data.BaseStamina}", ConsoleColor.Cyan);
                UI.Print($" {data.Lore}", ConsoleColor.DarkGray);
                Console.WriteLine();
                i++;
            }

            string choice = UI.ReadChoice(" Enter class number [1-6]: ", new[] { "1","2","3","4","5","6" });
            SelectedClass = (PlayerClass)(int.Parse(choice) - 1);

            var chosen = Classes[SelectedClass];
            Stats.playerMaxHP = Stats.playerHP = chosen.BaseHP;
            Stats.playerAtkBonus = chosen.BaseAtk;
            Stats.playerMaxStamina = Stats.playerStamina = chosen.BaseStamina;

            UI.Print($" ✅ You chose: {chosen.Name}!", ConsoleColor.Green);
            UI.Pause();
        }

        public static int GetBerserkerDamageBonus(int currentHP, int maxHP, int baseDamage)
        {
            if (SelectedClass != PlayerClass.Berserker) return baseDamage;
            double missingHPRatio = (double)(maxHP - currentHP) / maxHP;
            return (int)(baseDamage * (1.0 + missingHPRatio * 0.9));
        }
    }

    // The rest of the code (Leaderboards, Shop, Bonfire, Program, Title) remains almost the same.
    // Only minor text adjustments were made to remove emoji references.

    public static class Leaderboards
    {
        public record LeaderEntry(string Name, int Score, int Kills, int Turns, int HighestTier, bool GameCleared, string Rank);

        public static List<LeaderEntry> leaderboard = new();

        public static (string rank, ConsoleColor color) GetRank(int score)
        {
            return score switch
            {
                >= 13000 => ("SSS - DIVINE SLAYER", ConsoleColor.Magenta),
                >= 10000 => ("SS - LEGENDARY", ConsoleColor.Red),
                >= 7500  => ("S - CHAMPION", ConsoleColor.Yellow),
                >= 5000  => ("A - VETERAN", ConsoleColor.Cyan),
                >= 3500  => ("B - WARRIOR", ConsoleColor.Green),
                >= 2000  => ("C - FIGHTER", ConsoleColor.White),
                >= 1000  => ("D - ADVENTURER", ConsoleColor.Gray),
                >= 500   => ("E - ROOKIE", ConsoleColor.DarkGray),
                _        => ("F - DEFEATED", ConsoleColor.DarkRed)
            };
        }

        public static void ShowFinalScore()
        {
            UI.PrintHeader("GAME RESULT", ConsoleColor.Yellow);
            int highestTier = Stats.monstersKilled == 0 ? 1 : Math.Min(5, (Stats.monstersKilled - 1) / 5 + 1);

            UI.Print($" Final Score     : {Stats.totalScore}", ConsoleColor.Cyan);
            UI.Print($" Monsters Slain  : {Stats.monstersKilled}/25", ConsoleColor.Cyan);
            UI.Print($" Highest Tier    : {new string('*', highestTier)}", ConsoleColor.Cyan);
            UI.Print($" Total Turns     : {Stats.totalTurns}", ConsoleColor.Cyan);

            var (rank, color) = GetRank(Stats.totalScore);
            Console.Write(" Final Rank: ");
            UI.Print(rank, color);
            UI.Pause("\n Press any key to exit...");
        }
    }

    public static class Shop
    {
        public static int purchaseCount = 0;
        public static int POTION_COST => 100 + purchaseCount * 50;
        public static int SWORD_COST => 200 + purchaseCount * 50;
        public static int ARMOR_COST => 150 + purchaseCount * 50;
        public static int RELIC_COST => 50 + purchaseCount * 50;

        public static void RunShop()
        {
            bool inShop = true;
            while (inShop)
            {
                UI.PrintHeader("ITEM SHOP");
                UI.Print($" Gold: {Stats.totalScore} pts", ConsoleColor.Yellow);
                Console.WriteLine($" [1] Health Potion - Cost: {POTION_COST} pts");
                Console.WriteLine($" [2] Sword Upgrade - Cost: {SWORD_COST} pts (+2 ATK)");
                Console.WriteLine($" [3] Armor Upgrade - Cost: {ARMOR_COST} pts (+10 Max HP)");
                Console.WriteLine($" [4] Holy Relic    - Cost: {RELIC_COST} pts (+1 Stamina Regen)");
                Console.WriteLine(" [5] Leave");

                string choice = UI.ReadChoice(" Choose: ", new[] { "1", "2", "3", "4", "5" });

                if (choice == "5") { inShop = false; continue; }

                int cost = choice switch { "1" => POTION_COST, "2" => SWORD_COST, "3" => ARMOR_COST, "4" => RELIC_COST, _ => 0 };

                if (Stats.totalScore < cost)
                {
                    UI.Print(" ❌ Not enough gold!", ConsoleColor.Red);
                }
                else
                {
                    Stats.totalScore -= cost;
                    purchaseCount++;
                    switch (choice)
                    {
                        case "1": Stats.extraPotions++; UI.Print(" Potion purchased!", ConsoleColor.Magenta); break;
                        case "2": Stats.playerAtkBonus += 2; UI.Print($" ATK +2 (Total: +{Stats.playerAtkBonus})", ConsoleColor.Green); break;
                        case "3": Stats.playerMaxHP += 10; Stats.playerHP += 10; UI.Print($" Max HP +10", ConsoleColor.Cyan); break;
                        case "4": Stats.staminaRegen += 1; UI.Print($" Stamina Regen +1", ConsoleColor.Blue); break;
                    }
                }
                UI.Pause();
            }
        }
    }

    public static class Bonfire
    {
        public static void RunBonfire()
        {
            UI.PrintHeader("BONFIRE REST");
            Stats.playerHP = Math.Min(Stats.playerMaxHP, Stats.playerHP + 100);
            Stats.playerStamina = Stats.playerMaxStamina;
            UI.Print(" You are fully restored!", ConsoleColor.Green);
            UI.Pause();
        }
    }

    public static class Program
    {
        public static readonly Random rand = new Random();

        public static void ResetGame()
        {
            Stats.totalScore = 0;
            Stats.monstersKilled = 0;
            Stats.totalTurns = 0;
            Stats.playerHP = 100;
            Stats.playerMaxHP = 100;
            Stats.playerStamina = 100;
            Stats.playerMaxStamina = 100;
            Stats.playerAtkBonus = 0;
            Stats.extraPotions = 0;
            Stats.staminaRegen = 10;
            Inventory._bag.Clear();
            Shop.purchaseCount = 0;
        }

        public static bool RunBattle()
        {
            var currentTier = (Stats.MonsterRarity)Math.Clamp(Stats.monstersKilled / 5 + 1, 1, 5);
            int battleInTier = Stats.monstersKilled % 5 + 1;
            var monster = Stats.GetMonsterStats(currentTier);

            int maxHP = monster.MaxHP + (battleInTier - 1) * 8;
            int atkMin = monster.AtkMin + (battleInTier - 1);
            int atkMax = monster.AtkMax + (battleInTier - 1) * 2;
            int score = monster.Score + (battleInTier - 1) * 20;

            int monsterHP = maxHP;
            int potions = 2 + Stats.extraPotions;
            int turn = 1;

            UI.PrintHeader($"BATTLE {battleInTier}/5 - TIER {(int)currentTier}", ConsoleColor.Red);
            UI.Print($" A wild {monster.Name} appears!", ConsoleColor.Cyan);

            while (Stats.playerHP > 0 && monsterHP > 0)
            {
                int playerShield = 0;
                UI.Print($"\n══════════ TURN {turn} ══════════", ConsoleColor.White);
                Console.WriteLine($" Player HP: {Stats.playerHP}/{Stats.playerMaxHP} | Stamina: {Stats.playerStamina}/{Stats.playerMaxStamina}");
                Console.WriteLine($" Monster HP: {monsterHP}/{maxHP}");

                string move = UI.ReadChoice("\n [atk] Attack [def] Defend [item] Potion [inv] Inventory : ",
                                            new[] { "ATK", "DEF", "ITEM", "INV" });

                if (move == "ATK")
                {
                    if (Stats.playerStamina >= 10)
                    {
                        Stats.playerStamina -= 10;
                        int dmg = rand.Next(5, 21) + Stats.playerAtkBonus;
                        dmg = ClassSystem.GetBerserkerDamageBonus(Stats.playerHP, Stats.playerMaxHP, dmg);
                        monsterHP = Math.Max(0, monsterHP - dmg);
                        UI.Print($" You dealt {dmg} damage!", ConsoleColor.Green);
                    }
                    else
                    {
                        UI.Print(" Not enough stamina to attack!", ConsoleColor.DarkYellow);
                    }
                }
                else if (move == "DEF")
                {
                    playerShield = rand.Next(8, 18);
                    Stats.playerStamina = Math.Min(Stats.playerMaxStamina, Stats.playerStamina + Stats.staminaRegen);
                    UI.Print($" Shield +{playerShield} | Stamina +{Stats.staminaRegen}", ConsoleColor.Cyan);
                }
                else if (move == "ITEM")
                {
                    if (potions > 0)
                    {
                        int heal = rand.Next(20, 35);
                        Stats.playerHP = Math.Min(Stats.playerMaxHP, Stats.playerHP + heal);
                        potions--;
                        UI.Print($" Healed +{heal} HP (Potions left: {potions})", ConsoleColor.Magenta);
                    }
                    else
                    {
                        UI.Print(" Out of potions!", ConsoleColor.Red);
                        continue;
                    }
                }
                else if (move == "INV")
                {
                    Inventory.ShowInventory();
                    continue;
                }

                Stats.totalTurns++;
                if (monsterHP <= 0) break;

                // Monster Turn
                if (rand.Next(2) == 0)
                {
                    int mDmg = rand.Next(atkMin, atkMax);
                    int net = Math.Max(0, mDmg - playerShield);
                    Stats.playerHP -= net;
                    UI.Print($" {monster.Name} attacks for {net} damage!", ConsoleColor.Red);
                }
                else
                {
                    UI.Print($" {monster.Name} is preparing a heavy attack...", ConsoleColor.Gray);
                }

                turn++;
                if (Stats.playerHP <= 0) break;
            }

            if (Stats.playerHP <= 0)
            {
                UI.Print(" YOU DIED!", ConsoleColor.Red);
                return false;
            }

            // Victory
            Stats.monstersKilled++;
            Stats.playerAtkBonus += 1;
            Stats.playerMaxHP += 2;
            Stats.playerHP = Math.Min(Stats.playerMaxHP, Stats.playerHP + 20);
            Stats.totalScore += score + Stats.playerHP;

            Inventory.AddItem(monster.Drop.ItemId);

            UI.Print($" VICTORY! Defeated {monster.Name}!", ConsoleColor.Yellow);
            UI.Pause();

            if (Stats.monstersKilled >= 25) return false;

            Console.WriteLine("\n [1] Shop [2] Bonfire");
            string next = UI.ReadChoice(" Choose: ", new[] { "1", "2" });
            if (next == "1") Shop.RunShop();
            else Bonfire.RunBonfire();

            return true;
        }

        public static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Monster Battle RPG";

            ResetGame();
            Title.ShowTitleScreen();
            ClassSystem.ChooseClass();

            bool playing = true;
            while (playing)
                playing = RunBattle();

            Leaderboards.ShowFinalScore();
            Console.ReadKey();
        }
    }

    public static class Title
    {
        public static void ShowTitleScreen()
        {
            UI.PrintHeader("MONSTER BATTLE RPG", ConsoleColor.Red);
            UI.Print(" Defeat 25 monsters and become a legend!", ConsoleColor.Cyan);
            UI.Pause("\n Press any key to start your journey...");
        }
    }
}
