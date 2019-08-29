using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd.UuJuJpGz {
    abstract class TopList<T> : IReadOnlyList<T> {
        protected T[] list;
        abstract public bool Add(T item);

        public T this[int index] => list[index];

        public int Count => list.Length;

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)list).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }

    static class TopList {
        public static TopList<T> New<T>(int count) where T : IComparable<T> {
            return new TopListIC<T>(count);
        }
        public static TopList<T> New<T>(int count, Func<T, T, bool> canAdd) {
            return new TopListFC<T>(count, canAdd);
        }

        public static void Test1() {
            //var top = TopList.New(10, (string s) => s[0]);
        }
    }

    class TopListIC<T> : TopList<T> where T : IComparable<T> {
        internal TopListIC(int count) {
            list = new T[count];
        }

        public override bool Add(T item) {
            int i = 0, i1 = 0, i2 = list.Length - 1;
            while (i1 != i2) {
                i = (i1 + i2) / 2;
                switch (item.CompareTo(list[i])) {
                case 1: i2 = i; break;
                case 0: goto Found;
                case -1: i1 = i + 1; break;
                }
                if (i1 > i2) { return false; }      // 没找到
            }
            i = i1;
            Found:
            Array.Copy(list, i, list, i + 1, list.Length - i - 1);
            list[i] = item;
            return true;
        }

    }
    class TopListFC<T> : TopList<T> {
        Func<T, T, bool> canAdd;
        internal TopListFC(int count, Func<T, T, bool> canAdd) {
            list = new T[count];
            this.canAdd = canAdd;
        }

        public override bool Add(T item) {
            int i = list.Length - 1;
            if (!canAdd(item, list[i])) { return false; }
            for (; i > 0 && canAdd(item, list[i - 1]); i--) {
                list[i] = list[i - 1];
            }
            list[i] = item;
            return true;
        }
    }

}
