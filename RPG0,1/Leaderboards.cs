using System;
using System.Linq;
using System.Collections.Generic;

using static Rpg.Program;
using static Rpg.Stats;
using static Rpg.Title;
using static Rpg.Leaderboards;
using static Rpg.Shop;


namespace Rpg;

public static class Leaderboards
{

    // ========== FINAL SCORE & LEADERBOARD ==========
    public static void ShowFinalScore()
    {
        Console.Clear();
        var (rank, rankColor) = GetRank(monstersKilled, totalScore, totalTurns);

        PrintColor(ConsoleColor.Yellow, "╔══════════════════════════════════════╗");
        PrintColor(ConsoleColor.Yellow, "║           🏆  GAME OVER  🏆            ║");
        PrintColor(ConsoleColor.Yellow, "╚══════════════════════════════════════╝\n");
        PrintColor(ConsoleColor.Cyan, $"  Final Score    : {totalScore} pts");
        PrintColor(ConsoleColor.Cyan, $"  Monsters Slain : {monstersKilled}");
        PrintColor(ConsoleColor.Cyan, $"  Total Turns    : {totalTurns}");

        if (playerAtkBonus > 0 || playerMaxHP > 100)
            PrintColor(ConsoleColor.Magenta, $"\n  Final Gear     : ATK +{playerAtkBonus} | Max HP {playerMaxHP}");

        Console.WriteLine();
        Console.Write("  Rank: ");
        PrintColor(rankColor, rank);

        // Add to leaderboard
        Console.Write("\n  Enter your name for the leaderboard: ");
        string playerName = Console.ReadLine()?.Trim() ?? "Unknown";
        if (string.IsNullOrWhiteSpace(playerName)) playerName = "Unknown";

        leaderboard.Add(new LeaderEntry(playerName, totalScore, monstersKilled, totalTurns, rank));

        ShowLeaderboard();

        Console.WriteLine("\n  Thanks for playing! Press any key to exit...");
        Console.ReadKey();
    }

    public static void ShowLeaderboard()
    {
        Console.Clear();
        PrintColor(ConsoleColor.Yellow, "╔══════════════════════════════════════════════════════╗");
        PrintColor(ConsoleColor.Yellow, "║               🏅  LEADERBOARD  🏅                    ║");
        PrintColor(ConsoleColor.Yellow, "╚══════════════════════════════════════════════════════╝");
        Console.WriteLine($"\n  {"#",-3} {"Name",-14} {"Score",7} {"Kills",6} {"Turns",6}  Rank");
        Console.WriteLine("  " + new string('─', 52));

        var sorted = leaderboard
            .OrderByDescending(e => e.Score)
            .ThenByDescending(e => e.Kills)
            .ThenBy(e => e.Turns)
            .ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            var e = sorted[i];
            var (_, rc) = GetRank(e.Kills, e.Score, e.Turns);
            string medal = i switch { 0 => "🥇", 1 => "🥈", 2 => "🥉", _ => $" {i + 1}." };
            Console.Write($"  {medal,-3} {e.Name,-14} {e.Score,7} {e.Kills,6} {e.Turns,6}  ");
            PrintColor(rc, e.Rank);
        }

        Console.WriteLine("  " + new string('─', 52));
    }

    // ========== RANK SYSTEM (harder conditions) ==========
    // Requirements stacked: kills + score threshold + efficiency (turns per kill)
    public static (string rank, ConsoleColor color) GetRank(int kills, int score, int turns)
    {
        double efficiency = kills > 0 ? (double)turns / kills : double.MaxValue; // lower = better

        return (kills, score, efficiency) switch
        {
            // SS: 8+ kills, 3000+ pts, avg ≤12 turns/kill
            _ when kills >= 8 && score >= 3000 && efficiency <= 12
                => ("SS - GOD SLAYER 🔥", ConsoleColor.Red),

            // S: 6+ kills, 2000+ pts, avg ≤15 turns/kill
            _ when kills >= 6 && score >= 2000 && efficiency <= 15
                => ("S  - Legend ⭐", ConsoleColor.Yellow),

            // A: 5+ kills, 1400+ pts
            _ when kills >= 5 && score >= 1400
                => ("A  - Hero", ConsoleColor.Cyan),

            // B: 4+ kills, 900+ pts
            _ when kills >= 4 && score >= 900
                => ("B  - Warrior", ConsoleColor.Green),

            // C: 3+ kills, 500+ pts
            _ when kills >= 3 && score >= 500
                => ("C  - Fighter", ConsoleColor.White),

            // D: 2+ kills
            _ when kills >= 2
                => ("D  - Adventurer", ConsoleColor.Gray),

            // E: 1 kill
            _ when kills >= 1
                => ("E  - Rookie", ConsoleColor.DarkGray),

            // F: 0 kills
            _ => ("F  - Defeated 💀", ConsoleColor.DarkGray)
        };
    }

}
