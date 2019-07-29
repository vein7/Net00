using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDLL {
    class Iterator {
        public IEnumerable<int> Test1() {
            yield return 1;
        }
    }
}
