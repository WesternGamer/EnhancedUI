using System;
using System.Collections.Generic;
using CefSharp;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI.Interfaces;

namespace EnhancedUI.Gui.Terminal.ControlPanel
{
    public class ControlPanelState : PanelState
    {
        public static ControlPanelState? Instance;

        // Link to SE's data model
        private MyTerminalBlock? interactedBlock;

        // Blocks by EntityId
        private readonly Dictionary<long, MyTerminalBlock> terminalBlocks = new();

        // Blocks states by EntityId
        private readonly Dictionary<long, BlockState> blockStates = new();

        // Getters, because CefSharp relays only method calls
        // ReSharper disable once UnusedMember.Global
        public Dictionary<long, BlockState> GetBlockStates() => blockStates;
        public BlockState GetBlockState(long entityId) => blockStates[entityId];

        public ControlPanelState()
        {
            Instance = this;
        }

        public override void Reload()
        {
            base.Reload();
            Clear();
        }

        public void Clear()
        {
            interactedBlock = null;

            foreach (var terminalBlock in terminalBlocks.Values)
            {
                terminalBlock.PropertiesChanged -= OnPropertyChanged;
            }

            terminalBlocks.Clear();
            blockStates.Clear();

            if (HasBound())
            {
                Browser.ExecuteScriptAsync("stateUpdated();");
            }
        }

        public void Update(MyTerminalBlock block)
        {
            if (!HasBound())
            {
                return;
            }

            if (interactedBlock?.EntityId == block.EntityId)
            {
                return;
            }

            interactedBlock = block;

            LoadBlockStates();

            foreach (var terminalBlock in terminalBlocks.Values)
            {
                terminalBlock.PropertiesChanged += OnPropertyChanged;
            }

            Browser.ExecuteScriptAsync("stateUpdated();");
        }

        // ReSharper disable once UnusedMember.Global

        private void LoadBlockStates()
        {
            terminalBlocks.Clear();
            blockStates.Clear();

            if (interactedBlock == null)
            {
                return;
            }

            foreach (var terminalBlock in interactedBlock.CubeGrid.GridSystems.TerminalSystem.Blocks)
            {
                terminalBlocks[terminalBlock.EntityId] = terminalBlock;
                blockStates[terminalBlock.EntityId] = new BlockState(terminalBlock);
            }
        }

        private void OnPropertyChanged(MyTerminalBlock terminalBlock)
        {
            if (terminalBlock.Closed)
                return;

            var entityId = terminalBlock.EntityId;

            blockStates[entityId] = new BlockState(terminalBlock);

            if (HasBound())
            {
                Browser.ExecuteScriptAsync("blockStateUpdated('" + entityId + "');");
            }
        }

        // Methods to call from JS to change blocks

        public void ModifyBlock(long entityId, string name, string customData)
        {
            var block = terminalBlocks[entityId];
            if (block.Closed)
                return;

            block.Name = name;
            block.CustomData = customData;
            blockStates[entityId] = new BlockState(block);

            if (HasBound())
            {
                Browser.ExecuteScriptAsync("blockStateUpdated('" + entityId + "');");
            }
        }

        public void ModifyBlockAttribute(long entityId, string propertyId, object value)
        {
            var block = terminalBlocks[entityId];
            if (block.Closed)
                return;

            var property = block.GetProperty(propertyId);

            switch (value)
            {
                case bool boolValue:
                    property.AsBool().SetValue(block, boolValue);
                    break;

                case long intValue:
                    property.As<Int64>().SetValue(block, intValue);
                    break;

                case float floatValue:
                    property.AsFloat().SetValue(block, floatValue);
                    break;
            }

            blockStates[entityId] = new BlockState(block);

            if (HasBound())
            {
                Browser.ExecuteScriptAsync("blockStateUpdated('" + entityId + "');");
            }
        }
    }
}