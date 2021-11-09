using EnhancedUI.Utils;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using VRage.Utils;

namespace EnhancedUI.ViewModel
{
    /// <summary>
    /// Block view model passed to JavaScript
    /// </summary>
    public class BlockViewModel : IDisposable
    {
        private static long nextId;

        /// <summary>
        /// Owning block view model.
        /// </summary>
        private readonly TerminalViewModel terminalModel;

        /// <summary>
        /// Terminal block.
        /// </summary>
        private readonly MyTerminalBlock block;

        /// <summary>
        /// Identification.
        /// </summary>
        public long Id { get; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// State version.
        /// </summary>
        public long Version { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Terminal property view models for the block.
        /// </summary>
        public readonly Dictionary<string, PropertyViewModel> Properties = new();

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// True as long as the block is valid (not closed); set to false if a block is destroyed
        /// </summary>
        public bool IsValid { get; private set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// True if the block is functional.
        /// </summary>
        public bool IsFunctional { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Name of the block (user editable).
        /// </summary>
        public string Name { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Custom data (user editable).
        /// </summary>
        public string CustomData { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Detailed information may be provided by some of the blocks (read-only).
        /// </summary>
        public string DetailedInfo { get; private set; }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Block type.
        /// </summary>
        public string ClassName => block.GetType().Name;

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string TypeId => block.BlockDefinition.Id.TypeId.ToString();

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public string SubtypeName => block.BlockDefinition.Id.SubtypeName;

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Geometry.
        /// </summary>
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

        /// <summary>
        /// Updates the valid state, functional state, name, custom data, and detailed info.
        /// </summary>
        /// <param name="changed">Is block fields changed already?</param>
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

        /// <summary>
        /// Updates model from game state.
        /// </summary>
        public void Update()
        {
            UpdateFields();
            UpdateProperties();
        }

        /// <summary>
        /// Updates the properties of a block.
        /// </summary>
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

        // 
        /// <summary>
        /// Applies model changes to game state.
        /// </summary>
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

        /// <summary>
        /// Sets the name of a block.
        /// </summary>
        /// <param name="name">The name of the block.</param>
        public void SetName(string name)
        {
            Name = name;
            NotifyChange();
        }

        /// <summary>
        /// Sets custom data of a block.
        /// </summary>
        /// <param name="customData">The custom data.</param>
        public void SetCustomData(string customData)
        {
            CustomData = customData;
            NotifyChange();
        }

        /// <summary>
        /// Set a property of a block.
        /// </summary>
        /// <param name="propertyId">ID of property.</param>
        /// <param name="value">Data the set the property to.</param>
        public void SetProperty(string propertyId, object? value)
        {
            if (!Properties.TryGetValue(propertyId, out PropertyViewModel? property))
            {
                return;
            }

            property.Value = value;
            NotifyChange();
        }

        /// <summary>
        /// Notifies that a property was changed.
        /// </summary>
        private void NotifyChange()
        {
            Version = terminalModel.GetNextVersion();
            terminalModel.NotifyUserModifiedBlock(Id);
        }
    }
}