using System.Text;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI.Interfaces;
using VRageMath;

namespace EnhancedUI.ViewModel
{
    /* Property view model passed to JavaScript */
    public class PropertyViewModel
    {
        private readonly BlockViewModel blockModel;
        private readonly ITerminalProperty property;

        // ReSharper disable once MemberCanBePrivate.Global
        public string Id { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string TypeName { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        public object? Value { get; set; }

        public override int GetHashCode() => Id.GetHashCode();

        public PropertyViewModel(BlockViewModel blockViewModel, ITerminalProperty terminalProperty)
        {
            blockModel = blockViewModel;
            property = terminalProperty;

            Id = terminalProperty.Id;
            TypeName = terminalProperty.TypeName;

            Update();
        }

        /* Updates value from in-game property */
        public bool Update()
        {
            var value = Read(blockModel.Block, property);
            if (value == Value)
                return false;

            Value = value;
            return true;
        }

        /* Applies the value to the in-game property */
        public bool Apply()
        {
            return Write(blockModel.Block, property, Value);
        }

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

        private static bool Write(MyTerminalBlock block, ITerminalProperty property, object? value)
        {
            if (value == Read(block, property))
                return false;

            switch (property.TypeName)
            {
                case "Boolean":
                    property.AsBool().SetValue(block, value as bool? ?? property.AsBool().GetDefaultValue(block));
                    return true;

                case "Single":
                    property.AsFloat().SetValue(block, value as float? ?? property.AsFloat().GetDefaultValue(block));
                    return true;

                case "Int64":
                    property.As<long>().SetValue(block, value as long? ?? property.As<long>().GetDefaultValue(block));
                    return true;

                case "StringBuilder":
                    property.As<StringBuilder>().SetValue(block,
                        value == null
                            ? property.As<StringBuilder>().GetDefaultValue(block)
                            : new StringBuilder(value as string ?? ""));
                    return true;

                case "Color":
                    property.As<Color>().SetValue(block,
                        value == null
                            ? property.As<Color>().GetDefaultValue(block)
                            : ParseColor(value as string ?? ""));
                    return true;
            }

            return false;
        }

        private static string FormatColor(Color c)
        {
            return $"#{c.R:X02}{c.G:X02}{c.B:X02}";
        }

        private static Color ParseColor(string c)
        {
            if (c.Length != 7 || !c.StartsWith("#"))
                return Color.Black;

            var r = int.Parse(c.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            var g = int.Parse(c.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            var b = int.Parse(c.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color(r, g, b);
        }
    }
}