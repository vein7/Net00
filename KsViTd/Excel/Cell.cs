using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KsViTd.Excel {

    public delegate bool DTry<T, TValue>(T obj, out TValue val);
    public static class ObjUtil {
        public static bool TryParse<T>(object obj, out T val) {
            throw new NotSupportedException();
        }
    }

    public abstract class Msg {
        #region 内部泛型静态字段
        protected static class SMsg<T> {
            public static string InvalidType;
        }
        static Msg() {
            SMsg<short>.InvalidType = "无效的整数";
            SMsg<int>.InvalidType = "无效的整数";
            SMsg<long>.InvalidType = "无效的整数";
            SMsg<ushort>.InvalidType = "无效的正整数";
            SMsg<uint>.InvalidType = "无效的正整数";
            SMsg<ulong>.InvalidType = "无效的正整数";
            SMsg<float>.InvalidType = "无效的数值";
            SMsg<double>.InvalidType = "无效的数值";
            SMsg<decimal>.InvalidType = "无效的数值";
            SMsg<DateTime>.InvalidType = "无效的日期格式";

            // 先注释，实际上可空类型不应该使用 Cell<Nullable<T>>，应该使用 CellNullable<T>，
            // CellNullable<T> 内部现实的是具体类型，所以以下的可空类型字段不需要
            //SMsg<short?>.InvalidType = "无效的整数";
            //SMsg<int?>.InvalidType = "无效的整数";
            //SMsg<long?>.InvalidType = "无效的整数";
            //SMsg<ushort?>.InvalidType = "无效的正整数";
            //SMsg<uint?>.InvalidType = "无效的正整数";
            //SMsg<ulong?>.InvalidType = "无效的正整数";
            //SMsg<float?>.InvalidType = "无效的数值";
            //SMsg<double?>.InvalidType = "无效的数值";
            //SMsg<decimal?>.InvalidType = "无效的数值";
            //SMsg<DateTime?>.InvalidType = "无效的日期格式";
        }
        #endregion

        protected string rowNo;
        protected int msgLen;
        public StringBuilder Builder = new StringBuilder();
        public bool RowHaveErr() { return msgLen > Builder.Length; }
        public override string ToString() { return Builder.ToString(); }

        public void SetRow(int rowNo) {
            this.rowNo = rowNo.ToString();
            msgLen = Builder.Length;
        }

        public abstract Msg Append(string name, string msg);
        public abstract Msg Append<T>(string cellName, T value, string msg);
        public abstract Msg Append(string cellName, string value, string msg);
        /// <summary>
        /// 解析时会为 cell.Value 赋值
        /// </summary>
        public abstract bool Parse<TValue>(CellBase<TValue> cell, object obj);
        public abstract bool NotNull<TValue>(CellBase<TValue> cell, object obj);
        public abstract bool VerifyLen(CellText cell);
        public abstract bool IsExist<TValue>(CellBase<TValue> cell, bool isExist);

    }

    public abstract class CellBase<TValue> : IReadCell {
        public string Name;
        public TValue Value;
        /// <summary>
        /// 把 object 解析为 TValue 类型，如果Parse 是null，会尝试调用默认解析方法
        /// </summary>
        public DTry<object, TValue> TryParse;
        /// <summary>
        /// Verify 执行一定是经过 Parse 之后，Value 的值一定有效的，然后才进行其他验证的。
        /// </summary>
        public Action<CellBase<TValue>, Msg> Verify;

        public CellBase(string name) { Name = name; }

        public abstract void Reading(Msg msg, object obj);
    }

    /// <summary>
    /// 可空
    /// </summary>
    public class CellNullable<TValue> : CellBase<TValue> {
        public CellNullable(string name) : base(name) { }

        public override void Reading(Msg msg, object obj) {
            Value = default(TValue);
            if (obj != null && string.IsNullOrWhiteSpace(obj.ToString()) == false) {
                msg.Parse(this, obj);
            }
            Verify(this, msg);
        }
    }

    /// <summary>
    /// 非空
    /// </summary>
    public class Cell<TValue> : CellBase<TValue> {
        public Cell(string name) : base(name) { }

        public override void Reading(Msg msg, object obj) {
            Value = default(TValue);
            if (msg.NotNull(this, obj) && msg.Parse(this, obj)) {
                Verify(this, msg);
            }
        }
    }

    /// <summary>
    /// 文本内容，不进行Parse，如果有长度限制，会进行检查
    /// </summary>
    public class CellText : CellBase<string> {
        /// <summary>
        /// 文本最大长度限制，默认是0，不进行最大长度限制检查
        /// </summary>
        public readonly int MaxLen;
        /// <summary>
        /// 是否可空
        /// </summary>
        public readonly bool IsNullable;
        public CellText(string name, bool isNullable = true, int maxLen = 0)
            : base(name) {
            MaxLen = maxLen;
            IsNullable = isNullable;
        }
        public override void Reading(Msg msg, object obj) {
            Value = obj == null ? "" : obj.ToString().Trim();
            if (IsNullable == false && msg.NotNull(this, obj) == false) { return; }
            if (msg.VerifyLen(this) == false) { return; }

            Verify(this, msg);
        }
    }

    /// <summary>
    /// 该类默认使用 double 类型解析，然后强制转换为 int      <para></para>
    /// Excel 没有 int 类型，对于特殊格式的数值可能会解析错误，例如数值 1.00 使用 int.TryParse 会解析错误
    /// </summary>
    public class CellInt : Cell<int> {
        public CellInt(string name)
            : base(name) {
            TryParse = ParseDouble;
        }
        public static bool ParseDouble(object obj, out int val) {
            var d = 0D;
            var bol = ObjUtil.TryParse(obj, out d);
            val = (int)d;
            return bol;
        }

    }

    /// <summary>
    /// 该类默认使用 double 类型解析，然后强制转换为 int。 <para></para>
    /// Excel 没有 Int 类型，对于特殊格式的数值可能会解析错误，例如数值 1.00 使用 int.TryParse 会解析错误
    /// </summary>
    public class CellIntN : CellNullable<int> {
        public CellIntN(string name)
            : base(name) {
            TryParse = CellInt.ParseDouble;
        }
    }

    public class CellDate : Cell<DateTime> {
        public CellDate(string name)
            : base(name) {
            // Cell<DateTime> 类默认会调用 ObjUtil.TryParse<T>(object obj, out T val)，obj 参数类型为 double 时会解析失败
            // 本类使用 ObjUtil.TryParse(object obj, out DateTime val)，因为 Cell<DateTime> 类在运行时无法进行重载
            TryParse = ObjUtil.TryParse;
        }
    }

    public class CellDateN : CellNullable<DateTime> {
        public CellDateN(string name)
            : base(name) {
            TryParse = ObjUtil.TryParse;
        }
    }

    public class CellEnum<TEnum> : Cell<TEnum> where TEnum : struct {
        public readonly bool IgnoreCase;
        public CellEnum(string name, bool ignoreCase = true)
            : base(name) {
            IgnoreCase = ignoreCase;
            TryParse = ParseEnum;
        }

        public bool ParseEnum(object obj, out TEnum val) {
            return Enum.TryParse<TEnum>(obj.ToString(), IgnoreCase, out val);
        }

    }


}
namespace KsViTd.Excel2 {
    public interface IReadCell {
        void Reading(object obj);
    }
    public delegate bool DTry<T, TValue>(T obj, out TValue val);

    public abstract class CellBase<TValue> : IReadCell {
        public string Name;
        public TValue Value;
        /// <summary>
        /// 把 object 解析为 TValue 类型，如果Parse 是null，会尝试调用默认解析方法
        /// </summary>
        public DTry<object, TValue> TryParse;
        /// <summary>
        /// Verify 执行一定是经过 Parse 之后，Value 的值一定有效的，然后才进行其他验证的。
        /// </summary>
        //public Action<CellBase<TValue>, Msg> Verify;

        //public CellBase(string name) { Name = name; }

        public abstract void Reading(object obj);
    }

    public abstract class CellN<TValue> : CellBase<TValue> {
        public override void Reading(object obj) {

        }
    }

    public static class Cell {
        public static CellBase<TValue> New<TValue>(string name, Action<TValue> Verify) {
            
            return null;
        }
        public static CellN<TValue> New<TValue>(string name, Action<TValue?> Verify) where TValue : struct {
            return null;
        }

        public static void Test() {
            Cell.New<int?>("AA", (int? s) => { });
        }
    }
}
