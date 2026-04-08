using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



using static RPG_Text_base.Stats;
using static RPG_Text_base.Program;
using static RPG_Text_base.Leaderboards;
using static RPG_Text_base.Shop;
using static RPG_Text_base.Bonfire;
using static RPG_Text_base.Inventory;

using RPG_Text_base;

namespace RPG_Text_base;

public static class Title
{
    // ========== ENTRY POINT ==========
    public static void Main()
    {
        Console.Title = "Monster Battle RPG";

        ShowTitleScreen();
        ClassSystem.ChooseClass();

        // Main game loop
        bool continuePlaying = true;
        while (continuePlaying)
        {
            continuePlaying = RunBattle();
        }

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
        PrintColor2(ConsoleColor.White, "         |", ConsoleColor.DarkGray, "  COMMANDS                                 |");
        PrintColor(ConsoleColor.DarkGray,
            "         +---------------------------------------+");
        PrintColor2(ConsoleColor.Green, "         |", ConsoleColor.Gray, "  atk   >>  Attack the monster              |");
        PrintColor2(ConsoleColor.Cyan, "         |", ConsoleColor.Gray, "  def   >>  Defend and gain shield          |");
        PrintColor2(ConsoleColor.Yellow, "         |", ConsoleColor.Gray, "  item  >>  Use Health Potion               |");
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

        PrintColor(ConsoleColor.DarkGray,
            "              [ Press any key to begin... ]");
        Console.WriteLine();

        Console.ReadKey(true);
        Console.Clear();
    }

    // ========== FINAL SCORE SCREEN ==========
    public static void ShowFinalScore()
    {
        Console.Clear();
        Console.WriteLine("══════════════════════════════════════════════");
        PrintColor(ConsoleColor.Yellow, "              ★ GAME OVER ★");
        Console.WriteLine("══════════════════════════════════════════════");

        PrintColor(ConsoleColor.Cyan, $"  Final Score        : {totalScore} pts");
        PrintColor(ConsoleColor.Magenta, $"  Monsters Slain     : {monstersKilled}/25");
        PrintColor(ConsoleColor.Blue, $"  Total Turns Taken  : {totalTurns}");
        PrintColor(ConsoleColor.Green, $"  Final HP           : {playerHP}/{playerMaxHP}");
        PrintColor(ConsoleColor.Green, $"  Final Stamina      : {playerStamina}/{playerMaxStamina}");

        Console.WriteLine("\n══════════════════════════════════════════════");
        PrintColor(ConsoleColor.DarkYellow, "  Thank you for playing Monster Battle RPG!");
        Console.WriteLine("\n  Press any key to exit...");
        Console.ReadKey(true);
    }
    // ── HELPERS ── (SỬ DỤNG PrintColor từ Program)
    // Không định nghĩa lại PrintColor ở đây nữa

    public static void PrintColor2(ConsoleColor color1, string text1, ConsoleColor color2, string text2)
    {
        Console.ForegroundColor = color1;
        Console.Write(text1);
        Console.ForegroundColor = color2;
        Console.WriteLine(text2);
        Console.ResetColor();
    }

}