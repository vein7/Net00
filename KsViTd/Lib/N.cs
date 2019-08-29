using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd.Lib {
    struct N<T> where T : struct {
        public readonly T Value;
        private bool hasValue;
        public N(T val) {
            Value = val;
            hasValue = true;
        }

        public static implicit operator bool(N<T> val) => val.hasValue;
        public static implicit operator T(N<T> val) => val.Value;
        public static implicit operator N<T>(T val) => new N<T>(val);


        public static void Test() {
            N<int> a;
            
        }
    }
}
