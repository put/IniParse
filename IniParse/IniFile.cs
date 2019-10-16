using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

namespace IniParse
{
    public class IniFile
    {
        public static ExpandoObject Parse(string path)
        {
            dynamic iniFile = new ExpandoObject();

            using var fs = File.OpenRead(path);
            using var reader = new StreamReader(fs);
            string line;
            string lastSectionName = null;

            while ((line = reader.ReadLine()) != null)
            {
                IniLineType type = IniParseHelper.GetLineType(line);

                switch (type)
                {
                    case IniLineType.Section:
                        {
                            string sectionName = IniParseHelper.GetSectionName(line);
                            lastSectionName = sectionName;
                            ((IDictionary<string, object>)iniFile).Add(sectionName, new ExpandoObject());
                            break;
                        }
                    case IniLineType.Property:
                        {
                            var propertyDetails = IniParseHelper.GetProperty(line);
                            if (lastSectionName != null)
                                ((IDictionary<string, object>)((IDictionary<string, object>)iniFile)[lastSectionName]).Add(propertyDetails.Name, propertyDetails.Value);
                            else ((IDictionary<string, object>)iniFile).Add(propertyDetails.Name, propertyDetails.Value);
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
