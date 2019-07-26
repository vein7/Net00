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
            // 该类嵌入在在当前类中，如：TestDLL.BiBc/'<>c__DisplayClass6_0'::'<JuBuBmLl>b__0'
            // 函数开始执行时， newobj    instance void TestDLL.BiBc/'<>c__DisplayClass6_0'::.ctor()
        }

        public Func<int,int> LwIgYr() {
            return (n) => Count + Width + 100;

            // 当闭包仅仅捕获的是类的成员时，编译器会为匿名函数生成一个函数，挂在当前类中，如：TestDLL.BiBc::'<LwIgYr>b__7_0'
        }

        public Func<int,int> NiMkHjUu() {
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
            int outside = 0;
            var ls = new Func<int>[2];
            for (int i = 0; i < 2; i++) {
                int inside = 0;
                ls[i] = () => outside + inside;
            }

        }

    }
}
