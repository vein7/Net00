using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd.SrFa {
    public class HsBc {
        readonly static Random _ran = new Random();
        /* 
         按照概率生成随机数，分配红包金额
         */
        public static int[] Test1() {
            // 4000元，最低 100，最高 500，红包个数的范围4000/500 - 4000/100，8-40
            // 现在设定红包个数是20个，大部分数值应该是在 4000 / 20 = 200 这个范围，

            var min = 100;
            var max = 500;

            var num = _ran.Next(min, max);
            Console.WriteLine(num);
            return new[] { num };
        }

        public static int[] GenRedpack(int amount,int count, int min, int max) {
            amount *= 100;
            min *= 100;
            max *= 100;
            var avg = amount / count;
            var nums = new int[count];
            var sum = 0;
            for (var i = 0; i < count; i++) {
                nums[i] = _ran.Next(min, max);
                sum += nums[i];
            }

            // 写点注释


            return nums;
        }

    }
}
