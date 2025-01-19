using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExileCore2;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Interfaces;
using ImGuiNET;

namespace WaystoneCrafter
{
    public class WaystoneFilter
    {
        private readonly WaystoneCrafterSettings Settings;
        private readonly WaystoneModifier _waystoneModifier;

        public WaystoneFilter(WaystoneCrafterSettings settings, WaystoneModifier waystoneModifier)
        {
            Settings = settings;
            _waystoneModifier = waystoneModifier;
        }

        private void LogMsg(string msg)
        {
            if (Settings.Debug.Value)
                DebugWindow.LogMsg(msg);
        }

        public async Task<bool> ShouldKeepItem(Entity item)
        {
            if (item == null) return false;

            var mods = item.GetComponent<Mods>();
            if (mods == null) return false;

            // Skip corrupted items
            if (await _waystoneModifier.IsCorrupted(item))
            {
                _waystoneModifier.LogMsg("[WaystoneCrafter] Skipping corrupted item");
                return false;
            }

            // Keep items with banned mods for output
            if (await _waystoneModifier.HasBannedMod(item))
            {
                _waystoneModifier.LogMsg("[WaystoneCrafter] Keeping item with banned mod for output");
                return true;
            }

            // Keep items with good mods for output
            if (await _waystoneModifier.HasGoodMod(item))
            {
                _waystoneModifier.LogMsg("[WaystoneCrafter] Keeping item with good mod for output");
                return true;
            }

            // Keep items that need crafting
            if (await NeedsCrafting(item))
            {
                _waystoneModifier.LogMsg("[WaystoneCrafter] Keeping item that needs crafting");
                return true;
            }

            _waystoneModifier.LogMsg("[WaystoneCrafter] Item does not meet any keep criteria");
            return false;
        }

        private async Task<bool> NeedsCrafting(Entity item)
        {
            var mods = item.GetComponent<Mods>();
            if (mods == null) return false;

            // Always craft normal items
            if (mods.ItemRarity == ItemRarity.Normal)
                return true;

            // Craft magic items that don't have 2 mods yet
            if (mods.ItemRarity == ItemRarity.Magic && mods.ItemMods.Count < 2)
                return true;

            // Craft magic items with 2 mods to make them rare
            if (mods.ItemRarity == ItemRarity.Magic && mods.ItemMods.Count == 2)
                return true;

            // Craft rare items that need more affixes
            if (mods.ItemRarity == ItemRarity.Rare)
            {
                var prefixCount = await _waystoneModifier.CountPrefixes(item);
                var suffixCount = await _waystoneModifier.CountSuffixes(item);

                if (prefixCount < 3 || (Settings.AlwaysFillAffixes.Value && suffixCount < 3))
                    return true;
            }

            return false;
        }

        public async Task<bool> HasBannedMod(Entity waystone)
        {
            var result = await _waystoneModifier.HasBannedMod(waystone);
            var name = waystone.GetComponent<Base>()?.Name ?? "Unknown";
            var mods = waystone.GetComponent<Mods>();
            
            if (mods != null)
            {
                foreach (var mod in mods.ItemMods)
                {
                    if (Settings.ModSettings.TryGetValue(mod.Name, out var setting) && setting.IsBannedMod.Value)
                    {
                        LogMsg($"[WaystoneCrafter] Map {name} has banned mod: {mod.Name} ({mod.DisplayName})");
                    }
                }
            }
            
            return result;
        }

        public async Task<bool> HasGoodMod(Entity waystone)
        {
            var result = await _waystoneModifier.HasGoodMod(waystone);
            var name = waystone.GetComponent<Base>()?.Name ?? "Unknown";
            var mods = waystone.GetComponent<Mods>();
            
            if (mods != null)
            {
                foreach (var mod in mods.ItemMods)
                {
                    if (Settings.ModSettings.TryGetValue(mod.Name, out var setting) && setting.IsGoodMod.Value)
                    {
                        LogMsg($"[WaystoneCrafter] Map {name} has good mod: {mod.Name} ({mod.DisplayName})");
                    }
                }
            }
            
            return result;
        }

        public async Task<float> CalculateWaystoneScore(Entity waystone)
        {
            if (await HasBannedMod(waystone))
                return float.MinValue;

            return await GetScore(waystone);
        }

        private async Task<string> DetermineDestinationTab(Entity item)
        {
            var hasBannedMod = await _waystoneModifier.HasBannedMod(item);
            var hasGoodMod = await _waystoneModifier.HasGoodMod(item);
            var prefixCount = await _waystoneModifier.CountPrefixes(item);
            var suffixCount = await _waystoneModifier.CountSuffixes(item);
            var name = item.GetComponent<Base>()?.Name ?? "Unknown";
            var mods = item.GetComponent<Mods>();

            LogMsg($"\n[WaystoneCrafter] ============ Sorting {name} ============");
            LogMsg($"[WaystoneCrafter] Status: Banned mod: {hasBannedMod}, Good mod: {hasGoodMod}");
            LogMsg($"[WaystoneCrafter] Affixes: {prefixCount} prefixes, {suffixCount} suffixes");
            LogMsg($"[WaystoneCrafter] Rarity: {mods?.ItemRarity}");
            
            if (mods != null)
            {
                LogMsg("[WaystoneCrafter] All mods on this map:");
                foreach (var mod in mods.ItemMods)
                {
                    LogMsg($"  - {mod.Name}: {mod.DisplayName}");
                    if (Settings.ModSettings.TryGetValue(mod.Name, out var setting) && setting.IsBannedMod.Value)
                    {
                        LogMsg($"  - WARNING: This is a banned mod");
                    }
                }
            }

            // Always move items with banned mods to bad maps tab, regardless of rarity or crafting state
            if (hasBannedMod)
            {
                LogMsg($"[WaystoneCrafter] {name} -> Bad Maps (has banned mod)");
                return Settings.OutputBadStashTab.Value;
            }

            // A map is only considered "good" if it has both a good mod AND is fully crafted
            bool isFullyCrafted = prefixCount >= 3 && (!Settings.AlwaysFillAffixes.Value || suffixCount >= 3);
            if (hasGoodMod)
            {
                if (isFullyCrafted)
                {
                    LogMsg($"[WaystoneCrafter] {name} -> Good Maps (has good mod and fully crafted)");
                    return Settings.OutputGoodStashTab.Value;
                }
                else
                {
                    LogMsg($"[WaystoneCrafter] {name} -> Rest Maps (has good mod but not fully crafted)");
                    LogMsg($"[WaystoneCrafter] Reason: Needs {(prefixCount < 3 ? "prefixes" : "suffixes")}");
                }
            }
            else
            {
                LogMsg($"[WaystoneCrafter] {name} -> Rest Maps (no good mods)");
            }

            return Settings.OutputRestStashTab.Value;
        }

        private async Task<int> GetScore(Entity item)
        {
            var score = 0;
            var mods = item?.GetComponent<Mods>();
            var name = item.GetComponent<Base>()?.Name ?? "Unknown";
            if (mods == null) return score;

            LogMsg($"[WaystoneCrafter] Calculating score for {name}:");
            foreach (var mod in mods.ItemMods)
            {
                if (Settings.ModSettings.TryGetValue(mod.Name, out var setting))
                {
                    if (setting.IsGoodMod.Value)
                    {
                        score++;
                        LogMsg($"  + {mod.Name} ({mod.DisplayName}) is good");
                    }
                    if (setting.IsBannedMod.Value)
                    {
                        score--;
                        LogMsg($"  - {mod.Name} ({mod.DisplayName}) is banned");
                    }
                }
            }
            LogMsg($"[WaystoneCrafter] Final score for {name}: {score}");
            return score;
        }
    }
}
