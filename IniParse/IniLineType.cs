using System;
using System.Collections.Generic;
using System.Text;

namespace IniParse
{
    public enum IniLineType
    {
        Unknown,
        Property,
        Section,
        Comment,
        Empty
    }
}
