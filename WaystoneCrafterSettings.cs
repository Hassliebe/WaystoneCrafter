using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;
using ExileCore2;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.PoEMemory.Elements;
using ExileCore2.PoEMemory.Models;
using ExileCore2.Shared;
using ExileCore2.Shared.Attributes;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;
using ImGuiNET;
using System.Numerics;

namespace WaystoneCrafter
{
    public class ModSetting
    {
        private ToggleNode _isGoodMod;
        private ToggleNode _isBannedMod;

        public ToggleNode IsGoodMod 
        { 
            get => _isGoodMod;
            set
            {
                _isGoodMod = value;
                if (_isGoodMod != null)
                {
                    _isGoodMod.OnValueChanged += (sender, val) =>
                    {
                        if (val && _isBannedMod != null)
                            _isBannedMod.Value = false;
                    };
                }
            }
        }

        public ToggleNode IsBannedMod 
        { 
            get => _isBannedMod;
            set
            {
                _isBannedMod = value;
                if (_isBannedMod != null)
                {
                    _isBannedMod.OnValueChanged += (sender, val) =>
                    {
                        if (val && _isGoodMod != null)
                            _isGoodMod.Value = false;
                    };
                }
            }
        }
    }

    public class WaystoneCrafterSettings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(false);

        [Menu("Debug", 0)]
        public ToggleNode Debug { get; set; } = new ToggleNode(false);

        [Menu("General", 1)]
        public EmptyNode GeneralHeader { get; set; } = new EmptyNode();

        [Menu("Fill Suffixes", "Continue crafting until map has 3 suffixes after prefixes are full", 2, 1)]
        public ToggleNode AlwaysFillAffixes { get; set; } = new ToggleNode(false);

        [Menu("Max Waystones to Craft", "Maximum number of waystones to craft at once (capped at 10 to prevent inventory issues)", 3, 1)]
        public RangeNode<int> MaxWaystonesToCraft { get; set; } = new RangeNode<int>(10, 1, 10);

        [Menu("Hotkeys", 5)]
        public EmptyNode HotkeysHeader { get; set; } = new EmptyNode();

        [Menu("Start Crafting Hotkey", 6, 5)]
        public HotkeyNode StartKey { get; set; } = new HotkeyNode(Keys.F6);

        [Menu("Stop Crafting Hotkey", 7, 5)]
        public HotkeyNode StopKey { get; set; } = new HotkeyNode(Keys.F7);

        [Menu("Stash Settings", 10)]
        public EmptyNode StashSettings { get; set; } = new EmptyNode();

        [Menu("Input Tab", "Tab containing waystones to craft", 11, 10)]
        public ListNode InputStashTab { get; set; } = new ListNode();

        [Menu("Currency Tab", "Tab containing currency", 12, 10)]
        public ListNode CurrencyStashTab { get; set; } = new ListNode();

        [Menu("Good Maps Tab", "Tab for maps with good mods", 13, 10)]
        public ListNode OutputGoodStashTab { get; set; } = new ListNode();

        [Menu("Bad Maps Tab", "Tab for maps with banned mods", 14, 10)]
        public ListNode OutputBadStashTab { get; set; } = new ListNode();

        [Menu("Rest Maps Tab", "Tab for maps that don't match criteria", 15, 10)]
        public ListNode OutputRestStashTab { get; set; } = new ListNode();

        [Menu("Delays", 20)]
        public EmptyNode DelaysHeader { get; set; } = new EmptyNode();

        [Menu("Tab Switch Delay (ms)", "Delay between switching stash tabs", 21, 20)]
        public RangeNode<int> TabSwitchDelay { get; set; } = new RangeNode<int>(100, 50, 500);

        [Menu("Action Delay (ms)", "Delay between actions like clicking items", 22, 20)]
        public RangeNode<int> ActionDelay { get; set; } = new RangeNode<int>(100, 50, 500);

        [Menu("Hover Item Delay (ms)", "Delay between hovering over items", 23, 20)]
        public RangeNode<int> HoverItemDelay { get; set; } = new RangeNode<int>(50, 10, 500);

        [Menu("Stash Item Delay (ms)", "Delay between stashing items", 24, 20)]
        public RangeNode<int> StashItemDelay { get; set; } = new RangeNode<int>(100, 10, 500);

        [Menu("Crafting Settings", 30)]
        public EmptyNode CraftingSettings { get; set; } = new EmptyNode();

        [Menu("Max Exalts", "Maximum exalts to use per item", 32, 30)]
        public RangeNode<int> MaxExaltsPerItem { get; set; } = new RangeNode<int>(3, 1, 10);

        [Menu("Mod Settings", 50)]
        public EmptyNode ModSettingsHeader { get; set; } = new EmptyNode();

        [Menu("Prefixes", 51, 50)]
        public EmptyNode PrefixHeader { get; set; } = new EmptyNode();

        [Menu("Elevated (+% Experience)", 52, 51)]
        public EmptyNode ElevatedHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 53, 52)]
        public ToggleNode ElevatedGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 54, 52)]
        public ToggleNode ElevatedBanned { get; set; } = new ToggleNode(false);

        [Menu("Teeming (+% Pack Size)", 55, 51)]
        public EmptyNode TeemingHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 56, 55)]
        public ToggleNode TeemingGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 57, 55)]
        public ToggleNode TeemingBanned { get; set; } = new ToggleNode(false);

        [Menu("Plundering (+% Quantity & Rarity)", 58, 51)]
        public EmptyNode PlunderingHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 59, 58)]
        public ToggleNode PlunderingGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 60, 58)]
        public ToggleNode PlunderingBanned { get; set; } = new ToggleNode(false);

        [Menu("Collector's (+% Rarity)", 61, 51)]
        public EmptyNode CollectorsHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 62, 61)]
        public ToggleNode CollectorsGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 63, 61)]
        public ToggleNode CollectorsBanned { get; set; } = new ToggleNode(false);

        [Menu("Bountiful (+% Gold)", 64, 51)]
        public EmptyNode BountifulHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 65, 64)]
        public ToggleNode BountifulGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 66, 64)]
        public ToggleNode BountifulBanned { get; set; } = new ToggleNode(false);

        [Menu("Breeding (+% Pack Size)", 67, 51)]
        public EmptyNode BreedingHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 68, 67)]
        public ToggleNode BreedingGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 69, 67)]
        public ToggleNode BreedingBanned { get; set; } = new ToggleNode(false);

        [Menu("Populated (+% Monster Packs)", 70, 51)]
        public EmptyNode PopulatedHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 71, 70)]
        public ToggleNode PopulatedGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 72, 70)]
        public ToggleNode PopulatedBanned { get; set; } = new ToggleNode(false);

        [Menu("Magic Monsters (+% Magic Monsters)", 73, 51)]
        public EmptyNode MagicPackHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 74, 73)]
        public ToggleNode MagicPackGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 75, 73)]
        public ToggleNode MagicPackBanned { get; set; } = new ToggleNode(false);

        [Menu("Brimming (+% Rare Monsters)", 76, 51)]
        public EmptyNode BrimmingHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 77, 76)]
        public ToggleNode BrimmingGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 78, 76)]
        public ToggleNode BrimmingBanned { get; set; } = new ToggleNode(false);

        [Menu("Swarming (+% Magic & Rare Monsters)", 79, 51)]
        public EmptyNode SwarmingHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 80, 79)]
        public ToggleNode SwarmingGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 81, 79)]
        public ToggleNode SwarmingBanned { get; set; } = new ToggleNode(false);

        [Menu("Salvager's (+% Chests)", 82, 51)]
        public EmptyNode SalvagerHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 83, 82)]
        public ToggleNode SalvagerGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 84, 82)]
        public ToggleNode SalvagerBanned { get; set; } = new ToggleNode(false);

        [Menu("Baron's (Additional Baron Packs)", 85, 51)]
        public EmptyNode BaronHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 86, 85)]
        public ToggleNode BaronGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 87, 85)]
        public ToggleNode BaronBanned { get; set; } = new ToggleNode(false);

        [Menu("Archivist's (+1 Strongbox)", 88, 51)]
        public EmptyNode ArchivistsHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 89, 88)]
        public ToggleNode ArchivistsGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 90, 88)]
        public ToggleNode ArchivistsBanned { get; set; } = new ToggleNode(false);

        [Menu("Treasurer's (+% Rare Chests)", 91, 51)]
        public EmptyNode TreasurersHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 92, 91)]
        public ToggleNode TreasurersGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 93, 91)]
        public ToggleNode TreasurersBanned { get; set; } = new ToggleNode(false);

        [Menu("Emboldening (+% Magic Pack Size)", 94, 51)]
        public EmptyNode EmboldeningHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 95, 94)]
        public ToggleNode EmboldeningGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 96, 94)]
        public ToggleNode EmboldeningBanned { get; set; } = new ToggleNode(false);

        [Menu("Additional Monster Packs", 97, 51)]
        public EmptyNode AdditionalPacksHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 98, 97)]
        public ToggleNode AdditionalPacksGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 99, 97)]
        public ToggleNode AdditionalPacksBanned { get; set; } = new ToggleNode(false);

        [Menu("Nemesis (Rare +1 Modifier)", 100, 51)]
        public EmptyNode NemesisHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 101, 100)]
        public ToggleNode NemesisGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 102, 100)]
        public ToggleNode NemesisBanned { get; set; } = new ToggleNode(false);

        [Menu("Devoted (+1 Shrine)", 103, 51)]
        public EmptyNode DevotedHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 104, 103)]
        public ToggleNode DevotedGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 105, 103)]
        public ToggleNode DevotedBanned { get; set; } = new ToggleNode(false);

        [Menu("Antiquarian's (+1 Essence)", 106, 51)]
        public EmptyNode AntiquariansHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 107, 106)]
        public ToggleNode AntiquariansGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 108, 106)]
        public ToggleNode AntiquariansBanned { get; set; } = new ToggleNode(false);

        [Menu("Crystallised (+1 Essence)", 109, 51)]
        public EmptyNode CrystallisedHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 110, 109)]
        public ToggleNode CrystallisedGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 111, 109)]
        public ToggleNode CrystallisedBanned { get; set; } = new ToggleNode(false);

        [Menu("Suffixes", 200, 50)]
        public EmptyNode SuffixHeader { get; set; } = new EmptyNode();

        [Menu("of the Inferno (Extra Fire Damage)", 201, 200)]
        public EmptyNode InfernoHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 202, 201)]
        public ToggleNode InfernoGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 203, 201)]
        public ToggleNode InfernoBanned { get; set; } = new ToggleNode(false);

        [Menu("of Frostbite (Extra Cold Damage)", 204, 200)]
        public EmptyNode FrostbiteHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 205, 204)]
        public ToggleNode FrostbiteGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 206, 204)]
        public ToggleNode FrostbiteBanned { get; set; } = new ToggleNode(false);

        [Menu("of Thunder (Extra Lightning Damage)", 207, 200)]
        public EmptyNode ThunderHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 208, 207)]
        public ToggleNode ThunderGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 209, 207)]
        public ToggleNode ThunderBanned { get; set; } = new ToggleNode(false);

        [Menu("of Pain (Monster Damage)", 210, 200)]
        public EmptyNode PainHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 211, 210)]
        public ToggleNode PainGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 212, 210)]
        public ToggleNode PainBanned { get; set; } = new ToggleNode(false);

        [Menu("of Fleeting (Monster Speed)", 213, 200)]
        public EmptyNode FleetingHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 214, 213)]
        public ToggleNode FleetingGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 215, 213)]
        public ToggleNode FleetingBanned { get; set; } = new ToggleNode(false);

        [Menu("of Destruction (Monster Crits)", 216, 200)]
        public EmptyNode DestructionHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 217, 216)]
        public ToggleNode DestructionGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 218, 216)]
        public ToggleNode DestructionBanned { get; set; } = new ToggleNode(false);

        [Menu("of Toughness (Monster Life)", 219, 200)]
        public EmptyNode ToughnessHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 220, 219)]
        public ToggleNode ToughnessGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 221, 219)]
        public ToggleNode ToughnessBanned { get; set; } = new ToggleNode(false);

        [Menu("of the Prism (Monster Resistances)", 222, 200)]
        public EmptyNode PrismHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 223, 222)]
        public ToggleNode PrismGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 224, 222)]
        public ToggleNode PrismBanned { get; set; } = new ToggleNode(false);

        [Menu("of Endurance (Armoured)", 225, 200)]
        public EmptyNode EnduranceHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 226, 225)]
        public ToggleNode EnduranceGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 227, 225)]
        public ToggleNode EnduranceBanned { get; set; } = new ToggleNode(false);

        [Menu("of the Blur (Evasive)", 228, 200)]
        public EmptyNode BlurHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 229, 228)]
        public ToggleNode BlurGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 230, 228)]
        public ToggleNode BlurBanned { get; set; } = new ToggleNode(false);

        [Menu("of Buffering (Energy Shield)", 231, 200)]
        public EmptyNode BufferingHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 232, 231)]
        public ToggleNode BufferingGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 233, 231)]
        public ToggleNode BufferingBanned { get; set; } = new ToggleNode(false);

        [Menu("of Venom (Poison)", 234, 200)]
        public EmptyNode VenomHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 235, 234)]
        public ToggleNode VenomGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 236, 234)]
        public ToggleNode VenomBanned { get; set; } = new ToggleNode(false);

        [Menu("of Puncturing (Bleed)", 237, 200)]
        public EmptyNode PuncturingHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 238, 237)]
        public ToggleNode PuncturingGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 239, 237)]
        public ToggleNode PuncturingBanned { get; set; } = new ToggleNode(false);

        [Menu("of the Unwavering (Ailment & Stun)", 240, 200)]
        public EmptyNode UnwaveringHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 241, 240)]
        public ToggleNode UnwaveringGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 242, 240)]
        public ToggleNode UnwaveringBanned { get; set; } = new ToggleNode(false);

        [Menu("of Shattering (Armour Break)", 243, 200)]
        public EmptyNode ShatteringHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 244, 243)]
        public ToggleNode ShatteringGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 245, 243)]
        public ToggleNode ShatteringBanned { get; set; } = new ToggleNode(false);

        [Menu("of Precision (Accuracy)", 246, 200)]
        public EmptyNode PrecisionHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 247, 246)]
        public ToggleNode PrecisionGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 248, 246)]
        public ToggleNode PrecisionBanned { get; set; } = new ToggleNode(false);

        [Menu("of Armageddon (Chaos Damage)", 249, 200)]
        public EmptyNode ArmageddonHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 250, 249)]
        public ToggleNode ArmageddonGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 251, 249)]
        public ToggleNode ArmageddonBanned { get; set; } = new ToggleNode(false);

        [Menu("of Impact (Stun)", 252, 200)]
        public EmptyNode ImpactHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 253, 252)]
        public ToggleNode ImpactGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 254, 252)]
        public ToggleNode ImpactBanned { get; set; } = new ToggleNode(false);

        [Menu("of Overpowering (Ailments)", 255, 200)]
        public EmptyNode OverpoweringHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 256, 255)]
        public ToggleNode OverpoweringGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 257, 255)]
        public ToggleNode OverpoweringBanned { get; set; } = new ToggleNode(false);

        [Menu("of Splitting (Projectiles)", 258, 200)]
        public EmptyNode SplittingHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 259, 258)]
        public ToggleNode SplittingGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 260, 258)]
        public ToggleNode SplittingBanned { get; set; } = new ToggleNode(false);

        [Menu("of Giants (Area)", 261, 200)]
        public EmptyNode GiantsHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 262, 261)]
        public ToggleNode GiantsGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 263, 261)]
        public ToggleNode GiantsBanned { get; set; } = new ToggleNode(false);

        [Menu("of Enfeeblement (Curse)", 264, 200)]
        public EmptyNode EnfeeblementHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 265, 264)]
        public ToggleNode EnfeeblementGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 266, 264)]
        public ToggleNode EnfeeblementBanned { get; set; } = new ToggleNode(false);

        [Menu("of Temporal Chains (Curse)", 267, 200)]
        public EmptyNode TemporalChainsHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 268, 267)]
        public ToggleNode TemporalChainsGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 269, 267)]
        public ToggleNode TemporalChainsBanned { get; set; } = new ToggleNode(false);

        [Menu("of Elemental Weakness (Curse)", 270, 200)]
        public EmptyNode ElementalWeaknessHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 271, 270)]
        public ToggleNode ElementalWeaknessGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 272, 270)]
        public ToggleNode ElementalWeaknessBanned { get; set; } = new ToggleNode(false);

        [Menu("of Penetration (Resistances)", 273, 200)]
        public EmptyNode PenetrationHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 274, 273)]
        public ToggleNode PenetrationGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 275, 273)]
        public ToggleNode PenetrationBanned { get; set; } = new ToggleNode(false);

        [Menu("of Exposure (Max Res)", 276, 200)]
        public EmptyNode ExposureHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 277, 276)]
        public ToggleNode ExposureGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 278, 276)]
        public ToggleNode ExposureBanned { get; set; } = new ToggleNode(false);

        [Menu("of Smothering (Recovery)", 279, 200)]
        public EmptyNode SmotheringHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 280, 279)]
        public ToggleNode SmotheringGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 281, 279)]
        public ToggleNode SmotheringBanned { get; set; } = new ToggleNode(false);

        [Menu("of Drought (Flask Charges)", 282, 200)]
        public EmptyNode DroughtHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 283, 282)]
        public ToggleNode DroughtGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 284, 282)]
        public ToggleNode DroughtBanned { get; set; } = new ToggleNode(false);

        [Menu("of Fatigue (Cooldown)", 285, 200)]
        public EmptyNode FatigueHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 286, 285)]
        public ToggleNode FatigueGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 287, 285)]
        public ToggleNode FatigueBanned { get; set; } = new ToggleNode(false);

        [Menu("of Obstruction (Monster Crit Defense)", 288, 200)]
        public EmptyNode ObstructionHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 289, 288)]
        public ToggleNode ObstructionGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 290, 288)]
        public ToggleNode ObstructionBanned { get; set; } = new ToggleNode(false);

        [Menu("of the Hexwarded (Curse Effect)", 291, 200)]
        public EmptyNode HexwardedHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 292, 291)]
        public ToggleNode HexwardedGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 293, 291)]
        public ToggleNode HexwardedBanned { get; set; } = new ToggleNode(false);

        [Menu("of Enervation (Charge Steal)", 294, 200)]
        public EmptyNode EnervationHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 295, 294)]
        public ToggleNode EnervationGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 296, 294)]
        public ToggleNode EnervationBanned { get; set; } = new ToggleNode(false);

        [Menu("Ice (Chilled Ground)", 300, 200)]
        public EmptyNode ChilledGroundHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 301, 300)]
        public ToggleNode ChilledGroundGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 302, 300)]
        public ToggleNode ChilledGroundBanned { get; set; } = new ToggleNode(false);

        [Menu("Ground Effects", 303, 200)]
        public EmptyNode GroundEffectsHeader { get; set; } = new EmptyNode();

        [Menu("Good", "", 304, 303)]
        public ToggleNode GroundEffectsGood { get; set; } = new ToggleNode(false);

        [Menu("Banned", "", 305, 303)]
        public ToggleNode GroundEffectsBanned { get; set; } = new ToggleNode(false);

        private Dictionary<string, ModSetting> _modSettings = new Dictionary<string, ModSetting>();
        public Dictionary<string, ModSetting> ModSettings => _modSettings;

        private List<string> _allStashNames = new List<string>();
        public List<string> AllStashNames
        {
            get => _allStashNames;
            set => _allStashNames = value;
        }

        public WaystoneCrafterSettings()
        {
            // Initialize mod settings
            ModSettings["MapExperienceGainIncrease"] = new ModSetting 
            { 
                IsGoodMod = ElevatedGood,
                IsBannedMod = ElevatedBanned
            };
            ModSettings["MapMonsterPackSizeIncrease"] = new ModSetting 
            { 
                IsGoodMod = TeemingGood,
                IsBannedMod = TeemingBanned
            };
            ModSettings["MapDroppedItemQuantityRarityIncrease"] = new ModSetting 
            { 
                IsGoodMod = PlunderingGood,
                IsBannedMod = PlunderingBanned
            };
            ModSettings["MapDroppedItemRarityIncrease"] = new ModSetting 
            { 
                IsGoodMod = CollectorsGood,
                IsBannedMod = CollectorsBanned
            };
            ModSettings["MapDroppedGoldIncrease"] = new ModSetting 
            { 
                IsGoodMod = BountifulGood,
                IsBannedMod = BountifulBanned
            };
            ModSettings["MapPackSizeIncrease"] = new ModSetting 
            { 
                IsGoodMod = BreedingGood,
                IsBannedMod = BreedingBanned
            };
            ModSettings["MapTotalEffectivenessIncrease"] = new ModSetting 
            { 
                IsGoodMod = PopulatedGood,
                IsBannedMod = PopulatedBanned
            };
            ModSettings["MapMagicPackIncrease"] = new ModSetting 
            { 
                IsGoodMod = MagicPackGood,
                IsBannedMod = MagicPackBanned
            };
            ModSettings["MapRarePackIncrease"] = new ModSetting 
            { 
                IsGoodMod = BrimmingGood,
                IsBannedMod = BrimmingBanned
            };
            ModSettings["MapMagicRarePackIncrease"] = new ModSetting 
            { 
                IsGoodMod = SwarmingGood,
                IsBannedMod = SwarmingBanned
            };
            ModSettings["MapRareMagicPackIncrease"] = new ModSetting 
            { 
                IsGoodMod = SwarmingGood,
                IsBannedMod = SwarmingBanned
            };
            ModSettings["MapMagicChestCountIncrease"] = new ModSetting 
            { 
                IsGoodMod = ArchivistsGood,
                IsBannedMod = ArchivistsBanned
            };
            ModSettings["MapRareChestCountIncrease"] = new ModSetting 
            { 
                IsGoodMod = TreasurersGood,
                IsBannedMod = TreasurersBanned
            };
            ModSettings["MapChestCountIncrease"] = new ModSetting 
            { 
                IsGoodMod = SalvagerGood,
                IsBannedMod = SalvagerBanned
            };
            ModSettings["MapMagicPackSizeIncrease"] = new ModSetting 
            { 
                IsGoodMod = EmboldeningGood,
                IsBannedMod = EmboldeningBanned
            };
            ModSettings["MapMonsterAdditionalPacks"] = new ModSetting 
            { 
                IsGoodMod = AdditionalPacksGood,
                IsBannedMod = AdditionalPacksBanned
            };
            ModSettings["MapMonsterAdditionalPacksBaron"] = new ModSetting 
            { 
                IsGoodMod = BaronGood,
                IsBannedMod = BaronBanned
            };
            ModSettings["MapRareMonstersAdditionalModifier"] = new ModSetting 
            { 
                IsGoodMod = NemesisGood,
                IsBannedMod = NemesisBanned
            };
            ModSettings["MapAdditionalShrine"] = new ModSetting 
            { 
                IsGoodMod = DevotedGood,
                IsBannedMod = DevotedBanned
            };
            ModSettings["MapAdditionalStrongbox"] = new ModSetting 
            { 
                IsGoodMod = AntiquariansGood,
                IsBannedMod = AntiquariansBanned
            };
            ModSettings["MapAdditionalEssence"] = new ModSetting 
            { 
                IsGoodMod = CrystallisedGood,
                IsBannedMod = CrystallisedBanned
            };
            ModSettings["MapMonsterFireDamage"] = new ModSetting { IsGoodMod = InfernoGood, IsBannedMod = InfernoBanned };
            ModSettings["MapMonsterColdDamage"] = new ModSetting { IsGoodMod = FrostbiteGood, IsBannedMod = FrostbiteBanned };
            ModSettings["MapMonsterLightningDamage"] = new ModSetting { IsGoodMod = ThunderGood, IsBannedMod = ThunderBanned };
            ModSettings["MapMonsterDamage"] = new ModSetting { IsGoodMod = PainGood, IsBannedMod = PainBanned };
            ModSettings["MapMonsterFast"] = new ModSetting { IsGoodMod = FleetingGood, IsBannedMod = FleetingBanned };
            ModSettings["MapMonsterCriticalStrikesAndDamage"] = new ModSetting { IsGoodMod = DestructionGood, IsBannedMod = DestructionBanned };
            ModSettings["MapMonsterLife"] = new ModSetting { IsGoodMod = ToughnessGood, IsBannedMod = ToughnessBanned };
            ModSettings["MapMonstersAllResistances"] = new ModSetting { IsGoodMod = PrismGood, IsBannedMod = PrismBanned };
            ModSettings["MapMonsterArmoured"] = new ModSetting { IsGoodMod = EnduranceGood, IsBannedMod = EnduranceBanned };
            ModSettings["MapMonstersEvasive"] = new ModSetting { IsGoodMod = BlurGood, IsBannedMod = BlurBanned };
            ModSettings["MapMonstersEnergyShield"] = new ModSetting { IsGoodMod = BufferingGood, IsBannedMod = BufferingBanned };
            ModSettings["MapPoisoning"] = new ModSetting { IsGoodMod = VenomGood, IsBannedMod = VenomBanned };
            ModSettings["MapBleeding"] = new ModSetting { IsGoodMod = PuncturingGood, IsBannedMod = PuncturingBanned };
            ModSettings["MapMonstersStunAilmentThreshold"] = new ModSetting { IsGoodMod = UnwaveringGood, IsBannedMod = UnwaveringBanned };
            ModSettings["MapMonstersArmourBreak"] = new ModSetting { IsGoodMod = ShatteringGood, IsBannedMod = ShatteringBanned };
            ModSettings["MapMonstersAccuracy"] = new ModSetting { IsGoodMod = PrecisionGood, IsBannedMod = PrecisionBanned };
            ModSettings["MapMonsterChaosDamage"] = new ModSetting { IsGoodMod = ArmageddonGood, IsBannedMod = ArmageddonBanned };
            ModSettings["MapMonsterStunBuildup"] = new ModSetting { IsGoodMod = ImpactGood, IsBannedMod = ImpactBanned };
            ModSettings["MapMonstersAilmentChance"] = new ModSetting { IsGoodMod = OverpoweringGood, IsBannedMod = OverpoweringBanned };
            ModSettings["MapMonsterMultipleProjectiles"] = new ModSetting { IsGoodMod = SplittingGood, IsBannedMod = SplittingBanned };
            ModSettings["MapMonsterAreaOfEffect"] = new ModSetting { IsGoodMod = GiantsGood, IsBannedMod = GiantsBanned };
            ModSettings["MapPlayerEnfeeblement"] = new ModSetting { IsGoodMod = EnfeeblementGood, IsBannedMod = EnfeeblementBanned };
            ModSettings["MapPlayerTemporalChains"] = new ModSetting { IsGoodMod = TemporalChainsGood, IsBannedMod = TemporalChainsBanned };
            ModSettings["MapPlayerElementalWeakness"] = new ModSetting { IsGoodMod = ElementalWeaknessGood, IsBannedMod = ElementalWeaknessBanned };
            ModSettings["MapMonstersElementalPenetration"] = new ModSetting { IsGoodMod = PenetrationGood, IsBannedMod = PenetrationBanned };
            ModSettings["MapPlayerMaxResists"] = new ModSetting { IsGoodMod = ExposureGood, IsBannedMod = ExposureBanned };
            ModSettings["MapPlayerReducedRegen"] = new ModSetting { IsGoodMod = SmotheringGood, IsBannedMod = SmotheringBanned };
            ModSettings["MapPlayerFlaskChargeGain"] = new ModSetting { IsGoodMod = DroughtGood, IsBannedMod = DroughtBanned };
            ModSettings["MapPlayerCooldownRecovery"] = new ModSetting { IsGoodMod = FatigueGood, IsBannedMod = FatigueBanned };
            ModSettings["MapMonstersBaseSelfCriticalMultiplier"] = new ModSetting { IsGoodMod = ObstructionGood, IsBannedMod = ObstructionBanned };
            ModSettings["MapMonstersCurseEffectOnSelfFinal"] = new ModSetting { IsGoodMod = HexwardedGood, IsBannedMod = HexwardedBanned };
            ModSettings["MapMonstersStealChargesOnHit"] = new ModSetting { IsGoodMod = EnervationGood, IsBannedMod = EnervationBanned };
            ModSettings["MapSpreadChilledGround"] = new ModSetting { IsGoodMod = ChilledGroundGood, IsBannedMod = ChilledGroundBanned };
            ModSettings["MapSpreadGroundEffect"] = new ModSetting { IsGoodMod = GroundEffectsGood, IsBannedMod = GroundEffectsBanned };
            ModSettings["MapSalvagersChests"] = new ModSetting { IsGoodMod = SalvagerGood, IsBannedMod = SalvagerBanned };
            ModSettings["MapBarons"] = new ModSetting { IsGoodMod = BaronGood, IsBannedMod = BaronBanned };
        }

        public void UpdateStashTabs(IEnumerable<string> stashNames)
        {
            AllStashNames = new List<string> { "None" };
            AllStashNames.AddRange(stashNames);

            InputStashTab.SetListValues(AllStashNames);
            CurrencyStashTab.SetListValues(AllStashNames);
            OutputGoodStashTab.SetListValues(AllStashNames);
            OutputBadStashTab.SetListValues(AllStashNames);
            OutputRestStashTab.SetListValues(AllStashNames);

            if (string.IsNullOrEmpty(InputStashTab.Value))
                InputStashTab.Value = "None";
            if (string.IsNullOrEmpty(CurrencyStashTab.Value))
                CurrencyStashTab.Value = "None";
            if (string.IsNullOrEmpty(OutputGoodStashTab.Value))
                OutputGoodStashTab.Value = "None";
            if (string.IsNullOrEmpty(OutputBadStashTab.Value))
                OutputBadStashTab.Value = "None";
            if (string.IsNullOrEmpty(OutputRestStashTab.Value))
                OutputRestStashTab.Value = "None";
        }

        public void DrawSettings()
        {
            // Empty as we're using the Menu system for all settings
        }
    }
}
