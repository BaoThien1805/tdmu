using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WEBXEMPHIMHKTMOVIE.Helpers
{
    public class PayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return CompareInfo.GetCompareInfo("en-US")
                .Compare(x, y, CompareOptions.Ordinal);
        }
    }
}