using System;
using System.Linq;
using System.Collections.Generic;

using static Rpg.Program;
using static Rpg.Title;
using static Rpg.Leaderboards;

namespace Rpg;


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
    // ========== SHOP PRICES ==========
    public const int POTION_COST = 150;
    public const int SWORD_COST = 500;
    public const int ARMOR_COST = 400;

    // ========== MONSTER DATA ==========
    public static (string name, int maxHP, int atkMin, int atkMax, int defMin, int defMax, int score, string emoji)
        GetMonsterStats()
    {
        string[] names = { "Zombie", "Slime", "Skeleton", "Vampire", "Werewolf", "Dragon" };
        return names[rand.Next(names.Length)] switch
        {
            "Zombie" => ("Zombie", 80, 10, 18, 3, 7, 100, "🧟"),
            "Slime" => ("Slime", 60, 8, 14, 5, 12, 80, "🤢"),
            "Skeleton" => ("Skeleton", 70, 14, 22, 2, 6, 120, "💀"),
            "Vampire" => ("Vampire", 90, 16, 26, 4, 9, 150, "🧛"),
            "Werewolf" => ("Werewolf", 110, 18, 28, 3, 8, 180, "🐺"),
            "Dragon" => ("Dragon", 150, 22, 35, 6, 14, 300, "🐉"),
            _ => ("Unknown", 70, 12, 20, 4, 9, 100, "👾"),
        };
    }


}
