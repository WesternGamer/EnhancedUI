using System;
using System.Collections.Generic;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI.Interfaces;

namespace EnhancedUI.ViewModel
{
    /* Block view model passed to JavaScript */
    public class BlockViewModel : IDisposable
    {
        // Owning block view model
        private readonly TerminalViewModel terminalModel;

        // Terminal block
        public readonly MyTerminalBlock Block;

        // Terminal property view models for the block
        private readonly Dictionary<string, PropertyViewModel> properties = new();

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
        public long EntityId => Block.EntityId;
        public override int GetHashCode() => EntityId.GetHashCode();

        // Block type
        public string ClassName => Block.GetType().Name;
        public string TypeId => Block.BlockDefinition.Id.TypeId.ToString();
        public string SubtypeName => Block.BlockDefinition.Id.SubtypeName;

        public BlockViewModel(TerminalViewModel terminalViewModel, MyTerminalBlock terminalBlock)
        {
            // Make the static code analysis happy
            Name = "";
            CustomData = "";
            DetailedInfo = "";

            Block = terminalBlock;
            terminalModel = terminalViewModel;

            UpdateFields();
            CreatePropertyModels();

            Block.PropertiesChanged += OnPropertyChanged;
        }

        private bool UpdateFields()
        {
            var changed = false;

            var isValid = !Block.Closed && Block.InScene && !Block.IsPreview;
            if (isValid != IsValid)
            {
                IsValid = isValid;
                changed = true;
            }

            var isFunctional = Block.IsFunctional;
            if (isFunctional != IsFunctional)
            {
                IsFunctional = isFunctional;
                changed = true;
            }

            var name = Block.CustomName.ToString();
            if (name == "")
                name = Block.DisplayNameText ?? Block.DisplayName;
            if (name != Name)
            {
                Name = name;
                changed = true;
            }

            if (CustomData != Block.CustomData)
            {
                CustomData = Block.CustomData;
                changed = true;
            }

            var detailedInfo = Block.DetailedInfo.ToString();
            if (detailedInfo != DetailedInfo)
            {
                DetailedInfo = detailedInfo;
                changed = true;
            }

            return changed;
        }

        private void CreatePropertyModels()
        {
            var terminalProperties = new List<ITerminalProperty>();
            Block.GetProperties(terminalProperties);
            foreach (var terminalProperty in terminalProperties)
            {
                properties[terminalProperty.Id] = new PropertyViewModel(this, terminalProperty);
            }
        }

        public void Dispose()
        {
            Block.PropertiesChanged -= OnPropertyChanged;

            properties.Clear();
        }

        private void OnPropertyChanged(MyTerminalBlock obj)
        {
            terminalModel.NotifyGameModifiedBlock(EntityId);
        }

        // Updates model from game state, returns true if anything has changed
        public bool Update()
        {
            var changed = UpdateFields();
            return UpdateProperties() || changed;
        }

        private bool UpdateProperties()
        {
            var changed = false;
            foreach (var property in properties.Values)
                changed = property.Update() || changed;

            return changed;
        }

        // Applies model changes to game state, returns true if anything has changed
        public bool Apply()
        {
            var changed = false;

            var defaultName = Block.DisplayNameText ?? Block.DisplayName;
            if (Name != defaultName && Name != Block.CustomName.ToString())
            {
                Block.CustomName.Clear();
                Block.CustomName.Append(Name);
                changed = true;
            }

            if (CustomData != Block.CustomName.ToString())
            {
                Block.CustomData = CustomData;
                changed = true;
            }

            foreach (var property in properties.Values)
                changed = property.Apply() || changed;

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
            if (!properties.TryGetValue(propertyId, out var property))
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