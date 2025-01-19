using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.Drawing;
using ExileCore2;
using ExileCore2.PoEMemory;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.Elements;
using ExileCore2.PoEMemory.Elements.InventoryElements;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Nodes;
using ExileCore2.Shared.Helpers;

namespace WaystoneCrafter
{
    public class WaystoneModifier
    {
        private readonly GameController GameController;
        internal readonly WaystoneCrafterSettings Settings;
        private bool _isCrafting;
        private int _craftedCount;
        private CancellationTokenSource _cancellationTokenSource;

        public bool IsCrafting => _isCrafting;

        public WaystoneModifier(GameController gameController, WaystoneCrafterSettings settings)
        {
            GameController = gameController;
            Settings = settings;
        }

        public async Task StartCrafting()
        {
            if (_isCrafting) return;

            _isCrafting = true;
            _craftedCount = 0;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await MainCraftingLoop(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                LogMsg("[WaystoneCrafter] Crafting operation cancelled");
            }
            catch (Exception ex)
            {
                LogError($"Error in StartCrafting: {ex.Message}");
            }
            finally
            {
                _isCrafting = false;
            }
        }

        public void StopCrafting()
        {
            if (!_isCrafting) return;
            _cancellationTokenSource?.Cancel();
            _isCrafting = false;
        }

        private async Task MainCraftingLoop(CancellationToken cancellationToken)
        {
            LogMsg("[WaystoneCrafter] Starting one batch of crafting");

            try
            {
                // First check if we have waystones in inventory before switching tabs
                var inventory = GameController.Game.IngameState.ServerData.PlayerInventories[0].Inventory;
                if (inventory == null)
                {
                    LogError("[WaystoneCrafter] Cannot access inventory");
                    return;
                }

                var waystonesInInventory = inventory.InventorySlotItems?
                    .Where(item => item?.Item?.GetComponent<Base>()?.Name?.Contains("Waystone") == true)
                    .ToList();

                if (!waystonesInInventory?.Any() == true)
                {
                    // Only switch to input tab if we have no waystones
                    if (!await SwitchToTab(Settings.InputStashTab.Value))
                    {
                        LogError("[WaystoneCrafter] Failed to switch to input tab");
                        return;
                    }
                }

                // Craft one batch of waystones
                if (!await CraftWaystones(cancellationToken))
                {
                    LogError("[WaystoneCrafter] Failed to craft waystones");
                    return;
                }

                LogMsg("[WaystoneCrafter] Finished batch, waiting for next start press");
            }
            catch (Exception ex)
            {
                LogError($"Error in MainCraftingLoop: {ex.Message}");
            }
        }

        private async Task<bool> SwitchToTab(string tabName)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                LogError($"Tab name is empty");
                return false;
            }

            var stashPanel = GameController.Game.IngameState?.IngameUi?.StashElement;
            if (stashPanel == null || !stashPanel.IsVisibleLocal)
            {
                LogError("Stash panel not visible");
                return false;
            }

            var allStashNames = stashPanel.AllStashNames;
            if (allStashNames == null)
            {
                LogError("Could not get stash tab names");
                return false;
            }

            var targetIndex = allStashNames.IndexOf(tabName);
            if (targetIndex == -1)
            {
                LogError($"Could not find tab: {tabName}");
                return false;
            }

            LogMsg($"Switching to tab: {tabName} (index {targetIndex})");
            var currentIndex = stashPanel.IndexVisibleStash;
            LogMsg($"Current tab index: {currentIndex}");
            
            var distance = targetIndex - currentIndex;
            if (distance == 0)
            {
                LogMsg("Already on correct tab");
                return true;
            }

            var key = distance < 0 ? Keys.Left : Keys.Right;
            var steps = Math.Abs(distance);

            LogMsg($"Moving {steps} steps {(distance < 0 ? "left" : "right")}");
            for (int i = 0; i < steps; i++)
            {
                Input.KeyDown(key);
                await Task.Delay(50);
                Input.KeyUp(key);
                await Task.Delay(Settings.TabSwitchDelay);
            }

            await Task.Delay(Settings.TabSwitchDelay.Value * 2);
            return true;
        }

        private async Task<bool> ReturnWaystoneToStash(NormalInventoryItem waystone)
        {
            if (waystone == null)
            {
                LogError("[WaystoneCrafter] Waystone is null");
                return false;
            }

            var mods = waystone.Item?.GetComponent<Mods>();
            if (mods == null)
            {
                LogError("Could not get mods component");
                return false;
            }

            var tabName = (await HasBannedMod(waystone.Item)) ? Settings.OutputBadStashTab.Value :
                         (await HasGoodMod(waystone.Item)) ? Settings.OutputGoodStashTab.Value :
                         Settings.OutputRestStashTab.Value;

            if (!await SwitchToTab(tabName))
            {
                LogError($"Failed to switch to tab: {tabName}");
                return false;
            }

            var rect = waystone.GetClientRect();
            Input.SetCursorPos(rect.Center);
            await Task.Delay(50);
            Input.Click(MouseButtons.Left);
            await Task.Delay(Settings.ActionDelay);
            return true;
        }

        private async Task<bool> ApplyCurrency(ExileCore2.PoEMemory.MemoryObjects.ServerInventory.InventSlotItem waystone, CancellationToken cancellationToken)
        {
            const int maxRetries = 3;
            int retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                        return false;

                    var inventory = GameController.Game.IngameState.ServerData.PlayerInventories[0].Inventory;
                    var stashPanel = GameController.Game.IngameState?.IngameUi?.StashElement;
                    if (stashPanel == null || !stashPanel.IsVisibleLocal)
                    {
                        LogError("[WaystoneCrafter] Stash panel not visible while getting currency");
                        return false;
                    }

                    var visibleStash = stashPanel.VisibleStash;
                    if (visibleStash == null)
                    {
                        LogError("[WaystoneCrafter] No visible stash while getting currency");
                        return false;
                    }

                    var items = visibleStash.VisibleInventoryItems;
                    if (items == null)
                    {
                        LogError("[WaystoneCrafter] Cannot get stash items while getting currency");
                        return false;
                    }

                    // Get waystone info for mod counts
                    var info = await WaystoneInfo.FromEntity(waystone.Item, Settings, this);
                    if (info == null) return false;

                    string currencyName = null;
                    if (info.Rarity == ItemRarity.Normal)
                    {
                        currencyName = "Orb of Alchemy";
                    }
                    else if (info.Rarity == ItemRarity.Magic)
                    {
                        if (info.PrefixCount == 0 || info.SuffixCount == 0)
                        {
                            currencyName = "Orb of Augmentation";
                        }
                        else
                        {
                            currencyName = "Regal Orb";
                        }
                    }
                    else if (info.Rarity == ItemRarity.Rare)
                    {
                        if (Settings.AlwaysFillAffixes.Value && 
                            (info.PrefixCount < 3 || info.SuffixCount < 3))
                        {
                            currencyName = "Exalted Orb";
                        }
                        else
                        {
                            return true; // Nothing more to do
                        }
                    }

                    if (string.IsNullOrEmpty(currencyName))
                    {
                        LogError("[WaystoneCrafter] Could not determine which currency to use");
                        return false;
                    }

                    // Find the currency item
                    var currency = items.FirstOrDefault(item =>
                        item?.Item?.GetComponent<Base>()?.Name?.Contains(currencyName) == true);

                    if (currency == null)
                    {
                        LogError($"[WaystoneCrafter] Could not find {currencyName} in currency tab");
                        return false;
                    }

                    var windowOffset = GameController.Window.GetWindowRectangle().TopLeft;

                    // Right-click the currency to pick it up
                    var currencyRect = currency.GetClientRect();
                    var currencyClickPos = currencyRect.Center + windowOffset;
                    LogMsg($"[WaystoneCrafter] Right-clicking {currencyName} at {currencyClickPos.X}, {currencyClickPos.Y}");

                    Input.SetCursorPos(currencyClickPos);
                    await Task.Delay(50);
                    Input.Click(MouseButtons.Right);
                    await Task.Delay(Settings.ActionDelay);

                    // Left-click the waystone to apply the currency
                    var rect = waystone.GetClientRect();
                    var clickPos = rect.Center + windowOffset;
                    LogMsg($"[WaystoneCrafter] Left-clicking waystone at {clickPos.X}, {clickPos.Y}");

                    Input.SetCursorPos(clickPos);
                    await Task.Delay(50);
                    Input.Click(MouseButtons.Left);
                    await Task.Delay(Settings.ActionDelay * 2); // Double delay to ensure mods are updated

                    return true;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    LogError($"[WaystoneCrafter] Failed to apply currency (attempt {retryCount}/{maxRetries}): {ex.Message}");
                    await Task.Delay(500); // Wait a bit before retrying
                }
            }

            LogError($"[WaystoneCrafter] Failed to apply currency after {maxRetries} attempts");
            return false;
        }

        private async Task<bool> CraftWaystones(CancellationToken cancellationToken)
        {
            const int maxRetries = 3;
            int retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    // First check inventory for waystones
                    var inventory = GameController.Game.IngameState.ServerData.PlayerInventories[0].Inventory;
                    var waystonesInInventory = inventory.InventorySlotItems?
                        .Where(item => item?.Item?.GetComponent<Base>()?.Name?.Contains("Waystone") == true)
                        .ToList();

                    if (cancellationToken.IsCancellationRequested)
                        return false;

                    LogMsg($"[WaystoneCrafter] Found {waystonesInInventory?.Count ?? 0} waystones in inventory");

                    // If we don't have enough waystones, get more from input tab
                    if ((waystonesInInventory?.Count ?? 0) < Settings.MaxWaystonesToCraft.Value)
                    {
                        // Switch to input tab to get more waystones
                        if (!await GetWaystonesFromInputTab(waystonesInInventory, cancellationToken))
                        {
                            LogError("[WaystoneCrafter] Failed to get waystones from input tab");
                            return false;
                        }

                        if (cancellationToken.IsCancellationRequested)
                            return false;

                        // Refresh inventory count after getting waystones
                        inventory = GameController.Game.IngameState.ServerData.PlayerInventories[0].Inventory;
                        waystonesInInventory = inventory.InventorySlotItems?
                            .Where(item => item?.Item?.GetComponent<Base>()?.Name?.Contains("Waystone") == true)
                            .ToList();
                    }

                    if (waystonesInInventory?.Any() != true)
                    {
                        LogError("[WaystoneCrafter] No waystones to craft");
                        return false;
                    }

                    if (cancellationToken.IsCancellationRequested)
                        return false;

                    // Switch to currency tab for crafting
                    if (!await SwitchToTab(Settings.CurrencyStashTab.Value))
                    {
                        LogError("[WaystoneCrafter] Failed to switch to currency tab");
                        return false;
                    }

                    if (cancellationToken.IsCancellationRequested)
                        return false;

                    // Craft all waystones in inventory
                    var waystonesToCraft = new List<ExileCore2.PoEMemory.MemoryObjects.ServerInventory.InventSlotItem>();
                    foreach (var waystone in waystonesInInventory)
                    {
                        if (waystone?.Item == null) continue;

                        var info = await WaystoneInfo.FromEntity(waystone.Item, Settings, this);
                        if (info == null) continue;

                        // Skip corrupted items
                        if (info.IsCorrupted)
                        {
                            LogMsg($"[WaystoneCrafter] Skipping corrupted waystone: {info.UniqueName}");
                            continue;
                        }

                        // Skip items with banned mods
                        if (info.HasBannedMod)
                        {
                            LogMsg($"[WaystoneCrafter] Skipping waystone with banned mod: {info.UniqueName}");
                            continue;
                        }

                        // Continue crafting if:
                        // - Item is not identified
                        // - Item is normal (needs to be alched)
                        // - Item is magic (needs augment/regal)
                        // - Item is rare and needs more prefixes/suffixes
                        var needsCrafting = !info.IsIdentified ||
                               info.Rarity == ItemRarity.Normal ||
                               info.Rarity == ItemRarity.Magic ||  // Always craft magic items
                               (info.Rarity == ItemRarity.Rare && 
                                (info.PrefixCount < 3 || 
                                 (Settings.AlwaysFillAffixes.Value && info.SuffixCount < 3)));  // Need to exalt

                        if (needsCrafting)
                        {
                            LogMsg($"[WaystoneCrafter] Will craft {info.UniqueName} - Rarity: {info.Rarity}, Prefixes: {info.PrefixCount}, Suffixes: {info.SuffixCount}");
                            waystonesToCraft.Add(waystone);
                        }
                        else
                        {
                            LogMsg($"[WaystoneCrafter] Skipping {info.UniqueName} - already fully crafted");
                        }
                    }

                    if (cancellationToken.IsCancellationRequested)
                        return false;

                    LogMsg($"[WaystoneCrafter] Found {waystonesToCraft.Count} unfinished waystones");

                    bool allCraftingSuccessful = true;
                    foreach (var waystone in waystonesToCraft)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return false;

                        var mods = waystone.Item?.GetComponent<Mods>();
                        var baseComponent = waystone.Item?.GetComponent<Base>();
                        if (baseComponent?.isCorrupted == true)
                            continue;
                        
                        // If unidentified, use scroll of wisdom first
                        if (mods != null && !mods.Identified)
                        {
                            LogMsg($"[WaystoneCrafter] Identifying waystone");
                            if (!await ApplyScroll(waystone))
                            {
                                LogError("[WaystoneCrafter] Failed to identify waystone");
                                allCraftingSuccessful = false;
                                continue;
                            }
                            // Refresh mods after identifying
                            mods = waystone.Item?.GetComponent<Mods>();
                        }

                        // Now apply currency if needed
                        if (mods != null && !baseComponent?.isCorrupted == true)
                        {
                            // Check for banned mods before applying currency
                            if (await HasBannedMod(waystone.Item))
                            {
                                LogMsg("[WaystoneCrafter] Found banned mod, skipping further crafting for this waystone");
                                continue;
                            }

                            if (!await ApplyCurrency(waystone, cancellationToken))
                            {
                                LogError("[WaystoneCrafter] Failed to apply currency");
                                allCraftingSuccessful = false;
                                continue;
                            }
                        }
                    }

                    if (!allCraftingSuccessful)
                    {
                        LogError("[WaystoneCrafter] Some waystones failed to craft");
                        return false;
                    }

                    // Check if we still have any unfinished waystones
                    inventory = GameController.Game.IngameState.ServerData.PlayerInventories[0].Inventory;
                    waystonesInInventory = inventory.InventorySlotItems?
                        .Where(item => item?.Item != null)
                        .Where(item => {
                            var baseComponent = item.Item.GetComponent<Base>();
                            return baseComponent != null && 
                                   baseComponent.Name.Contains("Waystone") &&
                                   !baseComponent.isCorrupted;
                        })
                        .ToList();

                    // Check if any waystones still need crafting
                    var unfinishedWaystones = new List<ExileCore2.PoEMemory.MemoryObjects.ServerInventory.InventSlotItem>();
                    foreach (var waystone in waystonesInInventory)
                    {
                        var mods = waystone.Item?.GetComponent<Mods>();
                        if (mods == null) continue;
                        
                        var baseComponent = waystone.Item?.GetComponent<Base>();
                        if (baseComponent?.isCorrupted == true) continue;
                        
                        if (await HasBannedMod(waystone.Item)) continue;
                        
                        // Check if it needs more affixes
                        if (await NeedsPrefixes(waystone.Item) || await NeedsSuffixes(waystone.Item))
                        {
                            unfinishedWaystones.Add(waystone);
                        }
                    }

                    if (unfinishedWaystones.Any())
                    {
                        LogMsg($"[WaystoneCrafter] Found {unfinishedWaystones.Count} waystones that still need crafting");
                        continue; // Continue crafting loop
                    }

                    // All waystones are finished, now sort them
                    if (!await SortWaystonesToTabs(waystonesInInventory, cancellationToken))
                    {
                        LogError("[WaystoneCrafter] Failed to sort waystones to tabs");
                        return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    LogError($"[WaystoneCrafter] Failed to craft waystones (attempt {retryCount}/{maxRetries}): {ex.Message}");
                    await Task.Delay(1000); // Wait a bit before retrying
                }
            }

            LogError($"[WaystoneCrafter] Failed to craft waystones after {maxRetries} attempts");
            return false;
        }

        private async Task<bool> ApplyScroll(ExileCore2.PoEMemory.MemoryObjects.ServerInventory.InventSlotItem waystone)
        {
            if (!await SwitchToTab(Settings.CurrencyStashTab.Value))
            {
                return false;
            }

            var stashPanel = GameController.Game.IngameState?.IngameUi?.StashElement;
            if (stashPanel == null || !stashPanel.IsVisibleLocal)
            {
                LogError("Stash panel not visible while getting scroll");
                return false;
            }

            var visibleStash = stashPanel.VisibleStash;
            if (visibleStash == null)
            {
                LogError("No visible stash while getting scroll");
                return false;
            }

            var items = visibleStash.VisibleInventoryItems;
            if (items == null)
            {
                LogError("Cannot get stash items while getting scroll");
                return false;
            }

            // Find scroll of wisdom
            var scroll = items.FirstOrDefault(item =>
                item?.Item?.GetComponent<Base>()?.Name?.Contains("Scroll of Wisdom") == true);

            if (scroll == null)
            {
                LogError("Could not find Scroll of Wisdom in currency tab");
                return false;
            }

            // Get window offset for click positions
            var windowOffset = GameController.Window.GetWindowRectangle().TopLeft;

            // Right-click the scroll to pick it up
            var scrollRect = scroll.GetClientRect();
            var scrollClickPos = scrollRect.Center + windowOffset;

            Input.SetCursorPos(scrollClickPos);
            await Task.Delay(50);
            Input.Click(MouseButtons.Right);
            await Task.Delay(Settings.ActionDelay);

            // Get the click position directly from the waystone's client rect
            var waystoneRect = waystone.GetClientRect();
            var waystoneClickPos = waystoneRect.Center + windowOffset;

            // Left-click the waystone to apply the scroll
            Input.SetCursorPos(waystoneClickPos);
            await Task.Delay(50);
            Input.Click(MouseButtons.Left);
            await Task.Delay(Settings.ActionDelay * 2); // Double delay to ensure mods are updated

            // Return true if the item is now identified
            var mods = waystone.Item?.GetComponent<Mods>();
            return mods != null && mods.Identified;
        }

        private async Task<List<NormalInventoryItem>> GetWaystonesFromInputTab()
        {
            var waystones = new List<NormalInventoryItem>();

            var stashPanel = GameController.Game.IngameState?.IngameUi?.StashElement;
            if (stashPanel == null || !stashPanel.IsVisibleLocal)
            {
                LogError("[WaystoneCrafter] Stash panel not visible");
                return null;
            }

            // Switch to input tab
            if (!await SwitchToTab(Settings.InputStashTab.Value))
            {
                LogError("[WaystoneCrafter] Failed to switch to input tab");
                return null;
            }

            await Task.Delay(Settings.ActionDelay); // Wait for tab switch to complete

            // Get items from stash
            var visibleStash = stashPanel.VisibleStash;
            if (visibleStash == null)
            {
                LogError("[WaystoneCrafter] No visible stash");
                return null;
            }

            var items = visibleStash.VisibleInventoryItems;
            if (items == null)
            {
                LogError("[WaystoneCrafter] No items in stash");
                return null;
            }

            LogMsg($"[WaystoneCrafter] Found {items.Count} items in stash tab");

            // Find waystones
            foreach (var item in items)
            {
                if (item?.Item == null) continue;

                var baseComponent = item.Item.GetComponent<Base>();
                if (baseComponent == null || !baseComponent.Name.Contains("Waystone")) continue;

                // Skip corrupted items
                if (baseComponent.isCorrupted)
                {
                    LogMsg($"[WaystoneCrafter] Skipping corrupted waystone: {baseComponent.Name}");
                    continue;
                }

                LogMsg($"[WaystoneCrafter] Found waystone: {baseComponent.Name}");
                waystones.Add(item);
            }

            LogMsg($"[WaystoneCrafter] Found {waystones.Count} waystones in input tab");
            return waystones;
        }

        private async Task<bool> GetWaystonesFromInputTab(List<ExileCore2.PoEMemory.MemoryObjects.ServerInventory.InventSlotItem> waystonesInInventory, CancellationToken cancellationToken)
        {
            var stashPanel = GameController.Game.IngameState?.IngameUi?.StashElement;
            if (stashPanel == null || !stashPanel.IsVisibleLocal)
            {
                LogError("[WaystoneCrafter] Stash panel not visible");
                return false;
            }

            // Switch to input tab
            if (!await SwitchToTab(Settings.InputStashTab.Value))
            {
                LogError("[WaystoneCrafter] Failed to switch to input tab");
                return false;
            }

            await Task.Delay(Settings.ActionDelay); // Wait for tab switch to complete

            // Get items from stash
            var visibleStash = stashPanel.VisibleStash;
            if (visibleStash == null)
            {
                LogError("[WaystoneCrafter] No visible stash");
                return false;
            }

            var items = visibleStash.VisibleInventoryItems;
            if (items == null)
            {
                LogError("[WaystoneCrafter] No items in stash");
                return false;
            }

            LogMsg($"[WaystoneCrafter] Found {items.Count} items in stash tab");

            // Find waystones
            var stashWaystones = items
                .Where(item => item?.Item != null)
                .Where(item => item.Item.GetComponent<Base>() != null)
                .Where(item => item.Item.GetComponent<Base>().Name.Contains("Waystone"))
                .Where(item => !item.Item.GetComponent<Base>().isCorrupted)
                .Take(Settings.MaxWaystonesToCraft.Value - (waystonesInInventory?.Count ?? 0))
                .ToList();

            if (stashWaystones == null || !stashWaystones.Any())
            {
                LogError("[WaystoneCrafter] No waystones found in input tab");
                return false;
            }

            LogMsg($"[WaystoneCrafter] Taking {stashWaystones.Count} waystones from input tab");

            // Ctrl-click each waystone to move it to inventory
            foreach (var waystone in stashWaystones)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                var rect = waystone.GetClientRect();
                var windowOffset = GameController.Window.GetWindowRectangle().TopLeft;
                var clickPos = rect.Center + windowOffset;

                Input.KeyDown(Keys.LControlKey);
                await Task.Delay(50);

                Input.SetCursorPos(clickPos);
                await Task.Delay(50);
                Input.Click(MouseButtons.Left);

                Input.KeyUp(Keys.LControlKey);
                await Task.Delay(Settings.ActionDelay);
            }

            return true;
        }

        private async Task<bool> SortWaystonesToTabs(List<ExileCore2.PoEMemory.MemoryObjects.ServerInventory.InventSlotItem> waystones, CancellationToken cancellationToken)
        {
            if (waystones == null || !waystones.Any())
                return true;

            LogMsg($"[WaystoneCrafter] Starting to sort {waystones.Count} waystones to their respective tabs");
            LogMsg($"[WaystoneCrafter] Using tabs: Good={Settings.OutputGoodStashTab.Value}, Bad={Settings.OutputBadStashTab.Value}, Rest={Settings.OutputRestStashTab.Value}");

            // Keep trying to sort until inventory is empty or we're cancelled
            while (!cancellationToken.IsCancellationRequested)
            {
                // Refresh inventory to get current waystones
                var inventory = GameController.Game.IngameState.ServerData.PlayerInventories[0].Inventory;
                var currentWaystones = inventory.InventorySlotItems?
                    .Where(item => item?.Item != null)
                    .Where(item => {
                        var baseComponent = item.Item.GetComponent<Base>();
                        return baseComponent != null && 
                               baseComponent.Name.Contains("Waystone") &&
                               !baseComponent.isCorrupted;
                    })
                    .ToList();

                if (currentWaystones == null || !currentWaystones.Any())
                {
                    LogMsg("[WaystoneCrafter] No more waystones in inventory, sorting complete");
                    return true;
                }

                LogMsg($"[WaystoneCrafter] Still have {currentWaystones.Count} waystones to sort");

                foreach (var waystone in currentWaystones)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return false;

                    var info = await WaystoneInfo.FromEntity(waystone.Item, Settings, this);
                    if (info == null)
                    {
                        LogError($"[WaystoneCrafter] Could not get waystone info");
                        continue;
                    }

                    info.LogDetails(this);

                    string targetTab;
                    if (info.HasBannedMod)
                    {
                        var bannedMod = info.Mods.First(m => m.IsBanned);
                        targetTab = Settings.OutputBadStashTab.Value;
                        LogMsg($"[WaystoneCrafter] Sorting {info.Name} -> {targetTab}");
                        LogMsg($"[WaystoneCrafter] Reason: Has banned mod '{bannedMod.DisplayName}'");
                        LogMsg($"[WaystoneCrafter] Current mods: {string.Join(", ", info.Mods.Select(m => m.DisplayName))}");
                    }
                    else if (!info.IsIdentified || 
                            info.Rarity == ItemRarity.Normal ||
                            (info.Rarity == ItemRarity.Magic && !info.HasBannedMod) ||  // Always craft magic items unless banned
                            (info.Rarity == ItemRarity.Rare && info.PrefixCount < 3) ||  // Need 3 prefixes
                            (info.Rarity == ItemRarity.Rare && Settings.AlwaysFillAffixes.Value && info.SuffixCount < 3))  // Need 3 suffixes if enabled
                    {
                        LogMsg($"[WaystoneCrafter] Skipping {info.Name} - still needs crafting");
                        LogMsg($"[WaystoneCrafter] Status: Rarity={info.Rarity}, Prefixes={info.PrefixCount}/3, Suffixes={info.SuffixCount}/3");
                        if (!info.IsIdentified) LogMsg($"[WaystoneCrafter] Needs identification");
                        continue;
                    }
                    else if (info.HasGoodMod && info.PrefixCount >= 3 && (!Settings.AlwaysFillAffixes.Value || info.SuffixCount >= 3))
                    {
                        targetTab = Settings.OutputGoodStashTab.Value;
                        var goodMods = info.Mods.Where(m => m.IsGood).Select(m => m.DisplayName);
                        LogMsg($"[WaystoneCrafter] Sorting {info.Name} -> {targetTab}");
                        LogMsg($"[WaystoneCrafter] Reason: Fully crafted with good mods: {string.Join(", ", goodMods)}");
                        LogMsg($"[WaystoneCrafter] Status: Prefixes={info.PrefixCount}/3, Suffixes={info.SuffixCount}/3");
                    }
                    else
                    {
                        targetTab = Settings.OutputRestStashTab.Value;
                        LogMsg($"[WaystoneCrafter] Sorting {info.Name} -> {targetTab}");
                        if (info.HasGoodMod)
                        {
                            LogMsg($"[WaystoneCrafter] Reason: Has good mods but needs {(info.PrefixCount < 3 ? $"{3 - info.PrefixCount} more prefixes" : $"{3 - info.SuffixCount} more suffixes")}");
                            LogMsg($"[WaystoneCrafter] Good mods present: {string.Join(", ", info.Mods.Where(m => m.IsGood).Select(m => m.DisplayName))}");
                        }
                        else
                        {
                            LogMsg($"[WaystoneCrafter] Reason: No good mods present");
                        }
                        LogMsg($"[WaystoneCrafter] Current mods: {string.Join(", ", info.Mods.Select(m => m.DisplayName))}");
                    }

                    if (!await SwitchToTab(targetTab))
                    {
                        LogError($"Failed to switch to tab: {targetTab}");
                        continue;
                    }

                    var rect = waystone.GetClientRect();
                    var windowOffset = GameController.Window.GetWindowRectangle().TopLeft;
                    var clickPos = rect.Center + windowOffset;

                    Input.KeyDown(Keys.LControlKey);
                    await Task.Delay(50);

                    Input.SetCursorPos(clickPos);
                    await Task.Delay(50);
                    Input.Click(MouseButtons.Left);

                    Input.KeyUp(Keys.LControlKey);
                    await Task.Delay(Settings.ActionDelay);
                }
            }

            return true;
        }

        private async Task StashItem(Entity item, string tabName)
        {
            var name = item.GetComponent<Base>()?.Name ?? "Unknown";
            var prefixCount = await CountPrefixes(item);
            var suffixCount = await CountSuffixes(item);
            var hasGoodMod = await HasGoodMod(item);
            var mods = item?.GetComponent<Mods>();

            LogMsg($"[WaystoneCrafter] Stashing {name} in {tabName} tab");
            LogMsg($"[WaystoneCrafter] Map details: {prefixCount} prefixes, {suffixCount} suffixes, Has good mod: {hasGoodMod}");
            
            if (mods != null)
            {
                LogMsg("[WaystoneCrafter] Mods on this map:");
                foreach (var mod in mods.ItemMods)
                {
                    LogMsg($"  - {mod.Name}: {mod.DisplayName}");
                }
            }

            // Original stashing logic here...
        }

        public async Task<bool> HasBannedMod(Entity item)
        {
            var info = await WaystoneInfo.FromEntity(item, Settings, this);
            return info?.HasBannedMod ?? false;
        }

        public async Task<bool> IsCorrupted(Entity item)
        {
            var baseComponent = item?.GetComponent<Base>();
            return baseComponent?.isCorrupted ?? false;
        }

        public async Task<int> CountPrefixes(Entity item)
        {
            var info = await WaystoneInfo.FromEntity(item, Settings, this);
            return info?.PrefixCount ?? 0;
        }

        public async Task<int> CountSuffixes(Entity item)
        {
            var info = await WaystoneInfo.FromEntity(item, Settings, this);
            return info?.SuffixCount ?? 0;
        }

        public async Task<bool> NeedsPrefixes(Entity item)
        {
            return await CountPrefixes(item) < 3;
        }

        public async Task<bool> NeedsSuffixes(Entity item)
        {
            return await CountSuffixes(item) < 3;
        }

        public async Task<bool> HasGoodMod(Entity item)
        {
            var info = await WaystoneInfo.FromEntity(item, Settings, this);
            return info?.HasGoodMod ?? false;
        }

        internal void LogMsg(string message)
        {
            if (Settings.Debug.Value)
            {
                DebugWindow.LogMsg(message);
            }
        }

        private void LogError(string message)
        {
            DebugWindow.LogError(message);
        }
    }
}
