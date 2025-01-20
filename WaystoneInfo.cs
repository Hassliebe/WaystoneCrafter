using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;
using ExileCore2.Shared.Nodes;
using WaystoneCrafter;

namespace WaystoneCrafter
{
    public class WaystoneInfo
    {
        public string Name { get; private set; }
        public string UniqueName { get; private set; }
        public List<(string InternalName, string DisplayName, bool IsGood, bool IsBanned, bool IsPrefix)> Mods { get; private set; } = new();
        public int PrefixCount { get; private set; }
        public int SuffixCount { get; private set; }
        public bool HasGoodMod { get; private set; }
        public bool HasBannedMod { get; private set; }
        public ItemRarity Rarity { get; private set; }
        public bool IsCorrupted { get; private set; }
        public bool IsIdentified { get; private set; }

        public static async Task<WaystoneInfo> FromEntity(Entity item, WaystoneCrafterSettings settings, WaystoneModifier modifier)
        {
            if (item == null) return null;

            var info = new WaystoneInfo();
            var baseComponent = item.GetComponent<Base>();
            var modsComponent = item.GetComponent<Mods>();

            if (baseComponent == null || modsComponent == null) return null;

            info.Name = baseComponent.Name;
            info.UniqueName = modsComponent.UniqueName ?? "Unknown";
            info.Rarity = modsComponent.ItemRarity;
            info.IsCorrupted = baseComponent.isCorrupted;
            info.IsIdentified = modsComponent.Identified;

            modifier.LogMsg($"[WaystoneCrafter] Processing waystone: {info.Name} ({info.UniqueName})");
            modifier.LogMsg($"[WaystoneCrafter] Rarity: {info.Rarity}, Corrupted: {info.IsCorrupted}, Identified: {info.IsIdentified}");

            // Process mods
            var allMods = modsComponent.ItemMods?.ToList() ?? new List<ItemMod>();
            if (allMods.Count == 0)
            {
                // Wait a bit and try again in case the mods haven't updated yet
                await Task.Delay(100);
                allMods = modsComponent.ItemMods?.ToList() ?? new List<ItemMod>();
            }
            
            modifier.LogMsg($"[WaystoneCrafter] Found {allMods.Count} mods on {info.Name} ({info.UniqueName})");
            
            foreach (var mod in allMods)
    {
        var modDef = WaystoneModDefinitions.GetModDefinition(mod.Name, mod.DisplayName);
        var modName = modDef?.InternalName ?? mod.Name;
        var isPrefix = modDef?.IsPrefix ?? !mod.DisplayName.StartsWith("of", StringComparison.OrdinalIgnoreCase);
        
        // Get the display name without "of " prefix for suffix property names
        var settingsName = mod.DisplayName;
        if (settingsName.StartsWith("of ", StringComparison.OrdinalIgnoreCase))
        {
            settingsName = settingsName.Substring(3);
        }
        
        // Simply check if the mod is good or banned using display name
        var goodToggleName = $"{settingsName}Good";
        var bannedToggleName = $"{settingsName}Banned";
        
        var goodProp = typeof(WaystoneCrafterSettings).GetProperty(goodToggleName);
        var bannedProp = typeof(WaystoneCrafterSettings).GetProperty(bannedToggleName);
        
        bool isGood = goodProp?.GetValue(settings) is ToggleNode goodNode && goodNode.Value;
        bool isBanned = bannedProp?.GetValue(settings) is ToggleNode bannedNode && bannedNode.Value;

        if (modDef == null)
        {
            modifier.LogMsg($"[WaystoneCrafter] Warning: No mod definition found for {modName} ({mod.DisplayName})");
        }
        
        info.Mods.Add((modName, mod.DisplayName, isGood, isBanned, isPrefix));
        modifier.LogMsg($"[WaystoneCrafter] Found mod on {info.UniqueName}: {modName} ({mod.DisplayName}) - {(isPrefix ? "Prefix" : "Suffix")}, Good: {isGood}, Banned: {isBanned}");
    }

            // Count prefixes and suffixes
            info.PrefixCount = info.Mods.Count(m => m.IsPrefix);
            info.SuffixCount = info.Mods.Count(m => !m.IsPrefix);
            info.HasGoodMod = info.Mods.Any(m => m.IsGood);
            info.HasBannedMod = info.Mods.Any(m => m.IsBanned);

            modifier.LogMsg($"[WaystoneCrafter] {info.UniqueName} has {info.PrefixCount} prefixes and {info.SuffixCount} suffixes");

            return info;
        }

        public void LogDetails(WaystoneModifier modifier)
        {
            modifier.LogMsg($"[WaystoneCrafter] Waystone details for {Name} ({UniqueName}):");
            modifier.LogMsg($"[WaystoneCrafter] - Rarity: {Rarity}");
            modifier.LogMsg($"[WaystoneCrafter] - Corrupted: {IsCorrupted}");
            modifier.LogMsg($"[WaystoneCrafter] - Identified: {IsIdentified}");
            modifier.LogMsg($"[WaystoneCrafter] - Prefixes: {PrefixCount}");
            modifier.LogMsg($"[WaystoneCrafter] - Suffixes: {SuffixCount}");
            modifier.LogMsg($"[WaystoneCrafter] - Has Good Mod: {HasGoodMod}");
            modifier.LogMsg($"[WaystoneCrafter] - Has Banned Mod: {HasBannedMod}");
            
            if (Mods.Any())
            {
                modifier.LogMsg($"[WaystoneCrafter] - Mods:");
                foreach (var (internalName, displayName, isGood, isBanned, isPrefix) in Mods)
                {
                    modifier.LogMsg($"[WaystoneCrafter]   * {internalName} ({displayName}) - Good: {isGood}, Banned: {isBanned}, {(isPrefix ? "Prefix" : "Suffix")}");
                }
            }
            else
            {
                modifier.LogMsg($"[WaystoneCrafter] - No mods found");
            }
        }
    }
}
