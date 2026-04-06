using System;
using System.Collections.Generic;
using static RPG_Text_base.Program;
using static RPG_Text_base.Stats;
using static RPG_Text_base.Leaderboards;
using static RPG_Text_base.Shop;
namespace RPG_Text_base;
public static class Title
{
    public static readonly Random rand = new();
    // Shared RNG used across modules (monsters, battles, etc.)

    // ========== ENTRY POINT ==========
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
     
        ShowTitleScreen();
        while (RunBattle()) { }
        ShowFinalScore();
    }

    // ========== TITLE SCREEN ==========
    public static void ShowTitleScreen()
    {
        // Clear and hide cursor for a cleaner title screen
        Console.Clear();
        bool prevCursor = false;
        bool cursorSupported = OperatingSystem.IsWindows();
        if (cursorSupported)
        {
            prevCursor = Console.CursorVisible;
            Console.CursorVisible = false;
        }

        // Decorative title block
        PrintColor(ConsoleColor.Yellow, "╔════════════════════════════════════════════╗");
        PrintColor(ConsoleColor.Yellow, "║          ⚔️  MONSTER BATTLE RPG  ⚔️        ║");
        PrintColor(ConsoleColor.Yellow, "╚════════════════════════════════════════════╝");

        Console.WriteLine();
        PrintColor(ConsoleColor.White, "   Defeat monsters • Earn score • Survive as long as you can!");

        Console.WriteLine("\n   Available commands:");
        Console.WriteLine("     atk   → Attack the monster");
        Console.WriteLine("     def   → Defend (gain shield)");
        Console.WriteLine("     item  → Use Health Potion");

        Console.WriteLine();
        PrintColor(ConsoleColor.Cyan, "   ★ Kill Bonus: Every monster slain gives +1 ATK & +2 Max HP permanently!");

        Console.WriteLine("\n   Press any key to begin your adventure...");
        Console.ReadKey(true);   // true = do not echo key
        if (cursorSupported)
            Console.CursorVisible = prevCursor;
        Console.Clear();
    }

}
