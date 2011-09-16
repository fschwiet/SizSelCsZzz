using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.jquerySource
{
    public class JQuerySource
    {
        public static string GetJQuerySource()
        {
            return ResourceLoader.LoadResourceRelativeToType(typeof(JQuerySource), "jquery-1.6.2.js");
        }
    }
}
