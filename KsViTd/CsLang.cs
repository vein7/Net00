using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd {
    class CsLang {
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
            foreach(var d in ls) {
                Console.WriteLine($"{d.Name}\t{d.Dnm1.ToString()}\t{d.Dnm1.GetType()}" );
            }

            string a = "";
            object b = a;
            F1(out b);

            return this;
        }

        public void F1(out object obj) {
            obj = new List<int>();
        }
        
        public void FunTest() {
            Func<object, object> f;
            Func<string, string> f2 = (string s) => s;
            f = (obj) => f2(obj as string);        //obj 不一定是string 类型
            
            foreach(Func<object,object>  fn in f.GetInvocationList()) {

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
        
        class FnList: IEnumerable<Func<string>> {
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
                throw new NotImplementedException();
            }
        }

        class CB: CA, ICloneable<CB> {
            public CB Clone() {
                throw new NotImplementedException();
            }
            public void Test() {
                base.Clone();
                this.Clone();
            }
        }

        static class Clone<T> {

        }

        abstract class ColModels<T> {
             
        }
        class CluesColModel : ColModels<int> {
            static List<int> cols = new List<int>();
            public CluesColModel(bool isReconstruct) {
            }

        }


    }

}
