using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace SizSelCsZzz
{
    public static class JQueryAjaxWaiter
    {
        public static bool IsAjaxPending(this IWebDriver webDriver)
        {
            return false;
        }
    }
}
