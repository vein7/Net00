using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd {
    class Linq {

        public Linq() {
            //GetEnumerator();
            

        }
        
        public Linq VlUiVe() {
            var arr = new[] { 1, 2, 3, 4 };
            arr.Where0(i => i > 2)
                .Where0(i => i > 3)
                .ToArray();
            return this;
        }

        class Itor : IEnumerable<int> {

            public void Test() {
                foreach(var i in this) {

                }
            }

            public IEnumerator<int> GetEnumerator() {
                yield return 1;
            }

            IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        }
    }

    class LTest {
        static IEnumerable<int> Test1() {
            Console.WriteLine("我进来了");
            yield return -2;
            Console.WriteLine("我又进来了");
            yield return -1;
            Console.WriteLine("我又进来了");
            var arr = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach(var i in arr) {
                if(i > 5) { yield break; }
                yield return i; 
            }
        }

        public LTest() {
            var ls = Test1();       // Test1() 函数执行了吗?
            Console.WriteLine("你进去了吗?");
            foreach (var item in ls) {
                Console.WriteLine(item);
            }
            Console.WriteLine("滚");
        }

        public void Test2() {
            var ls = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var t1 = Take(ls, 5);      // Take 函数执行了吗
            var t2 = Take(t1, 3);       // 这次又有执行Take吗?
            foreach(var item in t2) {
                Console.WriteLine(item);
            }
            ls.RemoveRange(0, 5);
            // 这次再次遍历迭代器，执行了 Take(ls, 5);  ls 数据源已经变了
            foreach (var item in t1) {
                Console.WriteLine(item);
            }
        }

        static IEnumerable<T> Take<T>(IEnumerable<T> ls, int count) {
            Console.WriteLine("Take: " + count);
            foreach(var item in ls) {
                if( count-- <= 0 ) { yield break; }
                yield return item;
            }
        }

    }

    static class ExFn {
        public static IEnumerable<T> Skip2<T>(this IEnumerable<T> ls, int count) {
            var n = 0;
            foreach(var item in ls) {
                if(++n > count) {
                    yield return item;
                }
            }
        }


    }

}
