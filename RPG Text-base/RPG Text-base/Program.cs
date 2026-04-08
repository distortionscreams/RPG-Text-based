using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static RPG_Text_base.Stats;
using static RPG_Text_base.Title;
using static RPG_Text_base.Leaderboards;
using static RPG_Text_base.Shop;
using static RPG_Text_base.Bonfire;
using static RPG_Text_base.Inventory;

namespace RPG_Text_base;

public static class Program
{
    // Helper methods
    public static void AdvanceTurn(ref int turn) => turn++;

    public static void PauseAndClear()
    {
        Console.WriteLine("\n  Press any key to continue...");
        Console.ReadKey(true);
        Console.Clear();
    }

    public static void PrintColor(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }


    public static readonly Random rand = new Random();   // Rand chính của game

    // ========== BATTLE SYSTEM ==========

    public static MonsterRarity GetCurrentTier()
        => (MonsterRarity)Math.Clamp(monstersKilled / 5 + 1, 1, 5);

    public static int GetBattleInTier()
        => monstersKilled % 5 + 1;

    public static bool IsGameCleared()
        => monstersKilled >= 25;

    public static string GetTierName(MonsterRarity rarity) => rarity switch
    {
        MonsterRarity.OneStar => "Common",
        MonsterRarity.TwoStar => "Uncommon",
        MonsterRarity.ThreeStar => "Rare",
        MonsterRarity.FourStar => "Epic",
        MonsterRarity.FiveStar => "Legendary",
        _ => "???"
    };

    // ── Helper hiển thị Gear ──
    public static string BuildGearLine()
    {
        return playerAtkBonus > 0 ? $"ATK Bonus: +{playerAtkBonus}" : "";
    }

    // ── Print Health Bar ──
    public static void PrintHealthBar(string label, int current, int max, ConsoleColor color)
    {
        double percent = (double)current / max;
        int barLength = 20;
        int filled = (int)(percent * barLength);

        // Đã sửa lỗi string concatenation
        string bar = new string('█', filled) + new string('░', barLength - filled);

        PrintColor(color, $"  {label,-10} [{bar}] {current}/{max} HP");
    }

    public static bool RunBattle()
    {
        // ── Setup ──────────────────────────────────────────────
        MonsterRarity currentTier = GetCurrentTier();
        int battleInTier = GetBattleInTier();
        int overallBattle = monstersKilled + 1;

        MonsterStats monster = GetMonsterStats(currentTier);
        string rarityStars = new string('⭐', (int)currentTier);
        string tierName = GetTierName(currentTier);

        // Scale độ khó dần trong tier
        int tierScale = battleInTier - 1;
        int maxHP = monster.MaxHP + tierScale * 8;
        int atkMin = monster.AtkMin + tierScale;
        int atkMax = monster.AtkMax + tierScale * 2;
        int defMin = monster.DefMin;
        int defMax = monster.DefMax;
        int score = monster.Score + tierScale * 20;

        // Combat variables
        int playerShield = 0;
        int monsterHP = maxHP;
        int monsterShield = 0;
        int potions = 2 + extraPotions;
        int turn = 1;

        // ── Battle Intro ───────────────────────────────────────
        Console.Clear();
        Console.WriteLine("════════════════════════════════════════");
        PrintColor(ConsoleColor.Yellow, $"  ⚠️  Battle {battleInTier}/5  [Tier {(int)currentTier} — {tierName}]");
        PrintColor(ConsoleColor.DarkGray, $"  Overall progress: {overallBattle}/25");
        Console.WriteLine("════════════════════════════════════════");

        if (battleInTier == 1 && monstersKilled > 0)
            PrintColor(ConsoleColor.Magenta, $"\n  ★ TIER UP! You've entered {tierName} territory! {rarityStars}");

        PrintColor(ConsoleColor.Cyan, $"\n  A wild {monster.Emoji} {monster.Name.ToUpper()} appears!");
        PrintColor(ConsoleColor.DarkYellow, $"  {rarityStars}  [{tierName}]");
        Console.WriteLine($"  HP: {maxHP} | ATK: {atkMin}–{atkMax} | Score reward: {score} pts");

        string gearLine = BuildGearLine();
        if (!string.IsNullOrEmpty(gearLine))
            PrintColor(ConsoleColor.Magenta, $"\n  Your gear: {gearLine}");

        Console.WriteLine($"\n  Current Score: {totalScore} pts | Monsters slain: {monstersKilled}");
        Console.WriteLine($"  ❤️  HP: {playerHP}/{playerMaxHP}  |  ⚡ Stamina: {playerStamina}/{playerMaxStamina}");

        Console.WriteLine("\n  Press any key to start fighting...");
        Console.ReadKey(true);
        Console.Clear();

        // ── Combat Loop ────────────────────────────────────────
        while (playerHP > 0 && monsterHP > 0)
        {
            PrintColor(ConsoleColor.White, $"\n══════════ TURN {turn}  [{monster.Emoji} {monster.Name} {rarityStars}] ══════════");

            PrintHealthBar("YOU      ", playerHP, playerMaxHP, ConsoleColor.Green);
            if (playerShield > 0)
                PrintColor(ConsoleColor.Cyan, $"           🛡️  Shield: {playerShield}");

            PrintHealthBar($"{monster.Emoji} {monster.Name,-10}", monsterHP, maxHP, ConsoleColor.Red);
            if (monsterShield > 0)
                PrintColor(ConsoleColor.Cyan, $"           🛡️  Shield: {monsterShield}");

            PrintColor(ConsoleColor.DarkYellow, $"  🏆 Score: {totalScore} pts  |  ☠️  Slain: {monstersKilled}  |  🧪 Potions: {potions}");
            PrintColor(ConsoleColor.Blue, $"  ⚡ Stamina: {playerStamina}/{playerMaxStamina}");
            PrintColor(ConsoleColor.DarkGray, $"  Progress: Battle {overallBattle}/25  (Tier {(int)currentTier}: {battleInTier}/5)");
            Console.WriteLine("────────────────────────────────────");

            // Player Input
            string input = "";
            bool canAttack = playerStamina >= 10;

            while (true)
            {
                if (canAttack)
                    Console.Write("\n  Your move [atk / def / item / inv]: ");
                else
                {
                    PrintColor(ConsoleColor.DarkYellow, "  ⚠️  Not enough stamina! You must DEFEND to recover stamina.");
                    Console.Write("\n  Your move [def / item / inv]: ");
                }

                input = Console.ReadLine()?.Trim().ToLower() ?? "";

                if (!canAttack && input == "atk")
                {
                    PrintColor(ConsoleColor.DarkYellow, "  ⚠️  You don't have enough stamina to attack!");
                    continue;
                }

                if (input == "atk" || input == "def" || input == "item" || input == "inv")
                    break;

                PrintColor(ConsoleColor.DarkYellow, "  ⚠️  Invalid command!");
            }

            Console.WriteLine();
            bool skipMonsterTurn = false;

            switch (input)
            {
                case "atk":
                    {
                        playerStamina -= 10;

                        int dmg = rand.Next(5, 21) + playerAtkBonus;   
                        bool isCrit = rand.Next(100) < 5;
                        if (isCrit) dmg = (int)(dmg * 2.0);

                        int netDmg = Math.Max(0, dmg - monsterShield);
                        monsterHP = Math.Max(0, monsterHP - netDmg);
                        monsterShield = 0;

                        string atkMsg = $"  ⚔️  You attack for {dmg} damage";
                        if (playerAtkBonus > 0) atkMsg += $" (+{playerAtkBonus} bonus)";

                        PrintColor(ConsoleColor.Green, atkMsg + "!");
                        if (isCrit)
                            PrintColor(ConsoleColor.Yellow, "  💥 CRITICAL HIT!");

                        if (netDmg < dmg)
                            Console.WriteLine($"  🛡️  Shield absorbed → net damage: {netDmg}");

                        if (monsterHP > 0)
                            Console.WriteLine($"  {monster.Name} has {monsterHP} HP left.");

                        if (monsterHP <= 0) goto EndCombat;

                        if (isCrit)
                        {
                            PrintColor(ConsoleColor.Yellow, $"  😵 {monster.Name} is stunned and skips its turn!");
                            skipMonsterTurn = true;
                        }
                        break;
                    }

                case "def":
                    playerShield = rand.Next(8, 18);
                    playerStamina = Math.Min(playerMaxStamina, playerStamina + staminaRegen);
                    PrintColor(ConsoleColor.Cyan, $"  🛡️  You brace and gain {playerShield} shield! (+{staminaRegen} Stamina)");
                    break;

                case "item":
                    if (potions > 0)
                    {
                        int heal = rand.Next(20, 35);
                        playerHP = Math.Min(playerMaxHP, playerHP + heal);
                        potions--;
                        PrintColor(ConsoleColor.Magenta, $"  🧪 Drank potion! +{heal} HP (now {playerHP}/{playerMaxHP})");
                        PrintColor(ConsoleColor.Green, "  ✨ Potion does not consume your turn!");
                        AdvanceTurn(ref turn);
                        PauseAndClear();
                        continue;
                    }
                    PrintColor(ConsoleColor.DarkYellow, "  ⚠️  No potions left! Turn wasted.");
                    break;

                case "inv":
                    Console.Clear();
                    Inventory.ShowInventory();
                    PauseAndClear();
                    continue;
            }

            if (monsterHP <= 0) break;

            // Monster's Turn
            if (!skipMonsterTurn)
            {
                Console.WriteLine();
                if (rand.Next(2) == 0) // Attack
                {
                    int mDmg = rand.Next(atkMin, atkMax);
                    int netMDmg = Math.Max(0, mDmg - playerShield);
                    playerHP = Math.Max(0, playerHP - netMDmg);

                    PrintColor(ConsoleColor.Red, $"  {monster.Emoji} {monster.Name} attacks for {mDmg} damage!");

                    if (playerShield > 0)
                        Console.WriteLine($"  🛡️  Your shield absorbed {playerShield} → net: {netMDmg}");

                    playerShield = 0;
                }
                else // Defend
                {
                    monsterShield = rand.Next(defMin, defMax);
                    PrintColor(ConsoleColor.Yellow, $"  {monster.Emoji} {monster.Name} defends and gains {monsterShield} shield.");
                }

                if (playerHP <= 0) break;
            }

            AdvanceTurn(ref turn);
            PauseAndClear();
        }

    EndCombat:
        totalTurns += turn;

        // ── Defeat ────────────────────────────────────────────
        if (playerHP <= 0)
        {
            PrintColor(ConsoleColor.Red, $"  💀 YOU DIED! {monster.Name} has defeated you...");
            Console.WriteLine($"\n  Fell at Battle {overallBattle}/25 ({tierName})");
            Console.WriteLine($"  Final Score   : {totalScore} pts");
            Console.WriteLine($"  Monsters Slain: {monstersKilled}");
            Console.WriteLine($"  Total Turns   : {totalTurns}");
            Console.WriteLine($"\n  Remaining Stamina: {playerStamina}/{playerMaxStamina}");
            Console.WriteLine("\n  Press any key to exit...");
            Console.ReadKey(true);
            return false;
        }

        // ── Victory ───────────────────────────────────────────
        monstersKilled++;
        playerAtkBonus += 1;
        playerMaxHP += 2;

        int hpBonus = playerHP;
        int speedBonus = Math.Max(0, (10 - turn) * 15);
        int earned = score + hpBonus + speedBonus;
        totalScore += earned;

        PrintColor(ConsoleColor.Yellow, $"  🎉 VICTORY! You defeated the {monster.Name}!");

        Console.WriteLine($"\n  ┌─── Score Breakdown ───────────────┐");
        PrintColor(ConsoleColor.Cyan, $"  │  Monster kill    : +{score,4} pts");
        PrintColor(ConsoleColor.Cyan, $"  │  HP remaining    : +{hpBonus,4} pts");
        PrintColor(ConsoleColor.Cyan, $"  │  Speed bonus     : +{speedBonus,4} pts");
        PrintColor(ConsoleColor.Yellow, $"  │  Total earned    : +{earned,4} pts");
        PrintColor(ConsoleColor.Yellow, $"  │  Grand total     :  {totalScore,4} pts");
        Console.WriteLine("  └───────────────────────────────────┘");


        // ── MONSTER DROP ─────────────────────────────────────────────
        Console.WriteLine();

        if (Inventory.AddItem(monster.Drop.ItemId))
        {
            PrintColor(ConsoleColor.Magenta,
                $"  🎁 DROPPED: {monster.Drop.Emoji} {monster.Drop.ItemName}!");
        }
        else
        {
            PrintColor(ConsoleColor.DarkYellow,
                $"  ⚠️  Inventory full! {monster.Drop.Emoji} {monster.Drop.ItemName} bị mất.");
        }




        PrintColor(ConsoleColor.Magenta, $"  ⬆️  Permanent bonus: ATK +1 | Max HP +2");

        Console.WriteLine($"\n  Survived with {playerHP}/{playerMaxHP} HP | Stamina: {playerStamina}/{playerMaxStamina} | {turn} turns.");

        if (monstersKilled % 5 == 0 && monstersKilled > 0)
        {
            int finishedTier = monstersKilled / 5;
            PrintColor(ConsoleColor.Magenta, $"\n  ★ TIER {finishedTier} CLEARED! {new string('⭐', finishedTier)}");

            if (finishedTier < 5)
            {
                string nextTier = GetTierName((MonsterRarity)(finishedTier + 1));
                PrintColor(ConsoleColor.Cyan, $"  ➡️  Next: {nextTier} {new string('⭐', finishedTier + 1)}");
            }
        }

        if (IsGameCleared())
        {
            Console.WriteLine("\n════════════════════════════════════════");
            PrintColor(ConsoleColor.Yellow, "  🏆 ★ GAME CLEAR! ★ 🏆");
            PrintColor(ConsoleColor.Cyan, "  You have conquered all 25 monsters!");
            Console.WriteLine($"\n  Final Score : {totalScore} pts");
            Console.WriteLine($"  Total Turns : {totalTurns}");
            Console.WriteLine($"\n  Final Stamina: {playerStamina}/{playerMaxStamina}");
            Console.WriteLine("\n  Press any key to exit...");
            Console.ReadKey(true);
            return false;
        }

        // ── Chọn Shop hay Bonfire ──────────────────
        Console.WriteLine("\n  Press any key to continue...");
        Console.ReadKey(true);
        Console.Clear();

        Console.WriteLine("════════════════════════════════════════");
        PrintColor(ConsoleColor.Cyan, "  Where do you want to go?");
        Console.WriteLine("    [1] 🛒  Visit the Shop");
        Console.WriteLine("    [2] 🔥  Rest at the Bonfire");

        string dest = ReadChoice("  Enter 1 or 2: ", ["1", "2"]);
        Console.Clear();

        if (dest == "1")
            RunShop();
        else
            RunBonfire();

        // ── Next Action ───────────────────────────────────
        Console.Clear();
        Console.WriteLine("════════════════════════════════════════");
        PrintColor(ConsoleColor.Cyan, "  What do you want to do next?");
        Console.WriteLine("    [1] ⚔️  Continue fighting");
        Console.WriteLine("    [2] 🏠  Retire and see final score");

        string next = ReadChoice("  Enter 1 or 2: ", ["1", "2"]);
        Console.Clear();
        return next == "1";
    }



    public static string ReadChoice(string prompt, string[] valid)
    {
        string input = "";
        while (!valid.Contains(input))
        {
            Console.Write(prompt);
            input = Console.ReadLine()?.Trim() ?? "";
            if (!valid.Contains(input))
                PrintColor(ConsoleColor.DarkYellow, $"  ⚠️  Please enter one of: {string.Join(", ", valid)}");
        }
        return input;
    }
}
