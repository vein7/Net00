using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd.SrFa {
    public class ListNode {
        public int val;
        public ListNode next;
        public ListNode(int x) {
            val = x;
            next = null;
        }
    }

    public class LmBc {
        public bool HasCycle(ListNode head) {
            ListNode p1 = head, p2 = head;
            while (p2 != null && p2.next != null) {
                p1 = p1.next;
                p2 = p2.next.next;
                if (p1 == p2) { return true; }
            }
            return false;
        }
    }
}
