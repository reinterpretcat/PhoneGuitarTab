using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.UI.Bootstrap
{
    using PhoneGuitarTab.Core.Bootstrap;

    public class TabBootstraperPlugin : IBootstrapperPlugin
    {
        public string Name { get { return "Tab"; } }

        public bool Run()
        {
            return true;
        }

        public bool Update()
        {
            return true;
        }
    }
}
