using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SizSelCsZzz.Extras;

namespace SizSelCsZzz.Test.jquerySource
{
    public class JQueryUtil
    {
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
