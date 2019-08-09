using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDLL {
    enum En1 {
        A,
        B
    }
    struct En2 {
        public int X;
        public int Y;
    }
    class Enums {
        int Count;
        static void Test1() {
            En1.A.ToString();
            // En1.A 是值类型，toString 的实现可以输出成员名称

        }
    }
}
