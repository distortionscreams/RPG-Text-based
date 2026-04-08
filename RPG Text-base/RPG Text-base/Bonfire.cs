using System;
using System.Linq;
using System.Collections.Generic;

using static RPG_Text_base.Stats;
using static RPG_Text_base.Title;
using static RPG_Text_base.Leaderboards;
using static RPG_Text_base.Shop;
using static RPG_Text_base.Program;
using static RPG_Text_base.Inventory;

namespace RPG_Text_base;

public static class Bonfire
{
    // ========== BONFIRE SYSTEM ==========

    public static void RunBonfire()
    {
        Console.Clear();

        // ASCII art lửa trại
        PrintColor(ConsoleColor.DarkYellow, "           (   )  (   )  (   )");
        PrintColor(ConsoleColor.Yellow, "            ) (    ) (    ) (  ");
        PrintColor(ConsoleColor.Red, "            (   )  (   )  (   )");
        PrintColor(ConsoleColor.DarkYellow, "             )_(    )_(    )_(  ");
        PrintColor(ConsoleColor.Yellow, "            /|||\\  /|||\\  /|||\\  ");
        PrintColor(ConsoleColor.DarkRed, "           /|||||\\/||||||\\/|||||\\");
        PrintColor(ConsoleColor.DarkYellow, "  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        PrintColor(ConsoleColor.Yellow, "       🔥   YOU REST AT THE BONFIRE   🔥    ");
        PrintColor(ConsoleColor.DarkYellow, "  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

        Console.WriteLine();
        PrintColor(ConsoleColor.DarkGray, "  The warm flames crackle softly in the night.");
        PrintColor(ConsoleColor.DarkGray, "  You close your eyes and feel your strength return...");
        Console.WriteLine();

        // Cơ chế hồi: hồi một lượng HP cố định + hồi đầy Stamina
        int hpHeal = 100;
        int hpBefore = playerHP;
        int staminaBefore = playerStamina;

        playerHP = Math.Min(playerMaxHP, playerHP + hpHeal);
        playerStamina = playerMaxStamina;

        int actualHpHealed = playerHP - hpBefore;
        int actualStaminaHealed = playerStamina - staminaBefore;

        Console.WriteLine("  ┌─── Bonfire Recovery ──────────────────┐");
        PrintColor(ConsoleColor.Green,
            $"  │  ❤️  HP restored     : +{actualHpHealed} → {playerHP}/{playerMaxHP}");
        PrintColor(ConsoleColor.Blue,
            $"  │  ⚡ Stamina restored : +{actualStaminaHealed} → {playerStamina}/{playerMaxStamina}");
        PrintColor(ConsoleColor.Yellow,
            $"  │  ✨ You feel rested and ready!");
        Console.WriteLine("  └───────────────────────────────────────┘");

        Console.WriteLine();
        PrintColor(ConsoleColor.DarkGray, "  The bonfire flickers as you rise, ready to face the darkness again.");
        Console.WriteLine("\n  Press any key to leave the bonfire...");
        Console.ReadKey(true);
        Console.Clear();
    }

}

