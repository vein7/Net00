using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd.UuJuJpGz {
    class Stack<T> {
        int currentLen;
        LinkNode<T> current;


        public int Count { get; private set; }
        public T Peek => current.Items[currentLen - 1];

        public Stack() {
            current = new LinkNode<T>() { Items = new T[2] };
        }

        public Stack(int capacity) {
            current = new LinkNode<T>() { Items = new T[capacity] };
        }

        public Stack<T> Push(T item) {
            if (currentLen == current.Items.Length) { AddLinkNode(); }

            current.Items[currentLen] = item;
            ++currentLen;
            ++Count;
            return this;
        }

        public Stack<T> Pop() {
            if (currentLen == 0) {
                if (current.Pre == null) { throw new Exception("栈中无元素"); }
                current = current.Pre;
                currentLen = current.Items.Length;
            }
            --currentLen;
            --Count;
            return this;
        }


        private void AddLinkNode() {
            if (current.Next == null) { current.Next = new LinkNode<T> { Items = new T[Count], Pre = current }; }
            current = current.Next;
            currentLen = 0;
        }

    }

    class LinkNode<T> {
        public T[] Items;
        public LinkNode<T> Pre;
        public LinkNode<T> Next;
    }
}
