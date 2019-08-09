using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace KsViTd {

    [Serializable]
    class AdLog {
        public int Id;
        public string OpenId;
        public DateTime CTime;
    }
    class LqTest {
        public LqTest() {
            var writer = new StreamWriter("E:/fileName.txt");
            writer.Write("dddd");
            writer.Dispose();
        }

        public T Deserializable<T>() {
            var mem = new MemoryStream();
            var bin = new BinaryFormatter();
            return (T)(bin.Deserialize(mem));
        }

    }

}
