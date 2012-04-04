using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;

namespace SizSelCsZzz.Other
{
    class DriverWrapperUnwrapper
    {
        public static T GetDriverImplementationOf<T>(ISearchContext context) where T : class
        {
            var scriptExecutor = context as T;

            if (scriptExecutor == null && context is IWrapsDriver)
            {
                scriptExecutor = (context as IWrapsDriver).WrappedDriver as T;
            }
            return scriptExecutor;
        }
    }
}
