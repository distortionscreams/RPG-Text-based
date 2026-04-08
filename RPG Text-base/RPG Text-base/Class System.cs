using System;
using System.Collections.Generic;
using static RPG_Text_base.Stats;
using static RPG_Text_base.Shop;
using static RPG_Text_base.Program;

namespace RPG_Text_base;

public static class ClassSystem
{
    // ──────────────────────────────────────────────────────────
    //  DỮ LIỆU CLASS — ĐÃ CÂN BẰNG
    // ──────────────────────────────────────────────────────────

    public enum PlayerClass { Knight, Assassin, Mage, Archer, Berserker, Dragoon }

    public record ClassData(
        string Name,
        string Emoji,
        string Lore,
        int BaseHP,      // HP khởi đầu
        int BaseAtk,     // ATK bonus khởi đầu
        int BaseStamina  // STAMINA khởi đầu (mới thêm)
    );

    public static readonly Dictionary<PlayerClass, ClassData> Classes = new()
    {
        [PlayerClass.Knight] = new(
            Name: "Knight",
            Emoji: "🛡️",
            Lore: "Chiến binh phòng thủ vững chắc, sống dai và ổn định.",
            BaseHP: 130,
            BaseAtk: 8,
            BaseStamina: 100
        ),

        [PlayerClass.Assassin] = new(
            Name: "Assassin",
            Emoji: "🗡️",
            Lore: "Nhanh nhẹn, sát thương cao nhưng máu và stamina mỏng.",
            BaseHP: 100,
            BaseAtk: 10,
            BaseStamina: 90
        ),

        [PlayerClass.Mage] = new(
            Name: "Mage",
            Emoji: "🔮",
            Lore: "Sát thương phép thuật mạnh, nhưng cơ thể và stamina yếu ớt.",
            BaseHP: 80,
            BaseAtk: 15,
            BaseStamina: 80
        ),

        [PlayerClass.Archer] = new(
            Name: "Archer",
            Emoji: "🏹",
            Lore: "Cân bằng tốt giữa sát thương, phòng thủ và stamina.",
            BaseHP: 115,
            BaseAtk: 9,
            BaseStamina: 110
        ),

        [PlayerClass.Berserker] = new(
            Name: "Berserker",
            Emoji: "⚡",
            Lore: "Càng đánh càng mạnh khi máu thấp. Stamina trung bình.",
            BaseHP: 120,
            BaseAtk: 7,
            BaseStamina: 100
        ),

        // ── CLASS MỚI ─────────────────────────────────────
        [PlayerClass.Dragoon] = new(
            Name: "Dragoon",
            Emoji: "🐉",
            Lore: "Chiến binh rồng, sức bền và sức mạnh bùng nổ. Stamina cao nhất.",
            BaseHP: 125,
            BaseAtk: 10,
            BaseStamina: 130
        )
    };

    // Class đang được chọn
    public static PlayerClass SelectedClass { get; private set; } = PlayerClass.Knight;
    public static ClassData Current => Classes[SelectedClass];

    // ──────────────────────────────────────────────────────────
    //  CHỌN CLASS (gọi từ Title.cs)
    // ──────────────────────────────────────────────────────────

    public static void ChooseClass()
    {
        Console.Clear();
        PrintColor(ConsoleColor.Yellow, "╔═══════════════════════════════════════════╗");
        PrintColor(ConsoleColor.Yellow, "║        ⚔️   CHOOSE YOUR CLASS   ⚔️         ║");
        PrintColor(ConsoleColor.Yellow, "╚═══════════════════════════════════════════╝");

        Console.WriteLine();
        int i = 1;
        foreach (var (cls, data) in Classes)
        {
            Console.Write($"  [{i}] {data.Emoji}  ");
            PrintColor(ConsoleColor.Cyan, data.Name);
            Console.WriteLine($"       HP: {data.BaseHP}  |  ATK: +{data.BaseAtk}  |  STA: {data.BaseStamina}");
            PrintColor(ConsoleColor.DarkGray, $"       {data.Lore}");
            Console.WriteLine();
            i++;
        }

        string[] validInputs = ["1", "2", "3", "4", "5", "6"];
        string choice = ReadChoice("  Enter class number [1-6]: ", validInputs);

        SelectedClass = (PlayerClass)(int.Parse(choice) - 1);
        ClassData chosen = Current;

        // Áp dụng chỉ số base vào Stats
        playerMaxHP = chosen.BaseHP;
        playerHP = chosen.BaseHP;
        playerAtkBonus = chosen.BaseAtk;
        playerMaxStamina = chosen.BaseStamina;   // ← MỚI THÊM
        playerStamina = chosen.BaseStamina;      // ← MỚI THÊM

        Console.Clear();
        PrintColor(ConsoleColor.Yellow, $"\n  ✅ You chose: {chosen.Emoji} {chosen.Name}!");
        Console.WriteLine($"  Starting HP     : {chosen.BaseHP}");
        Console.WriteLine($"  Starting ATK    : +{chosen.BaseAtk}");
        Console.WriteLine($"  Starting Stamina: {chosen.BaseStamina}");
        PrintColor(ConsoleColor.DarkGray, $"  {chosen.Lore}");

        Console.WriteLine("\n  Press any key to begin your adventure...");
        Console.ReadKey(true);
        Console.Clear();
    }

    // ──────────────────────────────────────────────────────────
    //  BERSERKER PASSIVE: Tăng sát thương khi máu thấp
    // ──────────────────────────────────────────────────────────

    public static int GetBerserkerDamageBonus(int currentHP, int maxHP, int baseDamage)
    {
        if (SelectedClass != PlayerClass.Berserker)
            return baseDamage;

        double missingHPRatio = (double)(maxHP - currentHP) / maxHP;
        double multiplier = 1.0 + (missingHPRatio * 0.9);   // max x1.9 khi gần chết

        return (int)(baseDamage * multiplier);
    }

    // ──────────────────────────────────────────────────────────
    //  HELPERS
    // ──────────────────────────────────────────────────────────

    private static void PrintColor(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    private static string ReadChoice(string prompt, string[] valid)
    {
        string input = "";
        while (!Array.Exists(valid, v => v == input))
        {
            Console.Write(prompt);
            input = Console.ReadLine()?.Trim() ?? "";
            if (!Array.Exists(valid, v => v == input))
                PrintColor(ConsoleColor.DarkYellow, $"  ⚠️  Please enter one of: {string.Join(", ", valid)}");
        }
        return input;
    }
}