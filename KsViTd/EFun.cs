using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd {
    public static class EFun {
        public static IEnumerable<T> Where0<T>(this IEnumerable<T> source, Func<T, bool> fun) {
            foreach (var item in source) {
                if (fun(item)) {
                    yield return item;
                }
            }
        }
        public static string ToString<T>(this Nullable<T> val, string format, IFormatProvider formatProvider = null)
            where T : struct, IFormattable {
            return val == null ? "" : val.Value.ToString(format, formatProvider);
        }

        public static void TestToString() {
            DateTime? d = null;
            Console.WriteLine("1, " + d.ToString("yyyy"));
            d = DateTime.Now;
            Console.WriteLine("2, " + d.ToString("yyyy"));
            Console.WriteLine("3, " + DateTime.Now.ToString("yyyy", null));
        }
    }
}
