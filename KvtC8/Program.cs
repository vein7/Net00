using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//#nullable enable
namespace KvtC8 {
    public class Program {
        static void Main(string[] args) {
        }
    }

#nullable enable
    class NullTest {
        string Name;

        public void Test() {
            //Name = null;

            //DateTimeKind
        }
    }
    //#nullable disable



    class NullTest2 {
        string Title;
        public NullTest2() {
            //Title = "";
        }

    }

    struct NullTest4 {
        string name;
    }

    struct StructTest {
        public int AAA;//= 100;
        public string? SSS;
        public StructTest(int a) {
            AAA = 100;
            SSS = "";
        }
    }


}
