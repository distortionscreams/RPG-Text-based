using System;
using System.Collections.Generic;
using static RPG_Text_base.Program;
using static RPG_Text_base.Stats;
using static RPG_Text_base.Leaderboards;
using static RPG_Text_base.Shop;
using RPG_Text_base;
using static RPG_Text_base.ClassSystem;

namespace RPG_Text_base;

public static class Title
{
    // Shared Random instance used across the entire game
    public static readonly Random rand = new();

    // ========== ENTRY POINT ==========
    public static void Main()
    {
        ShowTitleScreen();
        ClassSystem.ChooseClass();

        // Main game loop: keep battling until player dies or chooses to retire
        while (RunBattle()) { }

        ShowFinalScore();
    }

    // ========== TITLE SCREEN ==========
    public static void ShowTitleScreen()
    {
        Console.Clear();
        Console.WriteLine();

        // ── TOP GLOW BAR ──
        PrintColor(ConsoleColor.DarkRed,
        "         ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░");
        PrintColor(ConsoleColor.Red,
            "         ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒");
        PrintColor(ConsoleColor.Yellow,
            "         ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓");

        Console.WriteLine();

        // ── TITLE ──
        PrintColor(ConsoleColor.Yellow,
        "              * *   MONSTER  BATTLE  RPG   * *");
        PrintColor(ConsoleColor.DarkYellow,
            "            -  -  -  -  -  -  -  -  -  -  -  -  -");

        Console.WriteLine();

        // ── BOTTOM GLOW BAR ──
        PrintColor(ConsoleColor.Yellow,
        "         ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓");
        PrintColor(ConsoleColor.Red,
            "         ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒");
        PrintColor(ConsoleColor.DarkRed,
            "         ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░");

        Console.WriteLine();

        // ── TAGLINE ──
        PrintColor(ConsoleColor.Cyan,
        "        Defeat monsters  >>  Earn score  >>  Survive");

        Console.WriteLine();

        // ── COMMANDS BOX ──
        PrintColor(ConsoleColor.DarkGray,
        "         +---------------------------------------+");
        PrintColor(ConsoleColor.DarkGray, "         |"); PrintColor2(ConsoleColor.White, "  COMMANDS", ConsoleColor.DarkGray, "                                 |");
        PrintColor(ConsoleColor.DarkGray,
            "         +---------------------------------------+");
        PrintColor(ConsoleColor.DarkGray, "         |"); PrintColor2(ConsoleColor.Green, "  atk", ConsoleColor.Gray, "  >>  Attack the monster              |");
        PrintColor(ConsoleColor.DarkGray, "         |"); PrintColor2(ConsoleColor.Cyan, "  def", ConsoleColor.Gray, "  >>  Defend and gain shield          |");
        PrintColor(ConsoleColor.DarkGray, "         |"); PrintColor2(ConsoleColor.Yellow, "  item", ConsoleColor.Gray, " >>  Use Health Potion               |");
        PrintColor(ConsoleColor.DarkGray,
            "         +---------------------------------------+");

        Console.WriteLine();

        // ── BONUS STRIP ──
        PrintColor(ConsoleColor.DarkYellow,
        "         ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░");
        PrintColor(ConsoleColor.DarkGray, "         |");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("  [KILL BONUS]");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("  +1 ATK  and  +2 Max HP  per kill      |");
        Console.ResetColor();
        PrintColor(ConsoleColor.DarkYellow,
        "         ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░");

        Console.WriteLine();

        // ── PROMPT ──
        PrintColor(ConsoleColor.DarkGray,
        "              [ Press any key to begin... ]");
        Console.WriteLine();

        Console.ReadKey(true);
        Console.Clear();
    }

    // ── HELPERS ──
    private static void PrintColor(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    private static void PrintColor2(ConsoleColor c1, string t1, ConsoleColor c2, string t2)
    {
        Console.ForegroundColor = c1;
        Console.Write(t1);
        Console.ForegroundColor = c2;
        Console.WriteLine(t2);
        Console.ResetColor();
    }
}
