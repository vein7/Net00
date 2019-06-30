using KsViTd.SrFa;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework;
using static System.Math;

namespace UTest1.SrFa {

    [TestClass]
    public class SrFaTest {
        [TestMethod]
        public void HsBcTest1() {
            using var strOut = new StringWriter();
            Console.SetOut(strOut);
            Console.WriteLine(
                string.Join(",", HsBc.Test1())
            );

        }

        [PexMethod]
        public void HelloWorld([PexAssumeNotNull]string value) {
            if (value.StartsWith("Hello")
            && value.EndsWith("World!")
            && value.Contains(" "))
                throw new Exception("found it!");

        }

        #region Java Hong Bao
        static Random _ran = new Random();

        [TestMethod]
        public void HsBcTest2() {
            // 有问题，很大概率不会中到平均值的边界值之外，
            // 如下参数，大部分红包会分布在，200 - 332 之间，而332 到 500 之前基本上很少，低于 10% 甚至更低。
            var ls = createBonusList(4000, 15, 200, 500);
        }

        public static bool canReward(double rate) {
            return _ran.NextDouble() <= rate;
        }

        private static int getRandomVal(int min, int max) {
            return _ran.Next(max - min + 1) + min;
        }

        public static int getRandomValWithSpecifySubRate(int boundMin, int boundMax, int subMin, int subMax, double subRate) {
            if (canReward(subRate)) {
                return getRandomVal(subMin, subMax);
            }
            return getRandomVal(boundMin, boundMax);
        }

        private static int randomBonusWithSpecifyBound(int totalBonus, int totalNum, int sendedBonus,
        int sendedNum, int rdMin, int rdMax) {
            int avg = totalBonus / totalNum;  // 平均值
            int leftLen = avg - rdMin;
            int rightLen = rdMax - avg;
            int boundMin = 0, boundMax = 0;

            // 大范围设置小概率
            if (leftLen == rightLen) {
                boundMin = Max((totalBonus - sendedBonus - (totalNum - sendedNum - 1) * rdMax), rdMin);
                boundMax = Min((totalBonus - sendedBonus - (totalNum - sendedNum - 1) * rdMin), rdMax);
            } else if (rightLen.CompareTo(leftLen) > 0) {
                // 上限偏离
                double bigRate = leftLen / (double)(leftLen + rightLen);
                int standardRdMax = avg + leftLen;  // 右侧对称上限点
                int _rdMax = canReward(bigRate) ? rdMax : standardRdMax;
                boundMin = Max((totalBonus - sendedBonus - (totalNum - sendedNum - 1) * standardRdMax), rdMin);
                boundMax = Min((totalBonus - sendedBonus - (totalNum - sendedNum - 1) * rdMin), _rdMax);
            } else {
                // 下限偏离
                double smallRate = rightLen / (double)(leftLen + rightLen);
                int standardRdMin = avg - rightLen;  // 左侧对称下限点
                int _rdMin = canReward(smallRate) ? rdMin : standardRdMin;
                boundMin = Max((totalBonus - sendedBonus - (totalNum - sendedNum - 1) * rdMax), _rdMin);
                boundMax = Min((totalBonus - sendedBonus - (totalNum - sendedNum - 1) * standardRdMin), rdMax);
            }

            // 已发平均值偏移修正-动态比例
            if (boundMin == boundMax) {
                return getRandomVal(boundMin, boundMax);
            }
            double currAvg = sendedNum == 0 ? (double)avg : (sendedBonus / (double)sendedNum);  // 当前已发平均值
            double middle = (boundMin + boundMax) / 2.0;
            int subMin = boundMin, subMax = boundMax;
            // 期望值 
            double exp = avg - (currAvg - avg) * sendedNum / (double)(totalNum - sendedNum);
            if (middle > exp) {
                subMax = (int)Round((boundMin + exp) / 2.0);
            } else {
                subMin = (int)Round((exp + boundMax) / 2.0);
            }
            int expBound = (boundMin + boundMax) / 2;
            int expSub = (subMin + subMax) / 2;
            double subRate = (exp - expBound) / (double)(expSub - expBound);
            return getRandomValWithSpecifySubRate(boundMin, boundMax, subMin, subMax, subRate);
        }


        public static int[] createBonusList(int totalBonus, int totalNum, int rdMin, int rdMax) {
            int sendedBonus = 0;
            int sendedNum = 0;
            var bonusList = new int[totalNum];
            while (sendedNum < totalNum) {
                int bonus = randomBonusWithSpecifyBound(totalBonus, totalNum, sendedBonus, sendedNum, rdMin, rdMax);
                bonusList[sendedNum] = bonus;
                sendedNum++;
                sendedBonus += bonus;
            }
            return bonusList;
        }
        #endregion
    }
}
