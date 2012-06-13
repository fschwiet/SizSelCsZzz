﻿using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using SizSelCsZzz.Other;

namespace SizSelCsZzz
{
    public class BySizzle : ByExternalScript
    {
        public static By CssSelector(string selector)
        {
            return new BySizzle()
            {
                _selector = Newtonsoft.Json.JsonConvert.SerializeObject(selector),
                _javascriptGlobal = "Sizzle",
                _resourceLocation = "sizzleSource.sizzle.js"
            };
        }
    }
}
