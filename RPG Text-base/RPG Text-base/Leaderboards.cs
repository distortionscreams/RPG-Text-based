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
    int HighestTier, // tier cao nhất đã clear (1–5)
    bool GameCleared, // có clear hết 25 trận không
    string Rank
    );
    public static List<LeaderEntry> leaderboard = new();

    // ========== RANK SYSTEM ==========
    /*
     * Calibration dựa trên actual game numbers:
     *
     * Tier 1 clear (5 kills): score ≈ 350–500 pts/monster → tổng ~2000–3000
     * Tier 2 clear (10 kills): tích lũy ~4500–6500
     * Tier 3 clear (15 kills): tích lũy ~8000–11000
     * Tier 4 clear (20 kills): tích lũy ~13000–18000
     * Tier 5 clear (25 kills): tích lũy ~20000–30000+
     *
     * Score/kill = monster.Score + hpBonus(0–260) + speedBonus(0–150) + tierScale*20
     * Average score/kill per tier:
     *   T1 ≈ 400   T2 ≈ 750   T3 ≈ 1100   T4 ≈ 1800   T5 ≈ 3000
     *
     * Efficiency = turns/kill: avg 4–6 nếu chơi tốt, 8–12 nếu chơi chậm
     */
    public static (string rank, ConsoleColor color) GetRank(int kills, int score, int turns, int highestTier, bool gameCleared)
    {
        double efficiency = kills > 0 ? (double)turns / kills : double.MaxValue;

        // ── SSS: Full clear + score cao + hiệu quả cực cao ──────────────────
        // Yêu cầu: clear all 25, score ≥ 22000 (tương đương đánh nhanh mỗi trận), avg ≤ 5 turns/kill
        if (gameCleared && score >= 22000 && efficiency <= 5.0)
            return ("SSS - DIVINE SLAYER 👑🔥", ConsoleColor.Magenta);

        // ── SS: Full clear hoặc tier 5 + score rất cao ───────────────────────
        // Full clear với score tốt: ~20000+ pts
        if (gameCleared && score >= 18000)
            return ("SS  - LEGENDARY ⚡", ConsoleColor.Red);

        // Không cần full clear nhưng đạt tier 5 và score khủng
        if (kills >= 20 && score >= 14000 && efficiency <= 7.0)
            return ("SS  - LEGENDARY ⚡", ConsoleColor.Red);

        // ── S: Vào tier 5 hoặc clear tier 4 ────────────────────────────────
        // Tier 4 clear (20 kills) + score ổn (~10000+)
        if (kills >= 20 && score >= 10000)
            return ("S   - CHAMPION 🏆", ConsoleColor.Yellow);

        // Tier 5 dù chỉ 1 kill, score vẫn rất cao
        if (highestTier >= 5 && score >= 8000)
            return ("S   - CHAMPION 🏆", ConsoleColor.Yellow);

        // ── A: Clear tier 3 (15 kills) ──────────────────────────────────────
        if (kills >= 15 && score >= 6000)
            return ("A   - VETERAN ⭐⭐⭐", ConsoleColor.Cyan);

        if (kills >= 15 && score >= 4500 && efficiency <= 8.0)
            return ("A   - VETERAN ⭐⭐⭐", ConsoleColor.Cyan);

        // ── B: Clear tier 2 (10 kills) ──────────────────────────────────────
        if (kills >= 10 && score >= 3500)
            return ("B   - WARRIOR ⭐⭐", ConsoleColor.Green);

        if (kills >= 10 && score >= 2500 && efficiency <= 9.0)
            return ("B   - WARRIOR ⭐⭐", ConsoleColor.Green);

        // ── C: Clear tier 1 (5 kills) ────────────────────────────────────────
        if (kills >= 5 && score >= 1500)
            return ("C   - FIGHTER ⭐", ConsoleColor.White);

        if (kills >= 5 && score >= 1000)
            return ("C   - FIGHTER ⭐", ConsoleColor.White);

        // ── D: Vào tier 2 (kills 3–4) ────────────────────────────────────────
        if (kills >= 3 && score >= 600)
            return ("D   - ADVENTURER", ConsoleColor.Gray);

        // ── E: Sống sót 1–2 trận ────────────────────────────────────────────
        if (kills >= 1)
            return ("E   - ROOKIE", ConsoleColor.DarkGray);

        // ── F: Thua ngay trận đầu ────────────────────────────────────────────
        return ("F   - DEFEATED 💀", ConsoleColor.DarkRed);
    }

    // ========== FINAL SCORE SCREEN ==========
    public static void ShowFinalScore()
    {
        Console.Clear();

        // Tính highestTier: nếu monstersKilled = 0 → tier 1, không tính là đã clear tier nào
        int highestTierReached = monstersKilled == 0 ? 1 : Math.Min(5, (monstersKilled - 1) / 5 + 1);
        bool gameCleared = IsGameCleared();

        var (rank, rankColor) = GetRank(monstersKilled, totalScore, totalTurns, highestTierReached, gameCleared);

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

        // ── Rank reference card ──────────────────────────────────────────────
        ShowRankCard(monstersKilled, totalScore, totalTurns, highestTierReached, gameCleared);

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

    // ========== RANK REFERENCE CARD ==========
    // Hiện bảng rank để player biết mình cần gì để lên hạng
    public static void ShowRankCard(int kills, int score, int turns, int highestTier, bool gameCleared)
    {
        Console.WriteLine();
        PrintColor(ConsoleColor.DarkGray, "  ┌─── Rank Requirements ──────────────────────────────┐");
        PrintColor(ConsoleColor.DarkGray, "  │  Rank  │ Kills │   Score   │ Tier │ Notes           │");
        PrintColor(ConsoleColor.DarkGray, "  ├────────┼───────┼───────────┼──────┼─────────────────┤");

        (string label, int reqKills, int reqScore, int reqTier, string note)[] ranks =
    {
        ("SSS", 25, 22000, 5, "Full clear, ≤5t/kill"),
        ("SS ", 20, 18000, 5, "Full clear or T5      "),
        ("S  ", 20, 10000, 4, "T4 clear             "),
        ("A  ", 15,  6000, 3, "T3 clear             "),
        ("B  ", 10,  3500, 2, "T2 clear             "),
        ("C  ",  5,  1500, 1, "T1 clear             "),
        ("D  ",  3,   600, 1, "—                    "),
        ("E  ",  1,     0, 1, "—                    "),
    };

        foreach (var r in ranks)
        {
            bool achieved = kills >= r.reqKills && score >= r.reqScore && highestTier >= r.reqTier;
            string check = achieved ? "✓" : " ";
            ConsoleColor col = achieved ? ConsoleColor.Green : ConsoleColor.DarkGray;
            PrintColor(col, $"  │ {check} {r.label}  │  {r.reqKills,3}  │ {r.reqScore,7} pts │  T{r.reqTier}  │ {r.note}│");
        }

        PrintColor(ConsoleColor.DarkGray, "  └────────┴───────┴───────────┴──────┴─────────────────┘");
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
            $"  {"#",-3} {"Name",-14} {"Score",8} {"Kills",6} {"Turns",6} {"Tier",6} {"Cleared",8}  Rank");
        Console.WriteLine("  " + new string('─', 72));

        var sorted = leaderboard
            .OrderByDescending(e => e.Score)
            .ThenByDescending(e => e.Kills)
            .ThenByDescending(e => e.HighestTier)
            .ThenBy(e => e.Turns)
            .ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            var e = sorted[i];
            var (_, rc) = GetRank(e.Kills, e.Score, e.Turns, e.HighestTier, e.GameCleared);

            string medal = i switch
            {
                0 => "🥇",
                1 => "🥈",
                2 => "🥉",
                _ => $" {i + 1}."
            };

            string clearedMark = e.GameCleared ? "  ✅ CLEAR" : "         ";
            string tierStars = new('⭐', e.HighestTier);

            Console.Write($"  {medal,-3} {e.Name,-14} {e.Score,8} {e.Kills,6} {e.Turns,6}  {tierStars,-6} {clearedMark}  ");
            PrintColor(rc, e.Rank);
        }

        Console.WriteLine("  " + new string('─', 72));
        Console.WriteLine();

        // Summary line
        int clearCount = leaderboard.Count(e => e.GameCleared);
        PrintColor(ConsoleColor.DarkGray,
            $"  {leaderboard.Count} total runs  |  {clearCount} full clears");
        Console.WriteLine();
    }

}
