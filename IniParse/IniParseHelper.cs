using System;
using System.Globalization;

namespace IniParse
{
    internal class IniParseHelper
    {
        public static IniLineType GetLineType(string line)
        {
            if (string.IsNullOrEmpty(line))
                return IniLineType.Empty;

            if (line[0] == ';')
                return IniLineType.Comment;

            if (line.Contains('='))
                return IniLineType.Property;

            if (line[0] == '[' && line[^1] == ']')
                return IniLineType.Section;

            return IniLineType.Unknown;
        }

        public static string GetSectionName(string line)
            => line[1..^1];

        public static (string Name, object Value) GetProperty(string line)
        {
            if (!line.Contains('='))
                throw new FormatException("Line is not a valid INI property");

            int assignmentIndex = line.IndexOf('=');
            string propName = line[..assignmentIndex].Trim();
            dynamic propValue = line[(assignmentIndex + 1)..].Trim();

            if (propValue.Length > 0)
            {
                if (((string)propValue)[0] == '"' && ((string)propValue)[^1] == '"')
                    propValue = ((string)propValue)[1..^1];
                else propValue = GetObjectFromPropertyValue(propValue);
            }

            return (propName, propValue);
        }

        private static dynamic GetObjectFromPropertyValue(string value)
        {
            if (int.TryParse(value, out int intResult))
                return intResult;
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleResult))
                return doubleResult;
            if (value.ToLower()[^1] == 'f' && float.TryParse(value[..^1], NumberStyles.Any, CultureInfo.InvariantCulture, out float forcedFloatResult))
                return forcedFloatResult;
            if (value.ToLower()[^1] == 'd' && double.TryParse(value[..^1], NumberStyles.Any, CultureInfo.InvariantCulture, out double forcedDoubleResult))
                return forcedDoubleResult;
            if (bool.TryParse(value, out bool boolResult))
                return boolResult;
            return value;
        }
    }
}