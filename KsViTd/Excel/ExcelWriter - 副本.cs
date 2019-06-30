using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace KsViTd.Excel {
    interface IExcelColSetting {
        void SetColumn(ExcelColumn eCol, ColModel col);
        void SetHeaderStyle(ExcelStyle style);
    }

    class Com {
        public int ComId;
        public string Ndd;
        public string Name;
        public DateTime CreateDate;
        public DateTime DeleteDate;
        public double XmSoLv;
        public double UiJxLv;
    }
    static class ExcelWriter {
        public static void Test() {
            Expression<Func<ExcelRange, object>> exp = e => e[1, 2].Value;
            
            var fn = lmd.Compile();


            using (var excel = new ExcelPackage()) {

                var cells = excel.Workbook.Worksheets.Add("S1").Cells;

                var a = fn(cells, 2, 2);

                excel.SaveAs(new System.IO.FileInfo(@"E:\190224.xlsx"));
            }





            Console.WriteLine(exp);
        }

        public static void F1() {
            //var res = 1;
            //while (n > 1) {
            //    res = res * n;
            //    n--;
            //}
            //return res;
            var nArgument = Expression.Parameter(typeof(int), "n");
            var result = Expression.Variable(typeof(int), "result");

            // Creating a label that represents the return value
            LabelTarget label = Expression.Label(typeof(int));

            var initializeResult = Expression.Assign(result, Expression.Constant(1));

            // This is the inner block that performs the multiplication,
            // and decrements the value of 'n'
            var block = Expression.Block(
                Expression.Assign(result,
                    Expression.Multiply(result, nArgument)),
                Expression.PostDecrementAssign(nArgument)
            );

            // Creating a method body.
            BlockExpression body = Expression.Block(
                new[] { result },
                initializeResult,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.GreaterThan(nArgument, Expression.Constant(1)),
                        block,
                        Expression.Break(label, result)
                    ),
                    label
                )
            );
        }

    }
    public class NumFmt {
        public readonly string ExcelFormmat;
        public readonly JRaw JsRenderer;

        public static readonly NumFmt ShiFou;
        public static readonly NumFmt BaiFenBi;

        private NumFmt(string excelFormat,string jsRender) {
            ExcelFormmat = excelFormat;
            JsRenderer = new JRaw("function(v, d, i){ return " + jsRender + "; }");
        }

        static NumFmt() {
            ShiFou = new NumFmt("是;否;否;", "val ? '是' : '否'");
            BaiFenBi = new NumFmt("0.00%", "(val * 100).toFixed(2) + '%'");
            
        }
        
    }

    

    class ExcelWriter<TEntity> {
        public Action<ExcelRange, IList<TEntity>> WriteExcel;

        public static void Test() {
            var ls = new List<ColModel>();
            var cm = new ColModels<Com>(ls) {
                {"大区", e => e.Name, NumFmt.ShiFou, 100 },
                {"小区", e => e.Name, 100 },
                {"经销商代码", e => e.Ndd, 100 },
                {"经销商名称", e => e.Name, 100 },
                {"有线索无试驾", new ColModels<Com>(ls) {
                    {"试驾率",e => e.UiJxLv, 50 },
                    {"线索率",e => e.UiJxLv, 50 }
                } },
                {"创建时间", e => e.CreateDate },
                {"删除时间", e => e.DeleteDate }
            };



        }

        public static Action<ExcelRange, IList<TEntity>> BuildAction(ColModels<TEntity> colModels) {
            var range = Expression.Parameter(typeof(ExcelRange), "range");
            var src = Expression.Parameter(typeof(IList<Com>), "src");
            var iSrc = Expression.Variable(typeof(int), "iSrc");
            var iRow = Expression.Variable(typeof(int), "iRow");
            var iCol = Expression.Variable(typeof(int), "iCol");
            var iSrcAdd = Expression.PreIncrementAssign(iSrc);
            var iRowAdd = Expression.PreIncrementAssign(iRow);
            var iColAdd = Expression.PreIncrementAssign(iCol);
            var iRowStart= Expression.Assign(iRow, Expression.Constant(colModels.RowCount));
            var iColStart = Expression.Assign(iCol,Expression.Constant(0));
            var endForRow = Expression.Label("endForRow");

            // ++iSrc; ++iRow; iCol = 0;
            // range[iRow, ++iCol].Value = src[iSrc].Field1;
            // range[iRow, ++iCol].Value = src[iSrc].Field2;   ...
            var forRowBody = Expression.Block(
                new Expression[] { iSrcAdd, iRowAdd, iColStart }.Concat(
                    colModels.Select(col => Expression.Assign(
                        Expression.Property(Expression.Call(range, "get_Item", null, iRow, iColAdd), "Value"),
                        col.RenderBody.Update(Expression.Call(src, "get_Item", null, iSrc))
                    )))
                );

            // iRow < src.Count ? forRowBody : break ;
            var forRow = Expression.Loop(Expression.IfThenElse(
                    Expression.LessThan(iRow, Expression.Property(src, "Count")), 
                    forRowBody,
                    Expression.Break(endForRow)),
                endForRow);

            var body = Expression.Block(new[] { iSrc, iRow, iCol }, iRowStart, forRow);

            var lmd = Expression.Lambda<Action<ExcelRange, IList<TEntity>>>(body, range, src);

            return lmd.Compile();
        }



        public static void Export(ColModels<TEntity> colModels, IList<TEntity> src, string xlsName) {

            var excel = new ExcelPackage();
            var cells = excel.Workbook.Worksheets.Add("Sheet1").Cells;
            var colSetting = new ExcelColDefault();     // 待处理 ,不需要 New 一个出来



            // 设置列
            for (var i = 0; i < colModels.Count; i++) {
                colSetting.SetColumn(cells.Worksheet.Column(i + 1), colModels[i]);
            }

            // 构建表头
            colModels.EachCengJi((col, ir, ic) => {
                var cell = cells[ir + 1, ic + 1, ir + col.rowspan, ic + col.colspan];
                cell.Merge = true;
                cell.Value = col.header;
                cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            });

            // 设置表头风格
            var headerStyle = cells[1, 1, colModels.RowCount, colModels.Count].Style;
            colSetting.SetHeaderStyle(headerStyle);


            for (int i = 0; i < src.Count; i++) {
                
            }


            //ExcelHelper.Download(excel, DateTime.Now.ToString("hh,mm,ss"));

        }
    }

    class ExcelColDefault : IExcelColSetting {
        public void SetColumn(ExcelColumn eCol, ColModel col) {
            eCol.Width = col.width / 6.4;      // width 单位是像素, 6.4 是一个比例
            eCol.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            eCol.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //col.SetColStyle?.Invoke(eCol.Style);
            //if (col.SetColStyle != null) { col.SetColStyle(eCol.Style); }
        }

        public void SetHeaderStyle(ExcelStyle style) {
            style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            style.VerticalAlignment = ExcelVerticalAlignment.Center;
            style.Border.Right.Style = ExcelBorderStyle.Thin;
            style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            style.Font.Bold = true;
            style.Fill.PatternType = ExcelFillStyle.Solid;
            //style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

        }

    }
}
