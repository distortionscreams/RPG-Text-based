using System;
using System.Linq;
using System.Collections.Generic;
using static RPG_Text_base.Program;
using static RPG_Text_base.Stats;
using static RPG_Text_base.Title;
using static RPG_Text_base.Shop;

namespace RPG_Text_base;

public static class Leaderboards
{
    // ======== DATA ========
    public record LeaderEntry(
        string Name,
        int Score,
        int Kills,
        int Turns,
        int HighestTier,
        bool GameCleared,
        string Rank
    );

    public static List<LeaderEntry> leaderboard = new();

    // ========== SIMPLE RANK SYSTEM (CHỈ DỰA VÀO SCORE) ==========
    public static (string rank, ConsoleColor color) GetRank(int score)
    {
        return score switch
        {
            >= 13000 => ("SSS - DIVINE SLAYER 👑🔥", ConsoleColor.Magenta),
            >= 10000 => ("SS  - LEGENDARY ⚡", ConsoleColor.Red),
            >= 7500 => ("S   - CHAMPION 🏆", ConsoleColor.Yellow),
            >= 5000 => ("A   - VETERAN ⭐⭐⭐", ConsoleColor.Cyan),
            >= 3500 => ("B   - WARRIOR ⭐⭐", ConsoleColor.Green),
            >= 2000 => ("C   - FIGHTER ⭐", ConsoleColor.White),
            >= 1000 => ("D   - ADVENTURER", ConsoleColor.Gray),
            >= 500 => ("E   - ROOKIE", ConsoleColor.DarkGray),
            _ => ("F   - DEFEATED 💀", ConsoleColor.DarkRed)
        };
    }

    // ========== FINAL SCORE SCREEN ==========
    public static void ShowFinalScore()
    {
        Console.Clear();

        int highestTierReached = monstersKilled == 0 ? 1 : Math.Min(5, (monstersKilled - 1) / 5 + 1);
        bool gameCleared = IsGameCleared();

        var (rank, rankColor) = GetRank(totalScore);

        // ── Header ──────────────────────────────────────────────────────────
        if (gameCleared)
        {
            PrintColor(ConsoleColor.Yellow, "╔══════════════════════════════════════════╗");
            PrintColor(ConsoleColor.Yellow, "║      🏆  GAME CLEAR — HALL OF FAME  🏆    ║");
            PrintColor(ConsoleColor.Yellow, "╚══════════════════════════════════════════╝\n");
        }
        else
        {
            PrintColor(ConsoleColor.Red, "╔══════════════════════════════════════════╗");
            PrintColor(ConsoleColor.Red, "║            💀  GAME OVER  💀              ║");
            PrintColor(ConsoleColor.Red, "╚══════════════════════════════════════════╝\n");
        }

        // ── Stats ───────────────────────────────────────────────────────────
        PrintColor(ConsoleColor.Cyan, $"  Final Score     : {totalScore,7} pts");
        PrintColor(ConsoleColor.Cyan, $"  Monsters Slain  : {monstersKilled,7} / 25");
        PrintColor(ConsoleColor.Cyan, $"  Highest Tier    : {new string('⭐', highestTierReached)} (Tier {highestTierReached})");
        PrintColor(ConsoleColor.Cyan, $"  Total Turns     : {totalTurns,7}");

        double avgTurns = monstersKilled > 0 ? Math.Round((double)totalTurns / monstersKilled, 1) : 0;
        PrintColor(ConsoleColor.DarkGray, $"  Avg turns/kill  : {avgTurns,7}");

        if (playerAtkBonus > 0 || playerMaxHP > 100)
            PrintColor(ConsoleColor.Magenta,
                $"\n  Final Stats      : ATK +{playerAtkBonus} | Max HP {playerMaxHP}");

        // ── Rank display ─────────────────────────────────────────────────────
        Console.WriteLine();
        Console.Write("  Final Rank: ");
        PrintColor(rankColor, rank);
        Console.WriteLine();

        // ── Rank reference card (đơn giản hơn) ───────────────────────────────
        ShowRankCard();

        // ── Leaderboard entry ────────────────────────────────────────────────
        Console.Write("\n  Enter your name for the leaderboard: ");
        string playerName = Console.ReadLine()?.Trim() ?? "Unknown";
        if (string.IsNullOrWhiteSpace(playerName)) playerName = "Unknown";

        leaderboard.Add(new LeaderEntry(
            playerName, totalScore, monstersKilled, totalTurns,
            highestTierReached, gameCleared, rank
        ));

        ShowLeaderboard();

        Console.WriteLine("\n  Thanks for playing! Press any key to exit...");
        Console.ReadKey();
    }

    // ========== RANK REFERENCE CARD (ĐƠN GIẢN) ==========
    public static void ShowRankCard()
    {
        Console.WriteLine();
        PrintColor(ConsoleColor.DarkGray, "  ┌─── Rank Requirements (by Score only) ──────────────┐");
        PrintColor(ConsoleColor.DarkGray, "  │  Rank  │   Minimum Score   │ Title                     │");
        PrintColor(ConsoleColor.DarkGray, "  ├────────┼───────────────────┼───────────────────────────┤");

        (string label, int reqScore, string title, ConsoleColor col)[] ranks =
        {
            ("SSS", 13000, "DIVINE SLAYER 👑🔥", ConsoleColor.Magenta),
            ("SS ", 10000, "LEGENDARY ⚡", ConsoleColor.Red),
            ("S  ",  7500, "CHAMPION 🏆", ConsoleColor.Yellow),
            ("A  ",  5000, "VETERAN ⭐⭐⭐", ConsoleColor.Cyan),
            ("B  ",  3500, "WARRIOR ⭐⭐", ConsoleColor.Green),
            ("C  ",  2000, "FIGHTER ⭐", ConsoleColor.White),
            ("D  ",  1000, "ADVENTURER", ConsoleColor.Gray),
            ("E  ",   500, "ROOKIE", ConsoleColor.DarkGray),
            ("F  ",     0, "DEFEATED 💀", ConsoleColor.DarkRed),
        };

        foreach (var r in ranks)
        {
            PrintColor(r.col, $"  │   {r.label}   │     {r.reqScore,5} pts     │ {r.title,-25} │");
        }

        PrintColor(ConsoleColor.DarkGray, "  └────────┴───────────────────┴───────────────────────────┘");
    }

    // ========== LEADERBOARD DISPLAY ==========
    public static void ShowLeaderboard()
    {
        Console.Clear();
        PrintColor(ConsoleColor.Yellow, "╔══════════════════════════════════════════════════════════════════╗");
        PrintColor(ConsoleColor.Yellow, "║                    🏅  LEADERBOARD  🏅                           ║");
        PrintColor(ConsoleColor.Yellow, "╚══════════════════════════════════════════════════════════════════╝");

        if (leaderboard.Count == 0)
        {
            PrintColor(ConsoleColor.DarkGray, "\n  No entries yet.\n");
            return;
        }

        // Header
        Console.WriteLine();
        PrintColor(ConsoleColor.DarkGray,
            $"  {"#",-3} {"Name",-14} {"Score",8} {"Kills",6} {"Tier",6}  Rank");
        Console.WriteLine("  " + new string('─', 60));

        var sorted = leaderboard
            .OrderByDescending(e => e.Score)
            .ThenByDescending(e => e.Kills)
            .ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            var e = sorted[i];
            var (rankText, rankColor) = GetRank(e.Score);

            string medal = i switch
            {
                0 => "🥇",
                1 => "🥈",
                2 => "🥉",
                _ => $" {i + 1}."
            };

            string tierStars = new('⭐', e.HighestTier);

            Console.Write($"  {medal,-3} {e.Name,-14} {e.Score,8} {e.Kills,6}  {tierStars,-6}  ");
            PrintColor(rankColor, rankText);
        }

        Console.WriteLine("  " + new string('─', 60));
        Console.WriteLine();

        int clearCount = leaderboard.Count(e => e.GameCleared);
        PrintColor(ConsoleColor.DarkGray,
            $"  {leaderboard.Count} total runs  |  {clearCount} full clears");
        Console.WriteLine();
    }
}