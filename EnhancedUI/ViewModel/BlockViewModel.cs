using System;
using System.Collections.Generic;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI.Interfaces;

namespace EnhancedUI.ViewModel
{
    // Block view model passed to JavaScript
    public class BlockViewModel : IDisposable
    {
        // Owning block view model
        private readonly TerminalViewModel terminalModel;

        // Terminal block
        private readonly MyTerminalBlock block;

        // State version
        public long Version { get; private set; }

        // Terminal property view models for the block
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly Dictionary<string, PropertyViewModel> Properties = new();

        // True as long as the block is valid (not closed),
        // set to false if a block is destroyed
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsValid { get; private set; }

        // True if the block is functional
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsFunctional { get; private set; }

        // Name of the block (used editable)
        // ReSharper disable once MemberCanBePrivate.Global
        public string Name { get; set; }

        // Custom data (user editable)
        // ReSharper disable once MemberCanBePrivate.Global
        public string CustomData { get; set; }

        // Detailed information may be provided by some of the blocks (read-only)
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string DetailedInfo { get; private set; }

        // Block identification
        public long EntityId => block.EntityId;
        public override int GetHashCode() => EntityId.GetHashCode();

        // Block type
        public string ClassName => block.GetType().Name;
        public string TypeId => block.BlockDefinition.Id.TypeId.ToString();
        public string SubtypeName => block.BlockDefinition.Id.SubtypeName;

#pragma warning disable 8618
        public BlockViewModel(TerminalViewModel terminalViewModel, MyTerminalBlock terminalBlock, long version)
        {
            terminalModel = terminalViewModel;
            block = terminalBlock;
            Version = version;

            UpdateFields(version);
            CreatePropertyModels();

            block.PropertiesChanged += OnPropertyChanged;
        }
#pragma warning restore 8618

        private bool UpdateFields(long version)
        {
            var changed = false;

            var isValid = !block.Closed && block.InScene && !block.IsPreview;
            if (isValid != IsValid)
            {
                IsValid = isValid;
                changed = true;
            }

            var isFunctional = block.IsFunctional;
            if (isFunctional != IsFunctional)
            {
                IsFunctional = isFunctional;
                changed = true;
            }

            var name = block.CustomName.ToString();
            if (name == "")
                name = block.DisplayNameText ?? block.DisplayName;
            if (name != Name)
            {
                Name = name;
                changed = true;
            }

            if (CustomData != block.CustomData)
            {
                CustomData = block.CustomData;
                changed = true;
            }

            var detailedInfo = block.DetailedInfo.ToString();
            if (detailedInfo != DetailedInfo)
            {
                DetailedInfo = detailedInfo;
                changed = true;
            }

            if (changed)
                Version = version;

            return changed;
        }

        private void CreatePropertyModels()
        {
            var terminalProperties = new List<ITerminalProperty>();
            block.GetProperties(terminalProperties);
            foreach (var terminalProperty in terminalProperties)
            {
                Properties[terminalProperty.Id] = new PropertyViewModel(block, terminalProperty);
            }
        }

        public void Dispose()
        {
            block.PropertiesChanged -= OnPropertyChanged;

            Properties.Clear();
        }

        private void OnPropertyChanged(MyTerminalBlock obj)
        {
            terminalModel.NotifyGameModifiedBlock(EntityId);
        }

        // Updates model from game state, returns true if anything has changed
        public bool Update(long version)
        {
            var changed = UpdateFields(version);
            return UpdateProperties(version) || changed;
        }

        private bool UpdateProperties(long version)
        {
            var changed = false;
            foreach (var property in Properties.Values)
                changed = property.Update(block) || changed;

            if (changed)
                Version = version;

            return changed;
        }

        // Applies model changes to game state, returns true if anything has changed
        public bool Apply()
        {
            var changed = false;

            var defaultName = block.DisplayNameText ?? block.DisplayName;
            if (Name != defaultName && Name != block.CustomName.ToString())
            {
                block.CustomName.Clear();
                block.CustomName.Append(Name);
                changed = true;
            }

            if (CustomData != block.CustomName.ToString())
            {
                block.CustomData = CustomData;
                changed = true;
            }

            foreach (var property in Properties.Values)
                changed = property.Apply(block) || changed;

            return changed;
        }

        public void SetName(string name)
        {
            Name = name;
            NotifyChange();
        }

        public void SetCustomData(string customData)
        {
            CustomData = customData;
            NotifyChange();
        }

        public void SetProperty(string propertyId, object? value)
        {
            if (!Properties.TryGetValue(propertyId, out var property))
                return;

            property.Value = value;
            NotifyChange();
        }

        private void NotifyChange()
        {
            terminalModel.NotifyUserModifiedBlock(EntityId);
        }
    }
}