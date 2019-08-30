using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDLL {
    class Async {
        public static async Task<string> Test1() {
            await Task.Delay(1);
            return $"sss{11}";
        }
    }
}
