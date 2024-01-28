using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IceAge
{
    public static class IceAgeHelper
    {
        public const string UsernameRegex = @"@?\b([A-Z0-9._%+-]+)@([A-Z0-9.-]+\.[A-Z]{2,})\b";
        public const RegexOptions UsernameRegexOptions = RegexOptions.IgnoreCase;
    }
}
