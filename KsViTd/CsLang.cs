﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd {
    class CsLang {

        public class ClassIniA {
            public static int a = 1;
            public int b;
            public static ClassIniB B;
            static ClassIniA() {
                Console.WriteLine("class ini ClassIniA");
                B = new ClassIniB(); // 此时不会第二次进入构造函数
                Console.WriteLine("B.b=" + B.b); // B 还是没有执行完构造函数
                Console.WriteLine("ClassIniB.a=" + ClassIniB.a); // 这个已经执行了
                Console.WriteLine("ClassIniB.c=" + ClassIniB.c); // 这个没有执行
            }
        }

        public class ClassIniB {
            public static int a = 10;
            public int b = ClassIniB.c + 100;
            public static ClassIniA A = new ClassIniA();// 编译后会添加在静态构造函数的前面，并执行
            public static int c = 20;
            static ClassIniB() {
                // 初始化静态字段，所以就去调用 ClassIniA 的静态构造函数
                Console.WriteLine("class ini ClassIniB");
                ClassIniB.a += 20;
                c = 33;
            }
        }

        public CsLang() {
            //TestDynamic();

            var a = new XYZ(3);

        }

        public CsLang TestDynamic() {
            var ls = new[] {
                new Dnm{ Name="dd", Dnm1 = 1 },
                new Dnm{ Name="d2", Dnm1 = DateTime.Now} ,
                new Dnm{ Name="d3", Dnm1 = "str" },
                new Dnm{ Name="d3", Dnm1 = 2D }
            };

            Console.WriteLine(sizeof(int));
            foreach (var d in ls) {
                Console.WriteLine($"{d.Name}\t{d.Dnm1.ToString()}\t{d.Dnm1.GetType()}");
            }

            string a = "";
            object b = a;
            F1(out b);

            return this;
        }

        public CsLang TestException1() {
            // 下一层Catch 是否能捕获上一层 Catch
            try {
                throw new ArgumentNullException("ArgumentNullException");
            } catch (ArgumentNullException ex) {
                Console.WriteLine(ex.Message);
                // 这里抛出的异常不会被下一层的捕获到
                throw new NullReferenceException("NullReferenceException");
            } catch (NullReferenceException ex) {
                Console.WriteLine(ex.Message);
            }
            return this;
        }

        public void F1(out object obj) {
            obj = new List<int>();
        }

        public void FunTest() {
            Func<object, object> f;
            Func<string, string> f2 = (string s) => s;
            f = (obj) => f2(obj as string);        //obj 不一定是string 类型

            foreach (Func<object, object> fn in f.GetInvocationList()) {

            }

            var f3 = Fn((int n1, int n2) => n1 * n2);
            //MulticastDelegate.Combine
            //Delegate.Remove()
        }

        public Action<T, T2> Act<T, T2>(Action<T, T2> act) => act;

        public Func<T, T2, Tr> Fn<T, T2, Tr>(Func<T, T2, Tr> fn) => fn;


        class Dnm {
            public string Name;
            public dynamic Dnm1;        // object 变量
        }

        class FnList : IEnumerable<Func<string>> {
            List<Func<string>> ls = new List<Func<string>>();
            public IEnumerator<Func<string>> GetEnumerator() {
                return ls.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return ls.GetEnumerator();
            }
            public FnList Add(Func<string> fn) {
                return this;
            }

            //public void Add(int i) { }

            public void Test() {
                var ls = new List<int> {
                    3,
                    3
                };

                //var ls2 = new JiHe() {
                //    2,3
                //};
                var ls3 = new List<Func<string>> {
                    F1,F2
                };
                var ls4 = new Func<string>[] {
                    F1,F2
                };

                var ls5 = new FnList { F1, F2,
                    ()=> "dd"
                };


            }

            public string F1() => "";
            public string F2() => "";

        }

        struct XYZ {
            public int X;
            public int Y;
            public int Z;
            public XYZ(int x) {
                this = new XYZ();       // 必须对所有字段赋值
                X = x;      // 在这个过程中, X先被赋值为0, 然后在赋值为 x
            }
        }

        #region 数组
        public void TestUuZu() {
            int[,] a1 = { { 333, 44, 55 },
                          { 44, 22, 44 }};

            foreach (var i in a1) {
                Console.WriteLine(i);
            }
        }
        #endregion

        class Cloneable<T> where T : Cloneable<T> {
            T Clone() => (T)MemberwiseClone();
        }

        interface ICloneable<out T> where T : ICloneable<T> {
            T Clone();
        }

        class CA : ICloneable<CA> {
            public CA Clone() {
                const int a = 3;
                throw new NotImplementedException();
            }
        }

        class CB : CA, ICloneable<CB> {
            new public CB Clone() {
                throw new NotImplementedException();
            }
            public void Test() {
                CA cA = base.Clone();
                CA cB = this.Clone();
            }
        }

        static class Clone<T> {

        }

        #region Interface

        public static void TestInterface() {
            new ClientA2().Test1();
        }

        class ClientA : IDisposable {
            public virtual void Dispose() => Console.WriteLine("ClientA Dispose");
            public virtual void Dispose(bool isDisposing) => Console.WriteLine("ClientA isDisposing: " + isDisposing);

        }

        class ClientA2 : ClientA {
            new public void Dispose() => Console.WriteLine("ClientA2 Dispose");

            public void Test1() {
                ((IDisposable)this).Dispose();
                this.Dispose();
            }

            public override void Dispose(bool isDisposing) => Console.WriteLine("ClientB isDisposing: " + isDisposing);
        }
        struct AA : IComparable {
            int IComparable.CompareTo(object obj) => -1;
            public int CompareTo(AA a) {
                return 1;
            }

            public static void Test1() {
                var a = new AA();
                var b = new AA();
                Console.WriteLine(a.CompareTo(b));
                Console.WriteLine(((IComparable)a).CompareTo(b));
            }
        }

        interface IMBase {
            void Base();
        };
        interface IMA : IMBase {
            void A();
        }
        interface IMB : IMBase {
            void B();
        }

        class IM : IMA, IMB {
            public void A() { }
            public void B() { }
            public void Base() { }

            // 不支持这种显式实现，
            //void IMA.Base() { }
            //void IMB.Base() { }
            // 只允许这种显式实现，
            //void IMBase.Base() { }
            // 此时 IMBase 不需要明确是属于谁的接口，只是表明 IM 类实现了三个接口：IMA, IMB, IMBase，这只是约束，并不归属接口谁属于谁

        }


        #endregion

        #region Dll
        public class Amb {
            public void Test() {
                var ambs = AppDomain.CurrentDomain.GetAssemblies();      // 获取当前已加载的程序集，程序集并不是立即加载的
                //ambs[1].GetReferencedAssemblies
            }
        }
        #endregion
    }

    #region 枚举
    public enum Received {
        /// <summary>
        /// 发放中
        /// </summary>
        SENDING,
        /// <summary>
        /// 已发放待领取
        /// </summary>
        SENT,
        /// <summary>
        /// 发放失败
        /// </summary>
        FAILED,
        /// <summary>
        /// 已领取
        /// </summary>
        RECEIVED,
        /// <summary>
        /// 退款中
        /// </summary>
        RFUND_ING,
        /// <summary>
        /// 已退款
        /// </summary>
        REFUND,
    }

    public static class EnumEF {
        static class EnumEF_<T> {
            public static IReadOnlyList<string> Descriptions;
        }
        static EnumEF() {
            EnumEF_<Received>.Descriptions = new[] {
                "发放中", "已发放待领取", "发放失败", "已领取", "退款中", "已退款"
            };
            EnumEF_<DayOfWeek>.Descriptions = new[] {
                "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六",
            };
        }

        public static string ToDescription(this Received r) {
            return EnumEF_<Received>.Descriptions[(int)r];
        }

        public static string ToDescription<E>(this E e) where E : Enum {
            return EnumEF_<E>.Descriptions[e.GetHashCode()];     //  不支持强制转换。。。
        }

        public static void Test() {
            (EnumEF_<Received>.Descriptions as string[])[1] = "已发放待领取2";
            Console.WriteLine(DayOfWeek.Sunday.ToDescription());
            Console.WriteLine(Received.SENDING.ToDescription());

        }

    }


    #endregion
}
