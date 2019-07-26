using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDLL {
    static class CStatic {
        static readonly int Count = 100;

        static int SFn1() {
            return 100 + Count;
        }
    }
}
