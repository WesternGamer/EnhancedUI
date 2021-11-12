using EnhancedUI.Utils;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using VRage.Utils;

namespace EnhancedUI.ViewModel
{
    // Block view model passed to JavaScript
    public class BlockViewModel : IDisposable
    {
        private static long nextId;

        // Owning block view model
        private readonly TerminalViewModel terminalModel;

        // Terminal block
        private readonly MyTerminalBlock block;

        // Identification
        public long Id { get; }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        // State version
        // ReSharper disable once MemberCanBePrivate.Global
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

        // Block type
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string ClassName => block.GetType().Name;

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string TypeId => block.BlockDefinition.Id.TypeId.ToString();

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string SubtypeName => block.BlockDefinition.Id.SubtypeName;

        // Geometry
        // ReSharper disable once UnusedMember.Global
        public int[] Position => block.Position.ToArray();

        // ReSharper disable once UnusedMember.Global
        public int[] Size => block.BlockDefinition.Size.ToArray();

        public override string ToString()
        {
            return $"BlockViewModel #{Id} at {block.Position} is {SubtypeName} \"{Name}\"";
        }

#pragma warning disable 8618
        public BlockViewModel(TerminalViewModel terminalViewModel, MyTerminalBlock terminalBlock, long version)
        {
            Id = Interlocked.Increment(ref nextId);

            terminalModel = terminalViewModel;
            block = terminalBlock;
            Version = version;

            MyLog.Default.Info($"EnhancedUI: {this}");

            UpdateFields(true);
            CreatePropertyModels();

            block.PropertiesChanged += OnPropertyChanged;
        }
#pragma warning restore 8618

        private void UpdateFields(bool changed = false)
        {
            bool isValid = !block.Closed && block.InScene && !block.IsPreview;
            if (isValid != IsValid)
            {
                IsValid = isValid;
                changed = true;
            }

            bool isFunctional = block.IsFunctional;
            if (isFunctional != IsFunctional)
            {
                IsFunctional = isFunctional;
                changed = true;
            }

            string? name = block.CustomName.ToString();
            if (name == "")
            {
                name = block.DisplayNameText ?? block.DisplayName;
            }

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

            string? detailedInfo = block.DetailedInfo.ToString();
            if (detailedInfo != DetailedInfo)
            {
                DetailedInfo = detailedInfo;
                changed = true;
            }

            if (changed)
            {
                Version = terminalModel.GetNextVersion();
            }
        }

        private void CreatePropertyModels()
        {
            List<ITerminalProperty>? terminalProperties = new List<ITerminalProperty>();
            block.GetProperties(terminalProperties);
            foreach (ITerminalProperty? terminalProperty in terminalProperties)
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
            terminalModel.NotifyGameModifiedBlock(Id);
        }

        // Updates model from game state, returns true if anything has changed
        public void Update()
        {
            UpdateFields();
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            bool changed = false;
            foreach (PropertyViewModel? property in Properties.Values)
            {
                changed = property.Update(block) || changed;
            }

            if (changed)
            {
                Version = terminalModel.GetNextVersion();
            }
        }

        // Applies model changes to game state, returns true if anything has changed
        public void Apply()
        {
            string? defaultName = block.DisplayNameText ?? block.DisplayName;
            if (Name != defaultName && Name != block.CustomName.ToString())
            {
                block.CustomName.Clear();
                block.CustomName.Append(Name);
            }

            if (CustomData != block.CustomData.ToString())
            {
                block.CustomData = CustomData;
            }

            foreach (PropertyViewModel? property in Properties.Values)
            {
                property.Apply(block);
            }
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
            if (!Properties.TryGetValue(propertyId, out PropertyViewModel? property))
            {
                return;
            }

            property.Value = value;
            NotifyChange();
        }

        private void NotifyChange()
        {
            Version = terminalModel.GetNextVersion();
            terminalModel.NotifyUserModifiedBlock(Id);
        }
    }
}