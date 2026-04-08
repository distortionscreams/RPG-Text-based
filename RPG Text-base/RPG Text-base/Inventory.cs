using System;
using System.Collections.Generic;
using System.Linq;
using static RPG_Text_base.Shop;

namespace RPG_Text_base;

public static class Inventory
{
    // ══════════════════════════════════════════════════════════════
    // ITEM DEFINITION
    // ══════════════════════════════════════════════════════════════
    public enum ItemType { Consumable, Weapon, Armor, Relic, Misc }

    public record Item(
        string Id,
        string Name,
        string Emoji,
        ItemType Type,
        string Description
    );

    // Registry toàn cục — thêm item vào đây khi mở rộng game
    public static readonly Dictionary<string, Item> AllItems = new();

    // ══════════════════════════════════════════════════════════════
    // PLAYER BAG
    // ══════════════════════════════════════════════════════════════
    private static readonly Dictionary<string, int> _bag = new();

    public static int MaxSlots { get; private set; } = 20;
    public static int TotalItems => _bag.Values.Sum();

    // ── Thêm item ──────────────────────────────────────────────
    public static bool AddItem(string itemId, int amount = 1)
    {
        if (!AllItems.ContainsKey(itemId))
        {
            PrintColor(ConsoleColor.Red, $" ❌ Unknown item: \"{itemId}\"");
            return false;
        }

        if (TotalItems + amount > MaxSlots)
        {
            PrintColor(ConsoleColor.DarkYellow,
                $" ⚠️ Inventory full! ({TotalItems}/{MaxSlots} slots used)");
            return false;
        }

        _bag.TryGetValue(itemId, out int current);
        _bag[itemId] = current + amount;
        return true;
    }

    // ── Xoá item ───────────────────────────────────────────────
    public static bool RemoveItem(string itemId, int amount = 1)
    {
        if (!_bag.TryGetValue(itemId, out int count) || count < amount)
            return false;

        _bag[itemId] = count - amount;
        if (_bag[itemId] <= 0)
            _bag.Remove(itemId);

        return true;
    }

    public static int GetCount(string itemId)
        => _bag.TryGetValue(itemId, out int n) ? n : 0;

    public static bool HasItem(string itemId) => GetCount(itemId) > 0;

    // ── Mở rộng túi ───────────────────────────────────────────
    public static void ExpandSlots(int extra = 5) => MaxSlots += extra;

    // ── Reset khi bắt đầu ván mới ─────────────────────────────
    public static void Reset()
    {
        _bag.Clear();
        MaxSlots = 20;
    }

    // ══════════════════════════════════════════════════════════════
    // SHOW INVENTORY (with item selection)
    // ══════════════════════════════════════════════════════════════
    public static void ShowInventory()
    {
        while (true)
        {
            Console.Clear();
            PrintColor(ConsoleColor.Yellow, "╔══════════════════════════════════════════╗");
            PrintColor(ConsoleColor.Yellow, "║          🎒   INVENTORY   🎒              ║");
            PrintColor(ConsoleColor.Yellow, "╚══════════════════════════════════════════╝");

            Console.WriteLine($"\n  Slots used: {TotalItems}/{MaxSlots}");
            Console.WriteLine("  ─────────────────────────────────────────");

            if (_bag.Count == 0)
            {
                Console.WriteLine();
                PrintColor(ConsoleColor.DarkGray, "  (Your inventory is empty)");
                Console.WriteLine();
                Console.WriteLine("  [B] Back");
                Console.Write("\n  Choose: ");
                if (Console.ReadLine()?.Trim().ToUpper() == "B") return;
                continue;
            }

            // Hiển thị danh sách items
            Console.WriteLine();
            var itemList = _bag.ToList();

            for (int i = 0; i < itemList.Count; i++)
            {
                var (id, qty) = itemList[i];
                Item item = AllItems[id];

                Console.Write($"  [{i + 1}] {item.Emoji}  ");
                PrintColor(ConsoleColor.Cyan, $"{item.Name}  x{qty}");
                PrintColor(ConsoleColor.DarkGray, $"       {item.Description}");
                Console.WriteLine();
            }

            Console.WriteLine("  ─────────────────────────────────────────");
            Console.WriteLine("  [B] Back");
            Console.Write("\n  Enter item number to view details (or B to go back): ");

            string input = Console.ReadLine()?.Trim().ToUpper();

            if (input == "B" || input == "BACK")
                return;

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= itemList.Count)
            {
                var selected = itemList[choice - 1];
                Item item = AllItems[selected.Key];
                int quantity = selected.Value;

                Console.Clear();
                PrintColor(ConsoleColor.Magenta, "╔══════════════════════════════════════════════╗");
                PrintColor(ConsoleColor.Magenta, "║                ITEM DETAILS                   ║");
                PrintColor(ConsoleColor.Magenta, "╚══════════════════════════════════════════════╝\n");

                Console.WriteLine($"  {item.Emoji}   {item.Name}");
                Console.WriteLine($"  Quantity: {quantity}\n");
                PrintColor(ConsoleColor.White, $"  {item.Description}");

                Console.WriteLine("\n" + new string('─', 50));
                Console.WriteLine("  Press Enter to return to inventory...");
                Console.ReadLine();
            }
            else
            {
                PrintColor(ConsoleColor.Red, "  ❌ Invalid choice!");
                Thread.Sleep(1200);
            }
        }
    }

    // ── Hiển thị nhanh trong combat HUD ───────────────────────
    public static void PrintInventorySummary()
    {
        if (_bag.Count == 0)
        {
            PrintColor(ConsoleColor.DarkGray, "  🎒 Inventory: empty");
            return;
        }

        var parts = _bag.Select(kvp =>
        {
            Item item = AllItems[kvp.Key];
            return $"{item.Emoji} {item.Name} x{kvp.Value}";
        });

        PrintColor(ConsoleColor.Magenta, $"  🎒 Inventory: {string.Join("  |  ", parts)}");
    }

    // ══════════════════════════════════════════════════════════════
    // HELPERS
    // ══════════════════════════════════════════════════════════════
    private static void PrintColor(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    // ══════════════════════════════════════════════════════════════
    // REGISTER ALL DROP ITEMS
    // ══════════════════════════════════════════════════════════════
    static Inventory()
    {
        // 1-Star
        RegisterItem("rotten_flesh", "Rotten Flesh", "🥩", ItemType.Misc, "Flesh from a zombie. Smells terrible.");
        RegisterItem("bone_shard", "Bone Shard", "🦴", ItemType.Misc, "Sharp bone fragment from a skeleton.");
        RegisterItem("slime_gel", "Slime Gel", "💧", ItemType.Misc, "Sticky gel from a slime.");
        RegisterItem("wolf_fang", "Wolf Fang", "🦷", ItemType.Misc, "Sharp fang from a wild wolf.");
        RegisterItem("ectoplasm", "Ectoplasm", "👻", ItemType.Misc, "Ghostly residue.");
        RegisterItem("cursed_coin", "Cursed Coin", "🪙", ItemType.Misc, "Coin that brings bad luck.");
        RegisterItem("grave_dust", "Grave Dust", "💨", ItemType.Misc, "Dust from an old grave.");

        // 2-Star
        RegisterItem("bat_wing", "Bat Wing", "🦇", ItemType.Misc, "Leathery wing of a giant bat.");
        RegisterItem("lizard_scale", "Lizard Scale", "🦎", ItemType.Misc, "Tough scale from a lizardman.");
        RegisterItem("dark_essence", "Dark Essence", "🌑", ItemType.Misc, "Pure darkness in liquid form.");
        RegisterItem("flesh_chunk", "Flesh Chunk", "✋", ItemType.Misc, "A chunk of rotten flesh.");
        RegisterItem("cursed_eye", "Cursed Eye", "👁️", ItemType.Misc, "An eye that still stares.");
        RegisterItem("iron_emblem", "Iron Emblem", "🛡️", ItemType.Misc, "Emblem of a fallen crusader.");
        RegisterItem("imp_horn", "Imp Horn", "😈", ItemType.Misc, "Small demonic horn.");

        // 3-Star
        RegisterItem("war_blade_shard", "War Blade Shard", "⚔️", ItemType.Misc, "Broken blade of a skeleton warrior.");
        RegisterItem("silver_claw", "Silver Claw", "🌕", ItemType.Misc, "Claw from a werewolf.");
        RegisterItem("wyvern_talon", "Wyvern Talon", "🐲", ItemType.Misc, "Sharp talon of a wyvern.");
        RegisterItem("ghoul_tooth", "Ghoul Tooth", "🦷", ItemType.Misc, "Tooth from a man-eater ghoul.");
        RegisterItem("phantom_blade", "Phantom Blade", "🗡️", ItemType.Misc, "Ghostly sword fragment.");
        RegisterItem("holy_crest", "Holy Crest", "✝️", ItemType.Misc, "Crest of a holy knight.");
        RegisterItem("hellhound_collar", "Hellhound Collar", "🔗", ItemType.Misc, "Collar from Cerberus.");

        // 4-Star
        RegisterItem("blood_vial", "Blood Vial", "🩸", ItemType.Misc, "Vial of vampire blood.");
        RegisterItem("demon_heart", "Demon Heart", "❤️‍🔥", ItemType.Misc, "Still beating demon heart.");
        RegisterItem("hollow_helm", "Hollow Helm", "⛑️", ItemType.Misc, "Empty helmet of a headless knight.");
        RegisterItem("stone_gaze_shard", "Stone Gaze Shard", "💎", ItemType.Misc, "Shard from Medusa's gaze.");
        RegisterItem("broken_halo", "Broken Halo", "👼", ItemType.Misc, "Fallen angel's broken halo.");
        RegisterItem("hellfire_core", "Hellfire Core", "🔥", ItemType.Misc, "Core of pure hellfire.");
        RegisterItem("shadow_fragment", "Shadow Fragment", "🌑", ItemType.Misc, "Piece of pure shadow.");

        // 5-Star
        RegisterItem("dragon_scale", "Dragon Scale", "🐉", ItemType.Misc, "Legendary dragon scale.");
        RegisterItem("abyssal_pearl", "Abyssal Pearl", "🌊", ItemType.Misc, "Pearl from the Leviathan.");
        RegisterItem("hydra_venom", "Hydra Venom", "☠️", ItemType.Misc, "Deadly venom of the Hydra.");
        RegisterItem("death_scythe_tip", "Death Scythe Tip", "💀", ItemType.Misc, "Tip of Death's scythe.");
        RegisterItem("fallen_feather", "Fallen Feather", "🪶", ItemType.Misc, "Feather from a Seraph of Death.");
        RegisterItem("divine_core", "Divine Core", "🌟", ItemType.Misc, "Core of divine energy.");
        RegisterItem("lucifer_sigil", "Lucifer's Sigil", "🔱", ItemType.Misc, "Sigil of the fallen angel.");
    }

    private static void RegisterItem(string id, string name, string emoji, ItemType type, string desc)
    {
        AllItems[id] = new Item(id, name, emoji, type, desc);
    }
}