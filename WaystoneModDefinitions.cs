using System;
using System.Collections.Generic;
using System.Linq;

namespace WaystoneCrafter
{
    public class ModDefinition
    {
        public string InternalName { get; set; }
        public string DisplayName { get; set; }
        public bool IsPrefix { get; set; }
        public string Description { get; set; }
        public string Variant { get; set; } // To distinguish between mods with same internal name
    }

    public static class WaystoneModDefinitions
    {
        private static readonly List<ModDefinition> ModList = new()
        {
            new ModDefinition
            {
                InternalName = "MapExperienceGainIncrease",
                DisplayName = "Elevated",
                IsPrefix = true,
                Description = "+% Experience gain"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterPackSizeIncrease",
                DisplayName = "Teeming",
                IsPrefix = true,
                Description = "+% Pack Size"
            },
            new ModDefinition
            {
                InternalName = "MapDroppedItemQuantityRarityIncrease",
                DisplayName = "Plundering",
                IsPrefix = true,
                Description = "(10–15)% increased Quantity of Items found in this Area\n(18–24)% increased Rarity of Items found in this Area"
            },
            new ModDefinition
            {
                InternalName = "MapDroppedItemRarityIncrease",
                DisplayName = "Collector's",
                IsPrefix = true,
                Description = "+% Rarity"
            },
            new ModDefinition
            {
                InternalName = "MapDroppedGoldIncrease",
                DisplayName = "Bountiful",
                IsPrefix = true,
                Description = "+% Gold"
            },
            new ModDefinition
            {
                InternalName = "MapPackSizeIncrease",
                DisplayName = "Breeding",
                IsPrefix = true,
                Description = "+% Pack Size"
            },
            new ModDefinition
            {
                InternalName = "MapTotalEffectivenessIncrease",
                DisplayName = "Populated",
                IsPrefix = true,
                Description = "+% Monster Packs"
            },
            new ModDefinition
            {
                InternalName = "MapMagicPackIncrease",
                DisplayName = "Teeming",
                IsPrefix = true,
                Description = "+% Magic Monsters"
            },
            new ModDefinition
            {
                InternalName = "MapRarePackIncrease",
                DisplayName = "Brimming",
                IsPrefix = true,
                Description = "+% Rare Monsters"
            },
            new ModDefinition
            {
                InternalName = "MapMagicRarePackIncrease",
                DisplayName = "Swarming",
                IsPrefix = true,
                Description = "x% increased Magic Monsters\nx% increased number of Rare Monsters"
            },
            new ModDefinition
            {
                InternalName = "MapMagicChestCountIncrease",
                DisplayName = "Archivist's",
                IsPrefix = true,
                Description = "+% Magic Chests"
            },
            new ModDefinition
            {
                InternalName = "MapRareChestCountIncrease",
                DisplayName = "Treasurer's",
                IsPrefix = true,
                Description = "+% Rare Chests"
            },
            new ModDefinition
            {
                InternalName = "MapMagicPackSizeIncrease",
                DisplayName = "Emboldening",
                IsPrefix = true,
                Description = "+% Magic Pack Size"
            },
            new ModDefinition
            {
                InternalName = "MapRareMonstersAdditionalModifier",
                DisplayName = "Nemesis",
                IsPrefix = true,
                Description = "Rare Monsters have +1 Modifier"
            },
            new ModDefinition
            {
                InternalName = "MapAdditionalShrine",
                DisplayName = "Devoted",
                IsPrefix = true,
                Description = "+1 Shrine"
            },
            new ModDefinition
            {
                InternalName = "MapAdditionalStrongbox",
                DisplayName = "Antiquarian's",
                IsPrefix = true,
                Description = "+1 Strongbox"
            },
            new ModDefinition
            {
                InternalName = "MapAdditionalEssence",
                DisplayName = "Crystallised",
                IsPrefix = true,
                Description = "+1 Essence"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterFireDamage",
                DisplayName = "of the Inferno",
                Description = "Monsters deal Extra Fire Damage\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonsterColdDamage",
                DisplayName = "of Frostbite",
                Description = "Monsters deal Extra Cold Damage\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonsterLightningDamage",
                DisplayName = "of Thunder",
                Description = "Monsters deal (26–30)% of Damage as Extra Lightning\n95% increased Waystones found in Area",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonsterDamage",
                DisplayName = "of Pain",
                Description = "+% Monster Damage\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonsterFast",
                DisplayName = "of Fleeting",
                Description = "+% Monster Speed (Movement, Attack, Cast)\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonsterCriticalStrikesAndDamage",
                DisplayName = "of Destruction",
                Description = "+% Monster Crit Chance & Damage\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonsterLife",
                DisplayName = "of Toughness",
                Description = "(30–39)% more Monster Life\n90% increased Waystones found in Area",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonstersAllResistances",
                DisplayName = "of the Prism",
                Description = "+% Monster Elemental Resistances\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonstersArmoured",
                DisplayName = "of Endurance",
                Description = "Monsters are Armoured\n70% increased Waystones found in Area",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonstersEvasive",
                DisplayName = "of the Blur",
                Description = "Monsters are Evasive\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonstersEnergyShield",
                DisplayName = "of Buffering",
                Description = "Monsters gain Extra Energy Shield\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapPoisoning",
                DisplayName = "of Venom",
                Description = "Monsters have 40% chance to Poison on Hit\n90% increased Waystones found in Area",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapBleeding",
                DisplayName = "of Puncturing",
                Description = "Monsters have 25% chance to Bleed on Hit\n90% increased Waystones found in Area",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonstersStunAndAilmentThreshold",
                DisplayName = "of the Unwavering",
                IsPrefix = false,
                Description = "+% Monster Ailment & Stun Threshold\n+% Waystones found"
            },
            new ModDefinition
            {
                InternalName = "MapMonstersArmourBreak",
                DisplayName = "of Shattering",
                Description = "Monsters Break Armour\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonstersAccuracy",
                DisplayName = "of Precision",
                Description = "+% Monster Accuracy\n75% increased Waystones found in Area",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonsterChaosDamage",
                DisplayName = "of Armageddon",
                Description = "Monsters deal (26–30)% of Damage as Extra Chaos\n75% increased Waystones found in Area",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonstersStunBuildup",
                DisplayName = "of Impact",
                Description = "Monsters have 100% increased Stun Buildup\n95% increased Waystones found in Area",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonstersAilmentChance",
                DisplayName = "of Overpowering",
                Description = "Monsters have 100% increased Freeze Buildup\nMonsters have 100% increased Ignite Chance\nMonsters have 100% increased Shock Chance\n100% increased Waystones found in Area",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonsterMultipleProjectiles",
                DisplayName = "of Splitting",
                Description = "Monsters fire Additional Projectiles\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAreaOfEffect",
                DisplayName = "of Giants",
                Description = "+% Monster Area of Effect\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapPlayerEnfeeblement",
                DisplayName = "of Enfeeblement",
                Description = "Players are Cursed with Enfeeble\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapPlayerTemporalChains",
                DisplayName = "of Temporal Chains",
                Description = "Players are Cursed with Temporal Chains\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapPlayerElementalWeakness",
                DisplayName = "of Elemental Weakness",
                Description = "Players are Cursed with Elemental Weakness\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonstersElementalPenetration",
                DisplayName = "of Penetration",
                Description = "Monster Damage Penetrates Resistances\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapPlayerMaxResists",
                DisplayName = "of Exposure",
                Description = "-% Maximum Player Resistances\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapPlayerReducedRegen",
                DisplayName = "of Smothering",
                Description = "Players have 40% less Recovery Rate of Life and Energy Shield\n125% increased Waystones found in Area",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapPlayersGainReducedFlaskCharges",
                DisplayName = "of Drought",
                Description = "Reduced Flask Charges\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapPlayerCooldownRecovery",
                DisplayName = "of Fatigue",
                Description = "Less Cooldown Recovery Rate\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonstersBaseSelfCriticalMultiplier",
                DisplayName = "of Obstruction",
                Description = "Monsters take Less Crit Damage\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonstersCurseEffectOnSelfFinal",
                DisplayName = "of the Hexwarded",
                Description = "75% less effect of Curses on Monsters\n70% increased Waystones found in Area",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapMonstersStealChargesOnHit",
                DisplayName = "of Enervation",
                Description = "Monsters Steal Charges\n+% Waystones found",
                IsPrefix = false
            },
            new ModDefinition
            {
                InternalName = "MapSpreadGroundEffect",
                DisplayName = "of Flames",
                Description = "Burning Ground\n+% Waystones found",
                IsPrefix = false,
                Variant = "Fire"
            },
            new ModDefinition
            {
                InternalName = "MapSpreadGroundEffect",
                DisplayName = "of Ice",
                Description = "Chilled Ground\n+% Waystones found",
                IsPrefix = false,
                Variant = "Cold"
            },
            new ModDefinition
            {
                InternalName = "MapSpreadGroundEffect",
                DisplayName = "of Electricity",
                Description = "Shocked Ground\n+% Waystones found",
                IsPrefix = false,
                Variant = "Lightning"
            },
            new ModDefinition
            {
                InternalName = "MapChestCountIncrease",
                DisplayName = "Salvager's",
                IsPrefix = true,
                Description = "x% increased Chests found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterStunAilmentThreshold",
                DisplayName = "of the Unwavering",
                IsPrefix = false,
                Description = "Monsters have x% increased Stun and Ailment Threshold\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterStunBuildup",
                DisplayName = "of Impact",
                IsPrefix = false,
                Description = "Monsters have x% increased Stun Effect Duration\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterArmoured",
                DisplayName = "of Endurance",
                IsPrefix = false,
                Description = "Monsters are Armoured\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAccuracy",
                DisplayName = "of Precision",
                IsPrefix = false,
                Description = "Monsters have x% increased Accuracy Rating\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksBaron",
                DisplayName = "Baron's",
                IsPrefix = true,
                Description = "Area contains an additional pack of Barons"
            },
            // Additional pack mods with their specific internal names
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksHallowed",
                DisplayName = "Hallowed",
                IsPrefix = true,
                Description = "Area contains (8–12) additional packs of Undead"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksBeastly",
                DisplayName = "Beastly",
                IsPrefix = true,
                Description = "Area contains (8–12) additional packs of Beasts"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksRusted",
                DisplayName = "Rusted",
                IsPrefix = true,
                Description = "Area contains (8–12) additional packs of Ezomyte Monsters"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksFaridun",
                DisplayName = "Faridun's",
                IsPrefix = true,
                Description = "Area contains (8–12) additional packs of Faridun Monsters"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksSacrificial",
                DisplayName = "Sacrificial",
                IsPrefix = true,
                Description = "Area contains (8–12) additional packs of Vaal Monsters"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksPerennial",
                DisplayName = "Perennial's",
                IsPrefix = true,
                Description = "Area contains (8–12) additional packs of Plagued Monsters"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksDoryani",
                DisplayName = "Doryani's",
                IsPrefix = true,
                Description = "Area contains (8–12) additional packs of Transcended Monsters"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksBrambled",
                DisplayName = "Brambled",
                IsPrefix = true,
                Description = "Area contains (8–12) additional packs of Bramble Monsters"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksVaal",
                DisplayName = "Sacrificial",
                IsPrefix = true,
                Description = "Area contains (8–12) additional packs of Vaal Monsters"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterArmourBreak",
                DisplayName = "of Shattering",
                IsPrefix = false,
                Description = "Monsters have x% increased Armour Break\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterPoisoning",
                DisplayName = "of Venom",
                IsPrefix = false,
                Description = "Monsters have x% chance to Poison on Hit\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterElementAilmentChance",
                DisplayName = "of Overpowering",
                IsPrefix = false,
                Description = "Monsters have x% increased Elemental Ailment Effect\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterDamageAsLightning",
                DisplayName = "of Thunder",
                IsPrefix = false,
                Description = "Monsters deal x% of Physical Damage as extra Lightning Damage\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterLifeIncrease",
                DisplayName = "of Toughness",
                IsPrefix = false,
                Description = "Monsters have x% increased Life\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalProjectiles",
                DisplayName = "of Splitting",
                IsPrefix = false,
                Description = "Monsters fire x additional Projectiles\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterDamageAsChaos",
                DisplayName = "of Armageddon",
                IsPrefix = false,
                Description = "Monsters deal x% of Physical Damage as extra Chaos Damage\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterEnergyShield",
                DisplayName = "of Buffering",
                IsPrefix = false,
                Description = "Monsters have x% increased Energy Shield\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapDroppedItemQuantityIncrease",
                DisplayName = "Plundering",
                IsPrefix = true,
                Description = "x% increased Quantity of Items found in this Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterBleeding",
                DisplayName = "of Puncturing",
                IsPrefix = false,
                Description = "Monsters have x% chance to cause Bleeding on Hit\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksEzomyte",
                DisplayName = "Rusted",
                IsPrefix = true,
                Description = "Area contains additional packs of Iron Guards"
            },
            new ModDefinition
            {
                InternalName = "MapMonstersCurseEffectOnSelf",
                DisplayName = "of the Hexwarded",
                IsPrefix = false,
                Description = "x% reduced Effect of Curses on Monsters\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapPlayerRecoveryRate",
                DisplayName = "of Smothering",
                IsPrefix = false,
                Description = "x% reduced Recovery Rate of Life and Energy Shield\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapSpreadBurningGround",
                DisplayName = "of Flames",
                IsPrefix = false,
                Description = "Areas contain Burning Ground\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapPlayerFlaskChargeGain",
                DisplayName = "of Drought",
                IsPrefix = false,
                Description = "x% reduced Flask Charges gained\nx% increased Waystones found in Area"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterDamageIncrease",
                DisplayName = "of Pain",
                IsPrefix = false,
                Description = "+% Monster Damage\n+% Waystones found"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterDamageAsCold",
                DisplayName = "of Frostbite",
                IsPrefix = false,
                Description = "Monsters deal Extra Cold Damage\n+% Waystones found"
            },
            new ModDefinition
            {
                InternalName = "MapPlayerMaximumResists",
                DisplayName = "of Exposure",
                IsPrefix = false,
                Description = "-% Maximum Player Resistances\n+% Waystones found"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterElementalResistances",
                DisplayName = "of the Prism",
                IsPrefix = false,
                Description = "+% Monster Elemental Resistances\n+% Waystones found"
            },
            new ModDefinition
            {
                InternalName = "MapSpreadShockedGround",
                DisplayName = "of Electricity",
                IsPrefix = false,
                Description = "Shocked Ground\n+% Waystones found"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksUndead",
                DisplayName = "Hallowed",
                IsPrefix = true,
                Description = "Area contains additional packs of undead monsters"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPacksBeasts",
                DisplayName = "Beastly",
                IsPrefix = true,
                Description = "Area contains additional packs of beast monsters"
            },
            new ModDefinition
            {
                InternalName = "MapMonstersStealCharges",
                DisplayName = "of Enervation",
                IsPrefix = false,
                Description = "Monsters steal charges on hit"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterSpeedIncrease",
                DisplayName = "of Fleeting",
                IsPrefix = false,
                Description = "Monsters have increased movement speed"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterGiantSize",
                DisplayName = "of Giants",
                IsPrefix = false,
                Description = "Monsters have increased size and area of effect"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterThorns",
                DisplayName = "Brambled",
                IsPrefix = true,
                Description = "Monsters reflect damage"
            },
            new ModDefinition
            {
                InternalName = "MapPlayerEnfeeble",
                DisplayName = "of Enfeeblement",
                IsPrefix = false,
                Description = "Players are cursed with Enfeeble"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterCriticalStrikes",
                DisplayName = "of Destruction",
                IsPrefix = false,
                Description = "Monsters have increased critical strike chance"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterEvasion",
                DisplayName = "of the Blur",
                IsPrefix = false,
                Description = "Monsters have increased evasion"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterFireDamage",
                DisplayName = "of the Inferno",
                IsPrefix = false,
                Description = "Monsters deal additional fire damage"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterIncreasedAreaOfEffect",
                DisplayName = "of Giants",
                IsPrefix = false,
                Description = "Monsters have increased area of effect"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterAdditionalPackBramble",
                DisplayName = "Brambled",
                IsPrefix = true,
                Description = "Area contains additional packs of bramble monsters"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterEvasive",
                DisplayName = "of the Blur",
                IsPrefix = false,
                Description = "Monsters are evasive"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterDamageAsFire",
                DisplayName = "of the Inferno",
                IsPrefix = false,
                Description = "Monsters deal additional fire damage"
            },
            new ModDefinition
            {
                InternalName = "MapMonsterCritIncrease",
                DisplayName = "of Destruction",
                IsPrefix = false,
                Description = "Monsters have increased critical strike chance"
            },
        };

        public static ModDefinition GetModDefinition(string internalName, string displayName = null)
        {
            var candidates = ModList.Where(m => m.InternalName == internalName);
            if (!string.IsNullOrEmpty(displayName))
            {
                // If we have a display name, use it to find the exact variant
                return candidates.FirstOrDefault(m => m.DisplayName == displayName);
            }
            return candidates.FirstOrDefault();
        }

        public static IEnumerable<ModDefinition> GetAllModDefinitions()
        {
            return ModList;
        }

        public static bool IsPrefix(string internalName)
        {
            var def = GetModDefinition(internalName);
            return def?.IsPrefix ?? false;
        }

        public static string GetDisplayName(string internalName)
        {
            return GetModDefinition(internalName)?.DisplayName ?? internalName;
        }

        public static string GetDescription(string internalName)
        {
            return GetModDefinition(internalName)?.Description ?? internalName;
        }
    }
}
