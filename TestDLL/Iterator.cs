using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDLL {
    class Iterator {
        public IEnumerable<int> Test1() {
            try {
                yield return 1;
            } finally {
                Console.WriteLine("ss");
            }
            yield return 1;

            // 生成迭代器类，如：TestDLL.Iterator/'<Test1>d__0'
            // 如果 try finally 的 try 内部没有使用 yield return，Dispose 方法直接返回什么也不做
            // 如果有，迭代器类生成 <>m__Finally1 方法，该方法执行 finally 的代码，Dispose 会调用 <>m__Finally1 方法
            // IEnumerator.Reset 接口方法的实现是直接抛出异常：System.NotSupportedException
        }

        public IEnumerable<int> Test2(int num) {
            yield return 1 + num;
        }

        public void Test2_1() {
            var ie = Test2(100);    //  :call  ...  TestDLL.Iterator::Test2(int32)
            var temp = 1000;
            foreach (var i in ie)
            // :callvirt  ...  [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
            // MoveNext()
            // i = Current
            {

            }
        }
    }
}
