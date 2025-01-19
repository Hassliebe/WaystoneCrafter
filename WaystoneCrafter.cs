using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore2;
using ExileCore2.PoEMemory;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.PoEMemory.Elements;
using ExileCore2.PoEMemory.Elements.InventoryElements;
using ExileCore2.Shared.Cache;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared;

namespace WaystoneCrafter
{
    public class WaystoneCrafter : BaseSettingsPlugin<WaystoneCrafterSettings>
    {
        private WaystoneModifier Modifier;
        private WaystoneFilter Filter;
        private bool _stashTabNamesInitialized;

        public override bool Initialise()
        {
            try
            {
                Modifier = new WaystoneModifier(GameController, Settings);
                Filter = new WaystoneFilter(Settings, Modifier);
                return true;
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"[WaystoneCrafter] Error in Initialise: {ex.Message}");
                return false;
            }
        }

        public override void Tick()
        {
            if (Settings.Enable.Value && !_stashTabNamesInitialized)
            {
                InitializeStashTabNames();
            }
        }

        public override void Render()
        {
            try
            {
                if (!Settings.Enable.Value) return;

                if (Settings.StartKey.PressedOnce())
                {
                    if (!Modifier.IsCrafting)
                    {
                        _ = Modifier.StartCrafting();
                        if (Settings.Debug.Value)
                            DebugWindow.LogMsg("[WaystoneCrafter] Crafting started");
                    }
                }
                else if (Settings.StopKey.PressedOnce())
                {
                    if (Modifier.IsCrafting)
                    {
                        Modifier.StopCrafting();
                        if (Settings.Debug.Value)
                            DebugWindow.LogMsg("[WaystoneCrafter] Crafting stopped");
                    }
                }
            }
            catch (Exception ex)
            {
                DebugWindow.LogError($"[WaystoneCrafter] Error in Render: {ex.Message}");
            }
        }

        private void InitializeStashTabNames()
        {
            var stashPanel = GameController.Game.IngameState?.IngameUi?.StashElement;
            if (stashPanel == null || !stashPanel.IsVisibleLocal) return;

            var allStashNames = stashPanel.AllStashNames;
            if (allStashNames == null) return;

            var stashNamesList = allStashNames.ToList();
            Settings.InputStashTab.SetListValues(stashNamesList);
            Settings.CurrencyStashTab.SetListValues(stashNamesList);
            Settings.OutputGoodStashTab.SetListValues(stashNamesList);
            Settings.OutputBadStashTab.SetListValues(stashNamesList);
            Settings.OutputRestStashTab.SetListValues(stashNamesList);

            _stashTabNamesInitialized = true;
        }
    }
}
