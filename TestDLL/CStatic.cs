using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDLL {
    public static class CStatic {
        static readonly int Count = 100;

        static int SFn1() {
            return 100 + Count;
        }
        static dynamic SFn2() {
            var obj1 = new { A = 3, B = 3, C = 4 };
            return new { A = 3, B = 4 };
        }
    }
}
