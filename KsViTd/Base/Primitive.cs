using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd.Base {
    delegate bool DTryParse<T>(string str, out T val);
    static class Primitive<T> {
        /// <summary>
        /// Jit 不会执行非泛型类 <see cref="Primitive"/> 的静态构造函数，<para></para>
        /// 这是正常的且正确的，如果 Jit 在加载一个类的时候，还去加载成员的类型，这样就递归加载了，没有必要
        /// </summary>
        static Primitive _primitive;

        static Primitive() {
            // 目的是为了执行非泛型的 Primitive() 静态构造函数
            // 每一个 Primitive<T> 泛型版本，都会执行静态构造函数，但是没有关系，非泛型 Primitive() 静态构造函数只会执行一次
            Primitive.Register();
            new NwBu();
        }

        public static DTryParse<T> TryParse;
        public static T MaxValue;
        public static T MinValue;

        class NwBu {
            public static void Register() { }
            static NwBu() {
                Console.WriteLine(typeof(T).ToString());

                Primitive<int>.TryParse = int.TryParse;
                Primitive<int>.MaxValue = int.MaxValue;
                Primitive<int>.MinValue = int.MinValue;

                Primitive<long>.TryParse = long.TryParse;
                Primitive<long>.MaxValue = long.MaxValue;
                Primitive<long>.MinValue = long.MinValue;


                Primitive<DateTime>.TryParse = DateTime.TryParse;
                Primitive<DateTime>.MaxValue = DateTime.MaxValue;
                Primitive<DateTime>.MinValue = DateTime.MinValue;
            }
        }
    }
    class Primitive {
        static Primitive() {
            Primitive<int>.TryParse = int.TryParse;
            Primitive<int>.MaxValue = int.MaxValue;
            Primitive<int>.MinValue = int.MinValue;

            Primitive<long>.TryParse = long.TryParse;
            Primitive<long>.MaxValue = long.MaxValue;
            Primitive<long>.MinValue = long.MinValue;


            Primitive<DateTime>.TryParse = DateTime.TryParse;
            Primitive<DateTime>.MaxValue = DateTime.MaxValue;
            Primitive<DateTime>.MinValue = DateTime.MinValue;

        }
        public static void Register() { }
    }


    public static class PrimitiveEx
    {
        public struct NewCtor { }
        public readonly static NewCtor New = new NewCtor();

        public static Dictionary<K, V> Dic<K, V>(this NewCtor New, K key, V value)
        {
            return new Dictionary<K, V>();
        }
    }


}
