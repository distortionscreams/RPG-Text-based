using System;
using System.Linq;
using System.Collections.Generic;
using static Rpg.Program;
using static Rpg.Stats;
using static Rpg.Leaderboards;
using static Rpg.Shop;

namespace Rpg;

public static class Title
{
    public static readonly Random rand = new Random();

    // ========== LEADERBOARD ==========
    public record LeaderEntry(string Name, int Score, int Kills, int Turns, string Rank);
    public static readonly List<LeaderEntry> leaderboard = new();

    // ========== ENTRY POINT ==========
    public static void Main()
    {
        ShowTitleScreen();
        while (RunBattle()) { }
        ShowFinalScore();
    }

    // ========== TITLE SCREEN ==========
    public static void ShowTitleScreen()
    {
        Console.Clear();
        PrintColor(ConsoleColor.Yellow, "╔══════════════════════════════════════╗");
        PrintColor(ConsoleColor.Yellow, "║       ⚔️   MONSTER BATTLE GAME   ⚔️ ║");
        PrintColor(ConsoleColor.Yellow, "╚══════════════════════════════════════╝");
        Console.WriteLine("\n  Defeat monsters, earn score, survive as long as you can!");
        Console.WriteLine("\n  Commands:");
        Console.WriteLine("    atk  - Attack the monster");
        Console.WriteLine("    def  - Defend (gain shield)");
        Console.WriteLine("    item - Use Health Potion");
        PrintColor(ConsoleColor.Cyan, "\n  Kill bonus: each monster slain gives ATK +1 & Max HP +2 permanently!");
        Console.WriteLine("\n  Press any key to start...");
        Console.ReadKey();
        Console.Clear();
    }
}
