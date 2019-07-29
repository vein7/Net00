using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KsViTd.Base;

namespace KsViTd {

    struct Kv<TKey, TValue> {
        public TKey Key;
        public TValue Value;

        public Kv(TKey key, TValue value) {
            Key = key;
            Value = value;
        }
    }

    //struct Nullb<T> where T : struct {
    //    bool isNull;
    //    T value;

    //    //public T Value => value;
    //    public Nullb(T v);

    //    public static implicit operator Nullb<T>(T value) => new Nullb<T> { value = value };
    //}
    //static class NullbEFun {
    //    static T Value<T>(this Nullb<T> ins) {
    //        return ins == null ? default(T) : (T)ins;
    //    }
    //}

    class Program {



        static void Main(string[] args) {
            //var tester = new DoXmIg().Parallel4();
            //DoXmIg.Task11();
            //EnumEF.Test();

            //new FjUe();
            //new Linq();
            //StructTest1();
            //Test();
            //new CsLang().FnBiBc1();
            //new DynamicTest().Test();
            //var a = IEnumTest();

            //没什么
            //KsViTd.Excel.ExcelWriter.Test();

            //Primitive<int>.TryParse("333", out var intA);
            //Primitive<long>.TryParse("333", out var longB);


            //new LTest().Test2();
            Console.ReadKey();
            return;
            Action<int> fun1 = (o) => Console.Write(o.ToString() + " ");
            fun1 += (o) => Console.WriteLine(o);

            fun1(2);
            //var fil = new Filter<StatisticalBase>();
            //var rep= new DReport().SqlQurey(fil);

            //new CsLang();

            C2 objC2 = "33";
            objC2 = "44";
            


            var m = typeof(C1).GetMember("IntB");

            Expression<Func<C1, int>> expr = c => c.IntB;
            var pInstance = Expression.Parameter(typeof(C1), "instance");
            var exprPro = Expression.PropertyOrField(pInstance, "IntB");
            var lam = Expression.Lambda<Func<C1, int>>(exprPro, pInstance);

            var members = typeof(C1).GetMembers();

            Console.ReadKey();


        }
        class A {
            int a = 3;
            public A() {
                PrintFields();
            }
            public virtual void PrintFields() { }
        }
        class B : A {
            int x = 1;
            int y;
            public B() {
                y = -1;
            }
            public override void PrintFields() {
                Console.WriteLine("x={0},y={1}", x, y);
            }
        }

        static IEnumerable<int> IEnumTest() {
            Console.WriteLine("IEnumTest");
            return new[] { 1, 3, 3 };
        }
        static int[] Calc() {
            var ls = new[] { 1, 2 }; 
            try {
                return ls;
            } finally {
                Console.WriteLine("finally");
                ls[1] = 33;
            }
        }
        static ref int Calc0() {
            var ls = new[] { 1, 2 }; 
            try {
                 return ref ls[1];
            } finally {
                Console.WriteLine("finally");
                ls[1] = 33;
            }
        }



        static async void Test() {
            Console.WriteLine("Async start");
            var s2 = GetAsync();
            for (var i = 0; i < 10; ++i) {
                Thread.Sleep(300);
                Console.Write(".");
            }
            var result = await s2;
            Console.WriteLine("\nResult: " + result);

        }

        static Task<string> GetAsync() {
            return Task.Run(() => {
                Thread.Sleep(2000);
                Console.WriteLine("完成");
                return "222222222";
            });

            //Thread.Sleep(2000);
            //Console.WriteLine("完成");
            //return "222222222";
        }


        static Action<int> Action1() {
            return (i) => ++i;
        }

        static void Fun1(int i) {
            int? a = null;
            int b = (int)a;

            int? c = b;
        }
        static void Fun1(int? i) {

        }


        #region Class
        class C1 {
            public Kv<int, int> Kv = new Kv<int, int>(3, 4);
            public int IntA;
            public int IntB { get; set; }



            public Kv<int, int> GetKv() {

                return Kv;
            }
        }

        class C2 : IDisposable {
            public string Name = "";

            public void Dispose() {
                Name = null;
            }

            public static implicit operator C2(string name) => new C2 { Name = name };
            public static implicit operator string(C2 c) => c.Name;
        }



        #endregion
    }
}
