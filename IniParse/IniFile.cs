using System;
using System.IO;
using System.Dynamic;
using System.Collections.Generic;

namespace IniParse
{
    public static class IniFile
    {
        public static ExpandoObject Parse(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new StreamReader(fs);

            string line;
            string lastSectionName = null;

            var iniFile = new ExpandoObject();
            var iniObjects = (IDictionary<string, object>)iniFile;

            while ((line = reader.ReadLine()) != null)
            {
                IniLineType type = IniParseHelper.GetLineType(line);

                switch (type)
                {
                    case IniLineType.Section:
                        {
                            string sectionName = IniParseHelper.GetSectionName(line);
                            lastSectionName = sectionName;
                            iniObjects.Add(sectionName, new ExpandoObject());
                            break;
                        }
                    case IniLineType.Property:
                        {
                            (string name, object value) = IniParseHelper.GetProperty(line);
                            if (lastSectionName != null)
                                ((IDictionary<string, object>)iniObjects[lastSectionName]).Add(name, value);
                            else iniObjects.Add(name, value);
                            break;
                        }
                    case IniLineType.Comment:
                    case IniLineType.Empty:
                    case IniLineType.Unknown:
                        continue;
                }
            }
            return iniFile;
        }
    }
}
