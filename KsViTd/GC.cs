using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd {
    class GCTest {

        public static void Test1() {
            //System.Data.Common.DbConnection conn = null;
            //conn.CreateCommand().ExecuteNonQueryAsync();
        }


        #region 21.3.3 GC 为本机资源提供的其他功能，抄录自 《CLR via C#》 
        public static void TestMemoryPressure1() {
            MemoryPressureDemo(0);
            MemoryPressureDemo(10 * 1024 * 1024);

            HandleCollectorDemo();
        }

        public static void MemoryPressureDemo(int size) {
            Console.WriteLine();
            Console.WriteLine($"MemoryPressureDemo, size={size}");
            for (int i = 0; i < 15; i++) {
                new BigNativeResource(size, i);     //  在循环时，很有可能就会频繁触发GC
            }

            // 出于演示目的，强制 GC
            GC.Collect();
        }

        public static void HandleCollectorDemo() {
            Console.WriteLine();
            Console.WriteLine($"HandleCollectorDemo");
            for (int i = 0; i < 10; i++) {
                new LimitedResource();
            }

            GC.Collect();
        }

        class BigNativeResource {
            int size;
            int i;
            public BigNativeResource(int size, int i) {
                this.size = size;
                this.i = i;
                if (this.size > 0) { GC.AddMemoryPressure(this.size); }
                Console.WriteLine("BigNativeResource create " + i);
            }

            ~BigNativeResource() {
                if (this.size > 0) { GC.RemoveMemoryPressure(this.size); }
                Console.WriteLine($"BigNativeResource destroy, size:{size}, i: " + i);
            }
        }

        class LimitedResource {
            static readonly HandleCollector hc = new HandleCollector("LimitedResource", 2);
            public LimitedResource() {
                hc.Add();
                Console.WriteLine($"LimitedResource create, count={hc.Count}");
            }

            ~LimitedResource() {
                hc.Remove();
                Console.WriteLine($"LimitedResource destroy, count={hc.Count}");
            }
        }


        #endregion

    }


    class Gc1 : SafeHandle {
        public Gc1() : base(new IntPtr(0), false) {

        }
        public override bool IsInvalid => throw new NotImplementedException();

        protected override bool ReleaseHandle() => throw new NotImplementedException();
    }

}
