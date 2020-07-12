using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd.SrFa {

    public class TreeNode {
        public int val;
        public TreeNode left;
        public TreeNode right;
        public TreeNode(int x) { val = x; }

        public void XmXuBmLi() {
            Console.WriteLine(this.val);
            this.left.XmXuBmLi();
            this.right.XmXuBmLi();
        }
    }

    class ErIaUu {
        public int MaxDepth(TreeNode root) {
            return root == null ? 0 : Math.Max(MaxDepth(root.left), MaxDepth(root.right)) + 1;
        }

        public bool IsValidBST(TreeNode root) {
            var curr = Int64.MinValue;
            
            return IsValid(root);

            bool IsValid(TreeNode node) {
                if (node == null) return true;
                if (IsValid(node.left) == false || node.val <= curr) { return false; }
                curr = node.val;
                return IsValid(node.right);
            }
        }

        public bool IsSymmetric(TreeNode root) {
            if (root == null) return true;
            return IsSymmetric(root.left, root.right);
            
        }

        public bool IsSymmetric(TreeNode left, TreeNode right) {
            if (left == null || right == null) { return left == right; }
            if (left.val != right.val) { return false; }
            return IsSymmetric(left.left, right.right) && IsSymmetric(left.right, right.left);
        }

        public IList<IList<int>> LevelOrder(TreeNode root) {
            var res = new List<IList<int>>();
            var qu = new Queue<TreeNode>();
            qu.Enqueue(root);
            while (qu.Count > 0) {
                var ls = new List<int>();
                for (int c = qu.Count, i = 0; i < c; i++) {
                    var node = qu.Dequeue();
                    if (node == null) { continue; }
                    ls.Add(node.val);
                    qu.Enqueue(node.left);
                    qu.Enqueue(node.right);
                }
                if (ls.Count <= 0) { continue; }
                res.Add(ls);
            }
            return res;
        }

        public TreeNode SortedArrayToBST(int[] nums) {
            return SortedArrayToBST(nums, 0, nums.Length - 1);
        }

        public TreeNode SortedArrayToBST(int[] nums, int start, int end) {
            if (start > end) { return null; }

            var i = (start + end) / 2;
            return new TreeNode(nums[i]) {
                left = SortedArrayToBST(nums, start, i - 1),
                right = SortedArrayToBST(nums, i + 1, end)
            };
        }
    }
}
