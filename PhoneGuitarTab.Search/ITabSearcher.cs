using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneGuitarTab.Search
{
    public interface ITabSearcher
    {
        void Run(int pageNumber, TabulatureType type);
    }
}
