using System;
using System.Collections.Generic;
using System.Text;

namespace IniParse
{
    internal class IniParseHelper
    {
        public static IniLineType GetLineType(string line)
        {
            if (line.Length == 0)
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
            int delimiterIndex = line.IndexOf('=');

            if (delimiterIndex < 1)
                throw new FormatException("Line is not a valid INI property");

            while (line[delimiterIndex - 1]  == ' ')
            {
                if (delimiterIndex - 1 >= 0)
                {
                    line = line.Remove(delimiterIndex - 1, 1);
                    delimiterIndex--;
                }
                else throw new FormatException("Line is not a valid INI property");
            }

            while (delimiterIndex + 1 < line.Length && line[delimiterIndex + 1] == ' ')
            {
                if (delimiterIndex + 1 == line.Length)
                    break;
                line = line.Remove(delimiterIndex + 1, 1);
            }

            string[] split = line.Split('=');

            string propName = split[0];
            dynamic propValue = split[1];

            if (split.Length > 2)
                propValue = string.Join(string.Empty, split[1..^1]);

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
            if (double.TryParse(value, out double doubleResult))
                return doubleResult;
            if (value.ToLower()[^1] == 'f' && float.TryParse(value[..^1], out float forcedFloatResult))
                return forcedFloatResult;
            if (value.ToLower()[^1] == 'd' && double.TryParse(value[..^1], out double forcedDoubleResult))
                return forcedDoubleResult;
            if (value.ToLower() == "true")
                return true;
            if (value.ToLower() == "false")
                return false;
            else return value;

        }
        
    }
}
