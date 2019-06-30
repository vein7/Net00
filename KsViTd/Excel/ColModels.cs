using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd.Excel {

    [DebuggerDisplay("{header},{name},{rowspan},{colspan}")]
    public class ColModel {
        #region Json 属性部分，对应 OmGrid.colModel 对象
        public string header;
        public string name;
        /// <summary> 对应 OmGrid.colModel 对象, 单位是像素 </summary>
        public int width;
        public string align;
        public JRaw renderer;

        [DefaultValue(1)]
        public int rowspan = 1;
        [DefaultValue(1)]
        public int colspan = 1;
        #endregion

        private LambdaExpression _field;
        [JsonIgnore]
        public MemberExpression RenderBody;
        [JsonIgnore]
        public Expression Field {
            get { return _field; }
            set {
                //switch (value) {
                //    case LambdaExpression lmd: Field = lmd.Body; break;
                //    case MemberExpression member:
                //        // 在生成委托时, 委托参数是 "e"
                //        if (member.Expression is ParameterExpression p && p.Name != "e") {
                //            member.Update(Expression.Parameter(typeof(Com), "e"));
                //        }
                //        RenderBody = member;
                        
                //        break;
                //    case MethodCallExpression method:
                //        var a = method.Method;
                //        Field = method.Object;
                //        break;
                //    case BinaryExpression binary:

                //        break;
                //    default: throw new Exception($"{nameof(Field)} 无法转换, {value}");
                //}
                
                //if (renderer == null) {
                //    renderer = new JRaw(string.Format("function(val, d, i){{ return {0}; }}", NumberFormat.JsRender));
                //}
            }
        }
        //[JsonIgnore]
        //public Action<ExcelStyle> SetColStyle;        // ExcelStyle 不能 New
        /// <summary> 
        /// 具体格式可以打开Excel -> 右键设置单元格格式 -> 【数字】选项卡 -> 【自定义】 查看格式 <para></para>
        /// 自定义格式, 可对数值进行判断, 最多可有四个类别, 用分号隔开, 【非负数;负数】、【正数;负数;零;本文】
        /// 
        /// </summary>
        [JsonIgnore]
        public string NumberFormat;


    }

    [DebuggerDisplay("{Header.header},{Count},Row:{RowCount},SubCols:{SubCols.Count}")]
    public class ColModels<TEntity> : IEnumerable<ColModel> {
        // 多表头的构造，在集合初始化的时候，共享同一个 Cols，所以设计一个静态字段，若要重新构造，注意 New 一个新的 
        // 用静态字段共享同一个 Cols, 会存在并行问题, 第一个 ColModels<EUser> 正在生成, 
        // 第二个执行 new ColModels<EUser>(true), 把两个构建的基本列表破坏了.
        // protected static List<ColModel> staticCols;
        protected List<ColModel> ShareCols;

        public ColModel Header = new ColModel();
        public List<ColModels<TEntity>> SubCols = new List<ColModels<TEntity>>();
        public int RowCount { get; private set; }// = 1;
        public int Count { get; private set; }
        int indexCol;

        /// <summary>多表头一定要传入同一个 cols</summary>
        public ColModels(List<ColModel> cols) { this.ShareCols = cols; RowCount = 1; }
        //public ColModels(bool isReconstruct = false) { if(isReconstruct) staticCols = new List<ColModel>() ; RowCount = 1; }

        /// <param name="field">TField 必须是 TEntity 的字段, 否则出错</param>
        public void Add<TField>(string header, Expression<Func<TEntity, TField>> field
        , int width = 80, string align = "center", string renderer = null) {
            ShareCols.Add(new ColModel {
                header = header,
                width = width,
                align = align,
                renderer = new JRaw(renderer),
                Field = field
            });
            ++Count;
        }

        /// <param name="field">TField 必须是 TEntity 的字段, 否则出错</param>
        public void Add<TField>(string header, Expression<Func<TEntity, TField>> field, NumFmt numFormat
        , int width = 80, string align = "center") {
            ShareCols.Add(new ColModel {
                header = header,
                NumberFormat = numFormat.ExcelFormmat,
                width = width,
                align = align,
                renderer = numFormat.JsRenderer,
                Field = field
            });
            ++Count;
        }

        public void Add(string header, ColModels<TEntity> newSubCols) {
            //if (newSubCols.Count == 0) { throw new Exception("不允许添加空的集合: newSubCols.count == 0"); }
            if (newSubCols.Count == 0) { return; }

            newSubCols.indexCol = ShareCols.Count - newSubCols.Count;
            newSubCols.Header = new ColModel { header = header, colspan = newSubCols.Count, rowspan = 1 };
            SubCols.Add(newSubCols);

            if (this.RowCount <= newSubCols.RowCount) { this.RowCount = newSubCols.RowCount + 1; }
            this.Count += newSubCols.Count;
            this.Header.colspan = this.Count;
        }

        public void BuildMultHeader() {
            var queue = new Queue<ColModels<TEntity>>();
            queue.Enqueue(this);
            while (queue.Count > 0) {
                var cm = queue.Dequeue();
                var i = cm.indexCol;
                foreach (var cols in cm.SubCols) {
                    while (i < cols.indexCol) { ShareCols[i++].rowspan = cm.RowCount; }
                    queue.Enqueue(cols);
                    i += cols.Count;
                }
                while (i < cm.indexCol + cm.Count) { ShareCols[i++].rowspan = cm.RowCount; }
            }

            // 断开与静态字段的关联
            //sCols = null;
        }

        /// <summary> 对应 OmGrid.colModel 对象 </summary>
        public string ToJson() {
            var ls = new List<ColModel>[this.RowCount];
            var row = -1;
            EachCengJi((col, ir, ic) => {
                if (ir != row) { ls[++row] = new List<ColModel>(); }
                ls[row].Add(col);
            });

            var setting = new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(ls, setting);
        }

        /// <summary> 前序遍历 </summary>
        public void EachQianXu(Action<ColModel, int, ColModels<TEntity>> action) {
            var i = 0;
            foreach (var cols in SubCols) {
                while (i < cols.indexCol) { action(ShareCols[i++], i, this); }
                action(cols.Header, cols.indexCol, this); ;
                cols.EachQianXu(action);
                i += cols.Count;
            }
            while (i < this.Count) { action(ShareCols[i++], i, this); }

        }

        /// <summary> 层级遍历 </summary>
        /// <param name="action">ColModel: 迭代元素, int: 所在表头的 indexRow, int: 所在表头的 indexCol  </param>
        public void EachCengJi(Action<ColModel, int, int> action) {
            var queue = new Queue<ColModels<TEntity>>();
            queue.Enqueue(this);
            while (queue.Count > 0) {
                var cm = queue.Dequeue();
                var i = cm.indexCol - 1;
                foreach (var cols in cm.SubCols) {
                    while (++i < cols.indexCol) { action(ShareCols[i], this.RowCount - cm.RowCount, i); }
                    action(cols.Header, this.RowCount - cm.RowCount, i);
                    queue.Enqueue(cols);
                    i += cols.Count - 1;
                }
                while (++i < cm.indexCol + cm.Count) { action(ShareCols[i], this.RowCount - cm.RowCount, i); }
            }
        }

        public IEnumerator<ColModel> GetEnumerator() {
            for (var i = indexCol; i < Count; i++) { yield return ShareCols[indexCol + i]; }
        }

        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

        // 允许越界, index < 0, index >= Count
        public ColModel this[int index] { get { return ShareCols[indexCol + index]; } }

        public Expression ChangeParameterName(Expression body) {
            //switch (body) {
            //    case MemberExpression member:
            //        // 在生成委托时, 委托参数是 "e"
            //        if (member.Expression is ParameterExpression p && p.Name != "e") {
            //            return member.Update(Expression.Parameter(typeof(TEntity), "e"));
            //        } else {
            //            throw new 
            //        }
            //        return body;
            //    //case BinaryExpression binary:

            //    //    break;
            //    default: throw new Exception($"无法转换, " + body);
            //}
            var member = body as MemberExpression;
            var par = member.Expression as ParameterExpression;
            if (member != null && par != null) {
                return par.Name == "e" ? body : member.Update(Expression.Parameter(typeof(TEntity), "e"));
            }

            throw new Exception($"无法转换, " + body);
        }



    }

}
