using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDLL {
    public static class CStatic {
        static readonly int Count = 100;

        public static int NumA => Count + 20;       // 好像仅仅只是一个方法，并没有生成一个 field 出来

        public static int NumB { get; }             // 这里会生成，

        static int SFn1() {
            return 100 + Count;
        }
        static dynamic SFn2() {
            var obj1 = new { A = 3, B = 3, C = 4 };
            return new { A = 3, B = 4 };
        }
    }
}
