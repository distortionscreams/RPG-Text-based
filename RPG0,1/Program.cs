using System;
using System.Collections.Generic;
using System.Linq;
using Rpg;

using static Rpg.Stats;
using static Rpg.Title;
using static Rpg.Leaderboards;
using static Rpg.Shop;

namespace Rpg;
public static class Program
{



    // ========== BATTLE ==========
    public static bool RunBattle()
    {
        var (name, maxHP, atkMin, atkMax, defMin, defMax, score, emoji) = GetMonsterStats();

        // Scale difficulty with kills
        int diff = monstersKilled * 5;
        maxHP += diff;
        atkMin += monstersKilled;
        atkMax += monstersKilled * 2;

        // Apply kill bonuses before battle starts
        int playerHP = playerMaxHP;
        int playerShield = 0;
        int monsterHP = maxHP;
        int monsterShield = 0;
        int potions = 2 + extraPotions;
        int turn = 1;

        // --- Intro ---
        PrintColor(ConsoleColor.Yellow, $"\n  ⚠️  Battle #{monstersKilled + 1} begins!");
        PrintColor(ConsoleColor.Cyan, $"  A wild {emoji} {name.ToUpper()} appears!");

        if (monstersKilled > 0)
            Console.WriteLine($"  [POWERED UP! +{diff} HP, +{monstersKilled * 2} ATK]");

        Console.WriteLine($"  HP: {maxHP} | Score reward: {score} pts");

        string gearLine = BuildGearLine();
        if (gearLine != "") PrintColor(ConsoleColor.Magenta, $"\n  Your gear: {gearLine}");

        Console.WriteLine($"\n  Current Score: {totalScore} pts | Monsters slain: {monstersKilled}");
        Console.WriteLine("\n  Press any key to fight...");
        Console.ReadKey();
        Console.Clear();

        // ========== COMBAT LOOP ==========
        while (playerHP > 0 && monsterHP > 0)
        {
            PrintColor(ConsoleColor.White, $"\n══════════════ TURN {turn} ══════════════");
            PrintHealthBar("YOU      ", playerHP, playerMaxHP, ConsoleColor.Green);
            if (playerShield > 0)
                PrintColor(ConsoleColor.Cyan, $"           🛡️  Shield: {playerShield}");

            PrintHealthBar($"{emoji} {name,-8}", monsterHP, maxHP, ConsoleColor.Red);
            if (monsterShield > 0)
                PrintColor(ConsoleColor.Cyan, $"           🛡️  Shield: {monsterShield}");

            PrintColor(ConsoleColor.DarkYellow,
                $"\n  🏆 Score: {totalScore} pts  |  ☠️  Slain: {monstersKilled}  |  🧪 Potions: {potions}");
            Console.WriteLine("────────────────────────────────────");

            // --- Input ---
            string[] validCmds = { "atk", "def", "item" };
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

            if (cmdIdx == 0) // ATK
            {
                int dmg = rand.Next(10, 21) + playerAtkBonus;
                bool isCrit = rand.Next(100) < 15;
                if (isCrit) dmg = (int)(dmg * 1.75);

                int netDmg = Math.Max(0, dmg - monsterShield);
                monsterHP = Math.Max(0, monsterHP - netDmg);
                monsterShield = 0;

                string atkMsg = $"  ⚔️  You attack {name} for {dmg} damage";
                if (playerAtkBonus > 0) atkMsg += $" (includes +{playerAtkBonus} bonus)";
                PrintColor(ConsoleColor.Green, atkMsg + "!");

                if (isCrit)
                    PrintColor(ConsoleColor.Yellow, "  💥 CRITICAL HIT! (15% chance)");
                if (netDmg < dmg)
                    Console.WriteLine($"  🛡️  {name}'s shield absorbs → net damage: {netDmg}");
                if (monsterHP > 0)
                    Console.WriteLine($"  {name} has {monsterHP} HP remaining.");

                if (monsterHP <= 0) break;

                if (isCrit)
                {
                    PrintColor(ConsoleColor.Yellow, $"  😵 {name} is stunned and skips their turn!");
                    skipMonsterTurn = true;
                }
            }
            else if (cmdIdx == 1) // DEF
            {
                playerShield = rand.Next(8, 18);
                PrintColor(ConsoleColor.Cyan, $"  🛡️  You brace yourself and gain {playerShield} shield!");
            }
            else // ITEM
            {
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
                else
                {
                    PrintColor(ConsoleColor.DarkYellow, "  ⚠️  No potions left! You waste your turn.");
                }
            }

            if (monsterHP <= 0) break;

            // --- Monster turn ---
            if (!skipMonsterTurn)
            {
                Console.WriteLine();
                if (rand.Next(2) == 0)
                {
                    int mDmg = rand.Next(atkMin, atkMax);
                    int netMDmg = Math.Max(0, mDmg - playerShield);
                    playerHP = Math.Max(0, playerHP - netMDmg);
                    PrintColor(ConsoleColor.Red, $"  {emoji} {name} attacks you for {mDmg} damage!");
                    if (playerShield > 0)
                        Console.WriteLine($"  🛡️  Your shield absorbs {playerShield} → net damage: {netMDmg}");
                    playerShield = 0;
                }
                else
                {
                    monsterShield = rand.Next(defMin, defMax);
                    PrintColor(ConsoleColor.Yellow, $"  {emoji} {name} is defending! Gains {monsterShield} shield.");
                }

                if (playerHP <= 0) break;
            }

            AdvanceTurn(ref turn);
            PauseAndClear();
        }

        totalTurns += turn;
        Console.WriteLine("\n════════════════════════════════════");

        // --- DEFEAT ---
        if (playerHP <= 0)
        {
            PrintColor(ConsoleColor.Red, $"  💀 YOU DIED! {name} has defeated you...");
            Console.WriteLine($"\n  Final Score   : {totalScore} pts");
            Console.WriteLine($"  Monsters Slain: {monstersKilled}");
            Console.WriteLine($"  Total Turns   : {totalTurns}");
            Console.WriteLine("════════════════════════════════════");
            Console.WriteLine("\n  Press any key to exit...");
            Console.ReadKey();
            return false;
        }

        // --- VICTORY ---
        monstersKilled++;

        // ★ Kill bonus: ATK +1, MaxHP +2 per monster killed
        playerAtkBonus += 1;
        playerMaxHP += 2;

        int hpBonus = playerHP;
        int speedBonus = Math.Max(0, (10 - turn) * 15);
        int earned = score + hpBonus + speedBonus;
        totalScore += earned;

        PrintColor(ConsoleColor.Yellow, $"  🎉 VICTORY! You defeated the {name}!");
        Console.WriteLine($"\n  ┌─── Score Breakdown ───────────────┐");
        PrintColor(ConsoleColor.Cyan, $"  │  Monster kill    : +{score,4} pts");
        PrintColor(ConsoleColor.Cyan, $"  │  HP bonus ({playerHP} HP) : +{hpBonus,4} pts");
        PrintColor(ConsoleColor.Cyan, $"  │  Speed bonus     : +{speedBonus,4} pts");
        PrintColor(ConsoleColor.Yellow, $"  │  Total earned    : +{earned,4} pts");
        PrintColor(ConsoleColor.Yellow, $"  │  Grand total     :  {totalScore,4} pts");
        Console.WriteLine($"  └───────────────────────────────────┘");
        PrintColor(ConsoleColor.Magenta,
            $"  ⬆️  Kill bonus applied: ATK +1 (now +{playerAtkBonus}), Max HP +2 (now {playerMaxHP})");
        Console.WriteLine($"\n  Survived with {playerHP}/{playerMaxHP} HP in {turn} turns.");
        Console.WriteLine($"  Monsters slain so far: {monstersKilled} 🏆");

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