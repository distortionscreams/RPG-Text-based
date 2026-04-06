using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static RPG_Text_base.Stats;
using static RPG_Text_base.Title;
using static RPG_Text_base.Leaderboards;
using static RPG_Text_base.Shop;
namespace RPG_Text_base;





public static class Program
{
    // ========== BATTLE ==========

    // Trả về tier hiện tại (1–5) dựa trên số quái đã giết
    // Mỗi tier gồm 5 trận liên tiếp
    public static MonsterRarity GetCurrentTier()
    {
        int tier = monstersKilled / 5 + 1;
        return (MonsterRarity)Math.Clamp(tier, 1, 5);
    }

    // Trả về số trận trong tier hiện tại (1–5)
    public static int GetBattleInTier() => monstersKilled % 5 + 1;

    // Kiểm tra game đã clear chưa (25 trận xong)
    public static bool IsGameCleared() => monstersKilled >= 25;

    // Lấy tên tier theo rarity
    public static string GetTierName(MonsterRarity rarity) => rarity switch
    {
        MonsterRarity.OneStar => "Common",
        MonsterRarity.TwoStar => "Uncommon",
        MonsterRarity.ThreeStar => "Rare",
        MonsterRarity.FourStar => "Epic",
        MonsterRarity.FiveStar => "Legendary",
        _ => "???"
    };

    public static bool RunBattle()
    {
        // ── Setup ──────────────────────────────────────────────
        MonsterRarity currentTier = GetCurrentTier();
        int battleInTier = GetBattleInTier();
        int overallBattle = monstersKilled + 1;

        MonsterStats monster = GetMonsterStats(currentTier);
        string rarityStars = new('⭐', (int)currentTier);
        string tierName = GetTierName(currentTier);

        // Scale nhẹ trong cùng tier (battleInTier: 1→5, tierScale: 0→4)
        int tierScale = battleInTier - 1;
        int maxHP = monster.MaxHP + tierScale * 8;
        int atkMin = monster.AtkMin + tierScale;
        int atkMax = monster.AtkMax + tierScale * 2;
        int defMin = monster.DefMin;
        int defMax = monster.DefMax;
        int score = monster.Score + tierScale * 20;

        // Combat state
        int playerHP = playerMaxHP;
        int playerShield = 0;
        int monsterHP = maxHP;
        int monsterShield = 0;
        int potions = 2 + extraPotions;
        int turn = 1;

        // ── Intro ──────────────────────────────────────────────
        Console.Clear();
        Console.WriteLine("════════════════════════════════════════");
        PrintColor(ConsoleColor.Yellow,
            $"  ⚠️  Battle {battleInTier}/5  [Tier {(int)currentTier} — {tierName}]");
        PrintColor(ConsoleColor.DarkGray,
            $"  Overall progress: {overallBattle}/25");
        Console.WriteLine("════════════════════════════════════════");

        if (battleInTier == 1 && monstersKilled > 0)
            PrintColor(ConsoleColor.Magenta,
                $"\n  ★ TIER UP! You've entered {tierName} territory! {rarityStars}");

        PrintColor(ConsoleColor.Cyan,
            $"\n  A wild {monster.Emoji} {monster.Name.ToUpper()} appears!");
        PrintColor(ConsoleColor.DarkYellow, $"  {rarityStars}  [{tierName}]");
        Console.WriteLine($"  HP: {maxHP} | ATK: {atkMin}–{atkMax} | Score reward: {score} pts");

        string gearLine = BuildGearLine();
        if (gearLine != "")
            PrintColor(ConsoleColor.Magenta, $"\n  Your gear: {gearLine}");

        Console.WriteLine($"\n  Current Score: {totalScore} pts | Monsters slain: {monstersKilled}");
        Console.WriteLine("\n  Press any key to fight...");
        Console.ReadKey();
        Console.Clear();

        // ── Combat Loop ────────────────────────────────────────
        while (playerHP > 0 && monsterHP > 0)
        {
            PrintColor(ConsoleColor.White,
                $"\n══════════ TURN {turn}  [{monster.Emoji} {monster.Name} {rarityStars}] ══════════");

            PrintHealthBar("YOU      ", playerHP, playerMaxHP, ConsoleColor.Green);
            if (playerShield > 0)
                PrintColor(ConsoleColor.Cyan, $"           🛡️  Shield: {playerShield}");

            PrintHealthBar($"{monster.Emoji} {monster.Name,-10}", monsterHP, maxHP, ConsoleColor.Red);
            if (monsterShield > 0)
                PrintColor(ConsoleColor.Cyan, $"           🛡️  Shield: {monsterShield}");

            PrintColor(ConsoleColor.DarkYellow,
                $"\n  🏆 Score: {totalScore} pts  |  ☠️  Slain: {monstersKilled}  |  🧪 Potions: {potions}");
            PrintColor(ConsoleColor.DarkGray,
                $"  Progress: Battle {overallBattle}/25  (Tier {(int)currentTier}: {battleInTier}/5)");
            Console.WriteLine("────────────────────────────────────");

            // Input
            string[] validCmds = ["atk", "def", "item"];
            int cmdIdx = -1;
            while (cmdIdx == -1)
            {
                Console.Write("\n  Your move [atk / def / item]: ");
                string input = Console.ReadLine()?.Trim().ToLower() ?? "";
                cmdIdx = Array.IndexOf(validCmds, input);
                if (cmdIdx == -1)
                    PrintColor(ConsoleColor.DarkYellow, "  ⚠️  Invalid command! Try: atk, def, or item.");
            }

            Console.WriteLine();
            bool skipMonsterTurn = false;

            switch (cmdIdx)
            {
                case 0: // ATK
                    {
                        int dmg = rand.Next(5, 21) + playerAtkBonus;
                        bool isCrit = rand.Next(100) < 5;
                        if (isCrit) dmg = (int)(dmg * 2);

                        int netDmg = Math.Max(0, dmg - monsterShield);
                        monsterHP = Math.Max(0, monsterHP - netDmg);
                        monsterShield = 0;

                        string atkMsg = $"  ⚔️  You attack {monster.Name} for {dmg} damage";
                        if (playerAtkBonus > 0) atkMsg += $" (includes +{playerAtkBonus} bonus)";
                        PrintColor(ConsoleColor.Green, atkMsg + "!");

                        if (isCrit)
                            PrintColor(ConsoleColor.Yellow, "  💥 CRITICAL HIT! (5% chance)");
                        if (netDmg < dmg)
                            Console.WriteLine($"  🛡️  {monster.Name}'s shield absorbs → net damage: {netDmg}");
                        if (monsterHP > 0)
                            Console.WriteLine($"  {monster.Name} has {monsterHP} HP remaining.");

                        if (monsterHP <= 0) goto EndCombat;

                        if (isCrit)
                        {
                            PrintColor(ConsoleColor.Yellow,
                                $"  😵 {monster.Name} is stunned and skips their turn!");
                            skipMonsterTurn = true;
                        }
                        break;
                    }
                case 1: // DEF
                    playerShield = rand.Next(8, 18);
                    PrintColor(ConsoleColor.Cyan,
                        $"  🛡️  You brace yourself and gain {playerShield} shield!");
                    break;

                default: // ITEM
                    if (potions > 0)
                    {
                        int heal = rand.Next(20, 35);
                        playerHP = Math.Min(playerMaxHP, playerHP + heal);
                        potions--;
                        PrintColor(ConsoleColor.Magenta,
                            $"  🧪 You drink a Health Potion and recover {heal} HP! (HP: {playerHP}/{playerMaxHP})");
                        PrintColor(ConsoleColor.Green, "  ✨ Using a potion doesn't cost your turn!");
                        AdvanceTurn(ref turn);
                        PauseAndClear();
                        continue;
                    }
                    PrintColor(ConsoleColor.DarkYellow, "  ⚠️  No potions left! You waste your turn.");
                    break;
            }

            if (monsterHP <= 0) break;

            // Monster turn
            if (!skipMonsterTurn)
            {
                Console.WriteLine();
                if (rand.Next(2) == 0)
                {
                    int mDmg = rand.Next(atkMin, atkMax);
                    int netMDmg = Math.Max(0, mDmg - playerShield);
                    playerHP = Math.Max(0, playerHP - netMDmg);
                    PrintColor(ConsoleColor.Red,
                        $"  {monster.Emoji} {monster.Name} attacks you for {mDmg} damage!");
                    if (playerShield > 0)
                        Console.WriteLine($"  🛡️  Your shield absorbs {playerShield} → net damage: {netMDmg}");
                    playerShield = 0;
                }
                else
                {
                    monsterShield = rand.Next(defMin, defMax);
                    PrintColor(ConsoleColor.Yellow,
                        $"  {monster.Emoji} {monster.Name} is defending! Gains {monsterShield} shield.");
                }

                if (playerHP <= 0) break;
            }

            AdvanceTurn(ref turn);
            PauseAndClear();
        }

    EndCombat:
        totalTurns += turn;
        Console.WriteLine("\n════════════════════════════════════");

        // ── Defeat ────────────────────────────────────────────
        if (playerHP <= 0)
        {
            PrintColor(ConsoleColor.Red,
                $"  💀 YOU DIED! {monster.Name} has defeated you...");
            Console.WriteLine($"\n  Fell at Battle {overallBattle}/25 (Tier {(int)currentTier} — {tierName})");
            Console.WriteLine($"  Final Score   : {totalScore} pts");
            Console.WriteLine($"  Monsters Slain: {monstersKilled}");
            Console.WriteLine($"  Total Turns   : {totalTurns}");
            Console.WriteLine("════════════════════════════════════");
            Console.WriteLine("\n  Press any key to exit...");
            Console.ReadKey();
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

        PrintColor(ConsoleColor.Yellow,
            $"  🎉 VICTORY! You defeated {monster.Emoji} {monster.Name}!");

        Console.WriteLine($"\n  ┌─── Score Breakdown ───────────────┐");
        PrintColor(ConsoleColor.Cyan, $"  │  Monster kill    : +{score,4} pts");
        PrintColor(ConsoleColor.Cyan, $"  │  HP bonus ({playerHP} HP) : +{hpBonus,4} pts");
        PrintColor(ConsoleColor.Cyan, $"  │  Speed bonus     : +{speedBonus,4} pts");
        PrintColor(ConsoleColor.Yellow, $"  │  Total earned    : +{earned,4} pts");
        PrintColor(ConsoleColor.Yellow, $"  │  Grand total     :  {totalScore,4} pts");
        Console.WriteLine($"  └───────────────────────────────────┘");
        PrintColor(ConsoleColor.Magenta,
            $"  ⬆️  Kill bonus: ATK +1 (now +{playerAtkBonus}), Max HP +2 (now {playerMaxHP})");
        Console.WriteLine($"\n  Survived with {playerHP}/{playerMaxHP} HP in {turn} turns.");

        // Thông báo hoàn thành tier
        bool justFinishedTier = monstersKilled % 5 == 0 && monstersKilled > 0;
        if (justFinishedTier)
        {
            int finishedTierNum = monstersKilled / 5;
            string completedStars = new('⭐', finishedTierNum);
            Console.WriteLine();
            PrintColor(ConsoleColor.Magenta,
                $"  ★ TIER {finishedTierNum} CLEARED! {completedStars} All 5 monsters defeated!");

            if (finishedTierNum < 5)
            {
                string nextName = GetTierName((MonsterRarity)(finishedTierNum + 1));
                PrintColor(ConsoleColor.Cyan,
                    $"  ➡️  Next tier: {nextName} {new string('⭐', finishedTierNum + 1)}");
            }
        }

        // ── Game Clear ────────────────────────────────────────
        if (IsGameCleared())
        {
            Console.WriteLine("\n════════════════════════════════════════");
            PrintColor(ConsoleColor.Yellow, "  🏆 ★ GAME CLEAR! ★ 🏆");
            PrintColor(ConsoleColor.Cyan, "  You have defeated all 25 monsters across 5 tiers!");
            Console.WriteLine($"\n  Final Score   : {totalScore} pts");
            Console.WriteLine($"  Total Turns   : {totalTurns}");
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("\n  Press any key to exit...");
            Console.ReadKey();
            return false;
        }

        Console.WriteLine("\n  Press any key to visit the Shop...");
        Console.ReadKey();
        Console.Clear();
        RunShop();

        Console.Clear();
        Console.WriteLine("════════════════════════════════════");
        PrintColor(ConsoleColor.Cyan, "  What do you want to do next?");
        Console.WriteLine("    [1] ⚔️  Continue fighting");
        Console.WriteLine("    [2] 🏠  Retire and see final score");
        Console.WriteLine();

        string next = ReadChoice("  Enter 1 or 2: ", "1", "2");
        Console.Clear();
        return next == "1";
    }

}
