using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd {
    public static class EFun {
        public static IEnumerable<T> Where0<T>(this IEnumerable<T> source, Func<T,bool> fun) {
            foreach(var item in source) {
                if (fun(item)) {
                    yield return item;
                }
            }
        }
        
    }
}
