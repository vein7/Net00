using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDLL {
    public class BiBc {
        int Count;
        DateTime CTime;

        public int Width { get; set; }

        public Func<int, int> JuBuBmLl() {
            var num = 100 + Count;
            return (n) => num + n;

            // num 没有分配在函数栈上，编译器会为匿名函数生成一个类，num 是该类的一个 field
            // 该类嵌入在在当前类中，如：TestDLL.BiBc/'<>c__DisplayClass6_0'
            // 函数开始执行时， newobj    instance void TestDLL.BiBc/'<>c__DisplayClass6_0'::.ctor()
        }

        public Func<int, int> LwIgYr() {
            return (n) => Count + Width + 100;

            // 当闭包仅仅捕获的是类的成员时，编译器会为匿名函数生成一个函数，挂在当前类中，如：TestDLL.BiBc::'<LwIgYr>b__7_0'
        }

        public Func<int, int> NiMkHjUu() {
            Func<int, string> fun2 = (n) => n.ToString() + ".00";
            return (n) => n + 100;

            // 没有使用闭包的匿名函数，会生成一个类（？静态类），如：TestDLL.BiBc/'<>c'
            // 当前 BiBc 类中，所有不使用闭包的匿名函数，都会生成相应的的函数在该类中
            // NiMkHjUu2() 里面的匿名函数，也是生成在该类中
        }

        public Func<int, int> NiMkHjUu2() {
            Func<int, string> fun2 = (n) => n.ToString() + ".0000";
            return (n) => n + 10000;
        }

        public Func<int, int> JuBuBmLl2() {
            var num = 100 + Count;
            return (n) => num + n;

            // 闭包捕获了局部变量，会再生成一个类，也就是 JuBuBmLl 和 JuBuBmLl2，不是使用同一个类
        }

        public List<Action> Foreach1() {
            var strs = new[] { "12", "33", "aa" };
            var fnLs = new List<Action>();
            foreach (var s in strs) {
                fnLs.Add(() => Console.WriteLine(s));       // C# 5， 每个闭包都是不同的引用
            }
            return fnLs;

            // 每个循环都会 new 一个匿名函数的那个类
        }

        public void GsXlBmLl() {
            int x = 1, y = 2, z = 3;
            Func<int> act1 = () => x + y;
            Func<int> act2 = () => x + z;

            // 两个匿名函数都在同一个类，这个类有三个成员x, y, z
        }


        public void For1() {
            int x = 0;
            var ls = new Func<int>[2];
            for (int i = 0; i < 2; i++) {
                int inside = 0;
                ls[i] = () => x + inside;
            }

            // 两个匿名函数，x 引用是两个匿名函数的同一个引用，inside 是两个匿名函数不同的引用，
            // 生成了两个类，其中一个类（C1）只捕获了 x，另一个类（C2）捕获了 inside，
            // 进入方法会 new 一个 C1，每次 for 里面会 new C2
        }

        public void GsXlBmLl2() {
            var ls = new Func<int>[3];
            int x = 0;
            {
                int y = 10;
                ls[0] = () => x + y;
            }
            {
                int y = 100;
                ls[1] = () => x + y;
            }
            {
                int z = 100;
                ls[2] = () => x + z;
            }
            // 生成了四个类
        }

        public Func<int> CjUu(int a, ref int b) {
            // Func<int> fn1 = () => a + b;     // 不能捕获 ref
            Func<int> fn2 = () => a + 10;
            return fn2;

            // int LocatFn() => a + b;          // 本地函数也不能使用
        }

        public void DiGv1() {
            Action<int> act = null;
            act = (n) => {
                if (n > 10 || n < 0) { return; }
                act(n--);
            };
            act(5);
            // 
        }

        public void FjXk1<T>() {
            var c1 = new Cell<T>();
            Action act1 = () => c1.Value.ToString();
        }

        public void FjXk2<T>(T t) {
            Action act1 = () => t.ToString();
        }

        class Cell<T> { public T Value; }
    }
}
