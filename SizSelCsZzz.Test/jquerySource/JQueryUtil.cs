using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.Test.jquerySource
{
    public class JQueryUtil
    {
        public static string GetJQuerySource()
        {
            return ResourceLoader.LoadResourceRelativeToType(typeof(JQueryUtil), "jquery-1.6.2.js");
        }

        public static string HtmlLoadingJQuery(string location, string content = "")
        {
            return @"<html>
<script src='" + location + @"'></script>
<body>
" + content + @"
</body>
</html>";
        }
    }
}
