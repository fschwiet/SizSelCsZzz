using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;

namespace SizSelCsZzz.Test
{
    public class can_use_Sizzle_css_selectors : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("a server serving a simple page", delegate
            {
                when("we try to find an element using a Sizzler specific selector", delegate
                {
                    then("it works");
                });
            });
        }
    }
}
