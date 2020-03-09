using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd
{
    static class EnumDescriptionImpl
    {

        static EnumDescriptionImpl()
        {
            EnumDescription<DayOfWeek>.Descriptions = new[] { "星期日", "星期一", "星期二", "星期三", };
        }

        public static string Desc<T>(this T en) where T : Enum
        {
            return EnumDescription<T>.Descriptions[en.GetHashCode()];
        }

        static void Test()
        {
            //Console.WriteLine(DayOfWeek.Monday.ToDescription);
            var arr1 = new[] { "星期日", "星期一", "星期二", "星期三", };
            //arr1.OrderBy
        }

    }

    static class EnumDescription<T> where T : Enum
    {
        public static string[] Descriptions;
    }

    class AA<T>
    {
        public T Vi { get; } = default;


        public AA(T t)
        {
            Vi = t;     // 可以在构造函数里面对只读属性赋值
        }
    }
}
