using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KsViTd {
    class LqTest {
        public LqTest() {
            var writer = new StreamWriter("E:/fileName.txt");
            writer.Write("dddd");
            writer.Dispose();
        }
    }
}
