using System;
using System.Linq;
using System.Collections.Generic;

using static RPG_Text_base.Program;   // Giữ để dễ gọi một số hàm nếu cần
using static RPG_Text_base.Title;
using static RPG_Text_base.Leaderboards;
using static RPG_Text_base.Shop;

namespace RPG_Text_base;

public static class Stats
{
    // ========== SESSION STATE ==========
    public static int totalScore = 0;
    public static int monstersKilled = 0;
    public static int totalTurns = 0;

    // ========== PLAYER STATS ==========
    public static int playerHP = 100;
    public static int playerMaxHP = 100;
    public static int playerStamina = 100;
    public static int playerMaxStamina = 100;

    public static int playerAtkBonus = 0;
    public static int extraPotions = 0;

    // Stamina regeneration when defending
    public static int staminaRegen = 10;

    // ========== MONSTER DATA ==========

    public enum MonsterRarity
    {
        OneStar = 1,
        TwoStar = 2,
        ThreeStar = 3,
        FourStar = 4,
        FiveStar = 5
    }

    // ── Drop item info ─────────────────────────────────────────
    public record DropInfo(
        string ItemId,
        string ItemName,
        string Emoji
    );

    // ── Monster stats record ──────────────────────
    public record MonsterStats(
        string Name,
        int MaxHP,
        int AtkMin,
        int AtkMax,
        int DefMin,
        int DefMax,
        int Score,
        string Emoji,
        DropInfo Drop
    );

    // ── Lấy monster ngẫu nhiên theo rarity ───────────────────
    public static MonsterStats GetMonsterStats(MonsterRarity rarity)
    {
        MonsterStats[] pool = rarity switch
        {
            MonsterRarity.OneStar => OneStarMonsters,
            MonsterRarity.TwoStar => TwoStarMonsters,
            MonsterRarity.ThreeStar => ThreeStarMonsters,
            MonsterRarity.FourStar => FourStarMonsters,
            MonsterRarity.FiveStar => FiveStarMonsters,
            _ => OneStarMonsters
        };

        // SỬA LỖI RAND: Dùng Program.rand thay vì rand trực tiếp
        return pool[Program.rand.Next(pool.Length)];
    }

    // ══════════════════════════════════════════════════════════
    //  ⭐ 1 STAR
    // ══════════════════════════════════════════════════════════
    public static readonly MonsterStats[] OneStarMonsters =
    {
        new("Zombie",    78, 10, 18, 3,  7,  78,  "🧟", new("rotten_flesh",    "Rotten Flesh",    "🥩")),
        new("Skeleton",  72, 12, 20, 2,  6,  88,  "💀", new("bone_shard",      "Bone Shard",      "🦴")),
        new("Slime",     65,  8, 15, 6, 12,  68,  "🤮", new("slime_gel",       "Slime Gel",       "💧")),
        new("Wolf",      83, 13, 22, 3,  8,  93,  "🐺", new("wolf_fang",       "Wolf Fang",       "🦷")),
        new("Ghost",     60, 11, 19, 2,  5,  83,  "👻", new("ectoplasm",       "Ectoplasm",       "👻")),
        new("Sinner",    90, 14, 23, 4,  9,  86,  "😈", new("cursed_coin",     "Cursed Coin",     "🪙")),
        new("Corpse",    95, 15, 24, 5, 10,  95,  "🪦", new("grave_dust",      "Grave Dust",      "💨")),
    };

    // ══════════════════════════════════════════════════════════
    //  ⭐⭐ 2 STAR
    // ══════════════════════════════════════════════════════════
    public static readonly MonsterStats[] TwoStarMonsters =
    {
        new("Giant Bat",    102, 16, 25, 5,  9,  132, "🦇", new("bat_wing",        "Bat Wing",        "🦇")),
        new("Lizardman",    113, 17, 27, 7, 13,  152, "🦎", new("lizard_scale",    "Lizard Scale",    "🦎")),
        new("Nightmare",    107, 18, 29, 4,  8,  162, "🌑", new("dark_essence",    "Dark Essence",    "🌑")),
        new("Flesh",        119, 15, 24, 8, 14,  142, "✋", new("flesh_chunk",     "Flesh Chunk",     "✋")),
        new("Cursed Skull",  96, 19, 30, 3,  7,  157, "☠️", new("cursed_eye",      "Cursed Eye",      "👁️")),
        new("Crusader",     127, 21, 31, 6, 12,  167, "🛡️", new("iron_emblem",     "Iron Emblem",     "🛡️")),
        new("Imp",          132, 22, 33, 7, 14,  175, "😈", new("imp_horn",        "Imp Horn",        "😈")),
    };

    // ══════════════════════════════════════════════════════════
    //  ⭐⭐⭐ 3 STAR
    // ══════════════════════════════════════════════════════════
    public static readonly MonsterStats[] ThreeStarMonsters =
    {
        new("Skeleton Warrior", 145, 23, 34, 9,  16, 230, "⚔️", new("war_blade_shard",  "War Blade Shard",  "⚔️")),
        new("Werewolf",         156, 25, 38, 7,  14, 250, "🐺", new("silver_claw",      "Silver Claw",      "🌕")),
        new("Wyvern",           162, 27, 40, 8,  15, 270, "🐲", new("wyvern_talon",     "Wyvern Talon",     "🐲")),
        new("Man-Eater Ghoul",  150, 24, 35, 6,  12, 240, "💀", new("ghoul_tooth",      "Ghoul Tooth",      "🦷")),
        new("Phantom Duelist",  139, 26, 39, 5,  11, 260, "🗡️", new("phantom_blade",    "Phantom Blade",    "🗡️")),
        new("Holy Knight",      174, 29, 42, 10, 17, 265, "⚔️", new("holy_crest",       "Holy Crest",       "✝️")),
        new("Cerberus",         180, 30, 44, 11, 18, 285, "🐕", new("hellhound_collar", "Hellhound Collar", "🔗")),
    };

    // ══════════════════════════════════════════════════════════
    //  ⭐⭐⭐⭐ 4 STAR
    // ══════════════════════════════════════════════════════════
    public static readonly MonsterStats[] FourStarMonsters =
    {
        new("Vampire",        160, 26, 39, 8,  15, 320, "🧛", new("blood_vial",       "Blood Vial",       "🩸")),
        new("Demon",          168, 28, 41, 9,  16, 345, "😈", new("demon_heart",      "Demon Heart",      "❤️‍🔥")),
        new("Headless Knight",178, 25, 38, 12, 19, 335, "⚔️", new("hollow_helm",      "Hollow Helm",      "⛑️")),
        new("Medusa",         153, 29, 43, 7,  14, 325, "🐍", new("stone_gaze_shard", "Stone Gaze Shard", "💎")),
        new("Fallen Angel",   164, 31, 46, 6,  13, 360, "👼", new("broken_halo",      "Broken Halo",      "👼")),
        new("Devil Hunter",   200, 34, 50, 10, 18, 410, "🔥", new("hellfire_core",    "Hellfire Core",    "🔥")),
        new("Nightstalker",   210, 36, 53, 11, 20, 430, "🌑", new("shadow_fragment",  "Shadow Fragment",  "🌑")),
    };

    // ══════════════════════════════════════════════════════════
    //  ⭐⭐⭐⭐⭐ 5 STAR
    // ══════════════════════════════════════════════════════════
    public static readonly MonsterStats[] FiveStarMonsters =
    {
        new("Dragon",         235, 36, 54, 14, 22, 540, "🐉", new("dragon_scale",     "Dragon Scale",     "🐉")),
        new("Leviathan",      265, 34, 52, 12, 20, 580, "🌊", new("abyssal_pearl",    "Abyssal Pearl",    "🌊")),
        new("Hydra",          252, 31, 49, 16, 25, 555, "🐍", new("hydra_venom",      "Hydra Venom",      "☠️")),
        new("Death",          212, 41, 60, 10, 18, 620, "💀", new("death_scythe_tip", "Death Scythe Tip", "💀")),
        new("Seraph of Death",225, 39, 56, 13, 21, 660, "👼", new("fallen_feather",   "Fallen Feather",   "🪶")),
        new("Seraphim",       285, 46, 66, 16, 24, 720, "🌟", new("divine_core",      "Divine Core",      "🌟")),
        new("Lucifer",        300, 48, 70, 17, 26, 770, "😈", new("lucifer_sigil",    "Lucifer's Sigil",  "🔱")),
    };
}
