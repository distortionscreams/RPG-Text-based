using System;
using System.Linq;
using System.Collections.Generic;

using static RPG_Text_base.Program;
using static RPG_Text_base.Title;

namespace RPG_Text_base;

public static class Stats
{
    // ========== SESSION STATE ==========
    public static int totalScore = 0;
    public static int monstersKilled = 0;
    public static int totalTurns = 0;

    // ========== PERSISTENT UPGRADES ==========
    public static int playerAtkBonus = 0;
    public static int playerMaxHP = 100;
    public static int extraPotions = 0;

    // ========== MONSTER DATA ==========

    public enum MonsterRarity
    {
        OneStar = 1,
        TwoStar = 2,
        ThreeStar = 3,
        FourStar = 4,
        FiveStar = 5
    }

    // Chỉ giữ các field mà battle thực sự dùng
    public record MonsterStats(
        string Name,
        int MaxHP,
        int AtkMin,
        int AtkMax,
        int DefMin,
        int DefMax,
        int Score,
        string Emoji
    );

    // Battle gọi GetMonsterStats(currentTier) — chọn ngẫu nhiên từ pool theo rarity
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
        return pool[rand.Next(pool.Length)];
    }

    // ── ⭐ 1 STAR ──────────────────────────────────────────────
    public static readonly MonsterStats[] OneStarMonsters =
   {
    new("Zombie",   70,  9,  16, 2, 6,   60,  "🧟"),
    new("Skeleton", 64,  11, 18, 1, 5,   70,  "💀"),
    new("Slime",    58,  7,  13, 5, 10,  50,  "🤮"),
    new("Wolf",     75,  12, 20, 2, 7,   75,  "🐺"),
    new("Ghost",    52,  10, 17, 1, 4,   65,  "👻"),
   };

    // ── ⭐⭐ 2 STAR ─────────────────────────────────────────────
    public static readonly MonsterStats[] TwoStarMonsters =
   {
    new("Giant Bat",    93,  15, 23, 4, 8,   110, "🦇"),
    new("Lizardman",    104, 16, 25, 6, 12,  130, "🦎"),
    new("Nightmare",    98,  17, 27, 3, 7,   140, "🌑"),
    new("Flesh",        110, 14, 22, 7, 13,  120, "✋"),
    new("Cursed Skull", 87,  18, 28, 2, 6,   135, "☠️"),
   };

    // ── ⭐⭐⭐ 3 STAR ────────────────────────────────────────────
    public static readonly MonsterStats[] ThreeStarMonsters =
   {
    new("Skeleton Warrior", 133, 21, 31, 8,  15, 200, "⚔️"),
    new("Werewolf",         144, 23, 35, 6,  13, 220, "🐺"),
    new("Wyvern",           150, 25, 37, 7,  14, 240, "🐲"),
    new("Man-Eater Ghoul",  138, 22, 32, 5,  11, 210, "💀"),
    new("Phantom Duelist",  127, 24, 36, 4,  10, 230, "🗡️"),
   };

    // ── ⭐⭐⭐⭐ 4 STAR ───────────────────────────────────────────
    public static readonly MonsterStats[] FourStarMonsters =
   {
    new("Vampire",         175, 29, 44, 9,  17, 350, "🧛"),
    new("Demon",           185, 31, 46, 10, 18, 380, "😈"),
    new("Headless Knight", 196, 28, 43, 14, 22, 370, "⚔️"),
    new("Medusa",          168, 32, 48, 8,  16, 360, "🐍"),
    new("Fallen Angel",    180, 35, 51, 7,  15, 400, "👼"),
   };

    // ── ⭐⭐⭐⭐⭐ 5 STAR ──────────────────────────────────────────
    public static readonly MonsterStats[] FiveStarMonsters =
   {
    new("Dragon",          260, 40, 60, 16, 25, 600, "🐉"),
    new("Leviathan",       295, 38, 58, 14, 23, 650, "🌊"),
    new("Hydra",           280, 35, 55, 18, 28, 620, "🐍"),
    new("Death",           235, 46, 67, 12, 21, 700, "💀"),
    new("Seraph of Death", 248, 44, 63, 15, 24, 750, "👼"),
   };
}
