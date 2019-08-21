using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd.UuJuJpGz {
    interface ITopList<T> : IReadOnlyList<T> {
        ITopList<T> Add(T item);
    }

    static class TopList {
        public static ITopList<T> New<T>(int count) where T : IComparable<T> {
            return new TopListIC<T>(count);
        }
        public static ITopList<T> New<T>(int count, Func<T, T, bool> compare) {
            return new TopListFC<T>(count, compare);
        }
    }

    class TopListIC<T> : ITopList<T> where T : IComparable<T> {
        T[] list;
        public TopListIC(int count) {
            list = new T[count];
        }

        public ITopList<T> Add(T item) {
            var index = list.Length;
            // 待实现

            return this;
        }

        public T this[int index] => list[index];

        public int Count => list.Length;

        public IEnumerator<T> GetEnumerator() => list.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
    class TopListFC<T> : ITopList<T> {
        public TopListFC(int count, Func<T, T, bool> compare) {
        }

        public T this[int index] => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public ITopList<T> Add(T item) => throw new NotImplementedException();
        public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }

}
