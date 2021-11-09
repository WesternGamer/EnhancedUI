using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI.Interfaces;
using System.Text;
using VRageMath;

namespace EnhancedUI.ViewModel
{
    /// <summary>
    /// Property view model passed to JavaScript
    /// </summary>
    public class PropertyViewModel
    {
        private readonly ITerminalProperty property;

        // ReSharper disable once MemberCanBePrivate.Global
        public object? Value { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public string Id => property.Id;

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string TypeName => property.TypeName;

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public PropertyViewModel(MyTerminalBlock block, ITerminalProperty terminalProperty)
        {
            property = terminalProperty;
            Update(block);
        }

        /// <summary>
        /// Updates value from in-game property.
        /// </summary>
        /// <param name="block">Block to update the value from in-game property.</param>
        /// <returns></returns>
        public bool Update(MyTerminalBlock block)
        {
            object? value = Read(block, property);
            if (value == Value)
            {
                return false;
            }

            Value = value;
            return true;
        }

        /// <summary>
        /// Applies the value to the in-game property of a block.
        /// </summary>
        /// <param name="block">Block to apply the in-game property to.</param>
        public void Apply(MyTerminalBlock block)
        {
            Write(block, property, Value);
        }

        /// <summary>
        /// Reads the property of a block.
        /// </summary>
        /// <param name="block">The block to read the property from.</param>
        /// <param name="property">The property to read the data from.</param>
        /// <returns></returns>
        private static object? Read(MyTerminalBlock block, ITerminalProperty property)
        {
            switch (property.TypeName)
            {
                case "Boolean":
                    return property.AsBool().GetValue(block);

                case "Single":
                    return property.AsFloat().GetValue(block);

                case "Int64":
                    return property.As<long>().GetValue(block);

                case "StringBuilder":
                    return property.As<StringBuilder>().GetValue(block).ToString();

                case "Color":
                    return FormatColor(property.As<Color>().GetValue(block));
            }

            return null;
        }

        /// <summary>
        /// Writes the property of a block.
        /// </summary>
        /// <param name="block">Block to write the property to.</param>
        /// <param name="property">Property to write the data to.</param>
        /// <param name="value">Data to write to the property.</param>
        private static void Write(MyTerminalBlock block, ITerminalProperty property, object? value)
        {
            object? currentValue = Read(block, property);
            if (value == currentValue)
            {
                return;
            }

            switch (property.TypeName)
            {
                case "Boolean":
                    property.AsBool().SetValue(block, value as bool? ?? property.AsBool().GetDefaultValue(block));
                    break;

                case "Single":
                    property.AsFloat().SetValue(block, value as float? ?? property.AsFloat().GetDefaultValue(block));
                    break;

                case "Int64":
                    property.As<long>().SetValue(block, value as long? ?? property.As<long>().GetDefaultValue(block));
                    break;

                case "StringBuilder":
                    property.As<StringBuilder>().SetValue(block,
                        value == null
                            ? property.As<StringBuilder>().GetDefaultValue(block)
                            : new StringBuilder(value as string ?? ""));
                    break;

                case "Color":
                    property.As<Color>().SetValue(block,
                        value == null
                            ? property.As<Color>().GetDefaultValue(block)
                            : ParseColor(value as string ?? ""));
                    break;
            }
        }

        private static string FormatColor(Color c)
        {
            return $"#{c.R:X02}{c.G:X02}{c.B:X02}";
        }

        private static Color ParseColor(string c)
        {
            if (c.Length != 7 || !c.StartsWith("#"))
            {
                return Color.Black;
            }

            int r = int.Parse(c.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            int g = int.Parse(c.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            int b = int.Parse(c.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color(r, g, b);
        }
    }
}