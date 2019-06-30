using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wisdom4s.Utility;

namespace Wisdom4s.Web.Entity
{

    /// <summary>
    /// 输出的消息包含 HTML 元素
    /// </summary>
    public class MsgHtml : Msg
    {
        /// <summary>
        /// 解析时会为 cell.Value 赋值
        /// </summary>
        public override bool Parse<TValue>(CellBase<TValue> cell, object obj)
        {
            if ((cell.TryParse ?? ObjUtil.TryParse)(obj, out cell.Value)) { return true; }

            Builder.AppendFormat("行：{0}，列：{1}，{2}。<br />", rowNo, cell.Name, SMsg<TValue>.InvalidType);
            return false;
        }

        public override bool NotNull<TValue>(CellBase<TValue> cell, object obj)
        {
            if (obj != null && string.IsNullOrWhiteSpace(obj.ToString()) == false) { return true; }

            Builder.AppendFormat("行：{0}，列：{1}，不能为空。<br />", rowNo, cell.Name);
            return false;
        }

        public override bool IsExist<TValue>(CellBase<TValue> cell, bool isExist)
        {
            if (isExist) { return true; }
            Builder.AppendFormat("行：{0}，列：{1}，“<b>{2}</b>”不存在。<br />", rowNo, cell.Name, cell.Value);
            return false;
        }

        public override bool VerifyLen(CellText cell)
        {
            if (cell.MaxLen > 0 && cell.Value.Length > cell.MaxLen)
            {
                Builder.AppendFormat("行：{0}，列：{1}，不能超过：<b>{2}</b>个字符<br />", rowNo, cell.Name, cell.MaxLen);
                return false;
            }
            return true;
        }

        public override Msg Append(string cellName, string msg)
        {
            Builder.AppendFormat("行：{0}，列：{1}，{2}<br />", rowNo, cellName, msg);
            return this;
        }

        public override Msg Append<T>(string cellName, T value, string msg)
        {
            Builder.AppendFormat("行：{0}，列：{1}，值：<b>{2}</b>，{3}<br />", rowNo, cellName, value.ToString(), msg);
            return this;
        }
        public override Msg Append(string cellName, string value, string msg)
        {
            Builder.AppendFormat("行：{0}，列：{1}，值：<b>{2}</b>，{3}<br />", rowNo, cellName, value, msg);
            return this;
        }


    }
}
