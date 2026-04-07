using System;
using System.Linq;
using System.Collections.Generic;
using static RPG_Text_base.Program;
using static RPG_Text_base.Stats;
using static RPG_Text_base.Title;
using static RPG_Text_base.Leaderboards;

namespace RPG_Text_base;

public static class Shop
{
    // ======== SHOP PRICES ========
    public const int BASE_POTION_COST = 100;
    public const int BASE_SWORD_COST = 200;
    public const int BASE_ARMOR_COST = 150;
    public const int BASE_RELIC_COST = 50;     // Thánh Tích

    // Số lần đã mua (cộng dồn, không reset)
    public static int purchaseCount = 0;

    // Giá hiện tại = giá gốc + purchaseCount * 50
    public static int POTION_COST => BASE_POTION_COST + purchaseCount * 50;
    public static int SWORD_COST => BASE_SWORD_COST + purchaseCount * 50;
    public static int ARMOR_COST => BASE_ARMOR_COST + purchaseCount * 50;
    public static int RELIC_COST => BASE_RELIC_COST + purchaseCount * 50;   // Thánh Tích

    // ========== SHOP ==========
    public static void RunShop()
    {
        bool inShop = true;
        while (inShop)
        {
            Console.Clear();
            PrintColor(ConsoleColor.Yellow, "╔══════════════════════════════════════╗");
            PrintColor(ConsoleColor.Yellow, "║           🏪   ITEM SHOP   🏪         ║");
            PrintColor(ConsoleColor.Yellow, "╚══════════════════════════════════════╝");
            PrintColor(ConsoleColor.DarkYellow, $"\n  💰 Your gold: {totalScore} pts");
            PrintColor(ConsoleColor.Magenta,
                $"  🗡️  ATK Bonus: +{playerAtkBonus}  |  🛡️  Max HP: {playerMaxHP}  |  ⚡ Stamina Regen: +{staminaRegen}");

            Console.WriteLine("\n  ┌─── Items Available ──────────────────┐");
            PrintShopItem("1", "🧪", "Health Potion", "Restore 20-35 HP in next battle", POTION_COST, totalScore >= POTION_COST);
            PrintShopItem("2", "⚔️ ", "Sword Upgrade", "Permanently +2 ATK per attack", SWORD_COST, totalScore >= SWORD_COST);
            PrintShopItem("3", "🛡️ ", "Armor Upgrade", "Permanently +10 Max HP", ARMOR_COST, totalScore >= ARMOR_COST);
            PrintShopItem("4", "✨", "Holy Relic", "Increase Stamina Regen (+1 per upgrade)", RELIC_COST, totalScore >= RELIC_COST);
            Console.WriteLine("  │                                      │");
            PrintColor(ConsoleColor.DarkGray, "  │  [5] Leave shop                      │");
            Console.WriteLine("  └──────────────────────────────────────┘");

            Console.Write("\n  Choose item [1/2/3/4/5]: ");
            switch (Console.ReadLine()?.Trim() ?? "")
            {
                case "1":
                    BuyItem(POTION_COST, () =>
                    {
                        extraPotions++;
                        PrintColor(ConsoleColor.Magenta, "  ✅ Bought Health Potion! +1 extra potion next battle.");
                    });
                    break;

                case "2":
                    BuyItem(SWORD_COST, () =>
                    {
                        playerAtkBonus += 2;
                        PrintColor(ConsoleColor.Green, $"  ✅ Sword upgraded! ATK permanently +2. (Total: +{playerAtkBonus})");
                    });
                    break;

                case "3":
                    BuyItem(ARMOR_COST, () =>
                    {
                        playerMaxHP += 10;
                        PrintColor(ConsoleColor.Cyan, $"  ✅ Armor upgraded! Max HP +10. (New: {playerMaxHP})");
                    });
                    break;

                case "4":
                    BuyItem(RELIC_COST, () =>
                    {
                        staminaRegen += 1;
                        PrintColor(ConsoleColor.Blue, $"  ✅ Holy Relic upgraded! Stamina Regen +1. (Now: +{staminaRegen} per Defend)");
                    });
                    break;

                case "5":
                    inShop = false;
                    break;

                default:
                    PrintColor(ConsoleColor.DarkYellow, "\n  ⚠️  Invalid choice. Please enter 1-5.");
                    Console.WriteLine("  Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    public static void BuyItem(int cost, Action onSuccess)
    {
        Console.WriteLine();
        if (totalScore < cost)
        {
            PrintColor(ConsoleColor.Red, $"  ❌ Not enough gold! Need {cost} pts, you have {totalScore} pts.");
        }
        else
        {
            totalScore -= cost;
            purchaseCount++;  // Tăng giá tất cả vật phẩm lên 50 cho lần mua tiếp theo
            onSuccess();
            PrintColor(ConsoleColor.DarkYellow, $"  💰 Remaining gold: {totalScore} pts");
            PrintColor(ConsoleColor.DarkRed, $"  📈 Price increased by 50! (Total increase: +{purchaseCount * 50})");
        }
        Console.ResetColor();
        Console.WriteLine("  Press any key to continue...");
        Console.ReadKey();
    }

    // ========== HELPERS ==========
    public static void AdvanceTurn(ref int turn) { turn++; totalTurns++; }

    public static void PauseAndClear()
    {
        Console.WriteLine("\n  Press any key for next turn...");
        Console.ReadKey();
        Console.Clear();
    }

    public static string ReadChoice(string prompt, params string[] valid)
    {
        string input = "";
        while (!valid.Contains(input))
        {
            Console.Write(prompt);
            input = Console.ReadLine()?.Trim() ?? "";
            if (!valid.Contains(input))
                PrintColor(ConsoleColor.DarkYellow, $"  ⚠️  Please enter one of: {string.Join(", ", valid)}");
        }
        return input;
    }

    public static string BuildGearLine()
    {
        var parts = new List<string>();
        if (playerMaxHP != 100) parts.Add($"Max HP={playerMaxHP}");
        if (playerAtkBonus != 0) parts.Add($"ATK Bonus=+{playerAtkBonus}");
        if (staminaRegen != 10) parts.Add($"Stamina Regen=+{staminaRegen}");
        return string.Join(" | ", parts);
    }

    public static void PrintColor(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static void PrintHealthBar(string label, int current, int max, ConsoleColor color)
    {
        const int barLength = 20;
        int filled = Math.Clamp((int)Math.Round((double)current / max * barLength), 0, barLength);
        string bar = new string('█', filled) + new string('░', barLength - filled);

        Console.Write($"  {label} ");
        Console.ForegroundColor = color;
        Console.Write($"[{bar}]");
        Console.ResetColor();
        Console.WriteLine($" {current}/{max}");
    }

    public static void PrintShopItem(string key, string emoji, string name, string desc, int cost, bool canAfford)
    {
        Console.Write($"  │  [{key}] {emoji} {name,-16} {desc,-30} ");
        Console.ForegroundColor = canAfford ? ConsoleColor.Yellow : ConsoleColor.DarkGray;
        Console.Write($"{cost,5} pts{(canAfford ? "" : " ❌")}");
        Console.ResetColor();
        Console.WriteLine("  │");
    }
}