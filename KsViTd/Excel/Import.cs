using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd.Excel {
    class Import {

        public Import Test() {
            

            return this;
        }

        public static T Parse<T>() {
            if(typeof(T) == typeof(int)) {
                //return 2;
            }
            return default(T);
        }

        class EClues {
            int ModelId;
            string Ndd;
            int Clues;
            int Delivery;
        }

        class CluesCheck : CheckExcel<EClues> {
            string[] colName = new[] { "经销商代码", "登记数", "交付数" };
            Func<object, int, int, bool> NddCheck;
            Func<object, int, int, bool> cluesCheck;
            public CluesCheck() {
                NddCheck += NotNull;
                cluesCheck += NotNull;
            }

            public override string ColName(int index) {
                return colName[index];
            }
        }

        class CheckCol<TEntity> {
            public string Name;
            public Func<object, TEntity, bool> Check;
            public CheckCol(string name, params Func<object, TEntity, bool>[] funcs) {
                Name = name;
                foreach (var f in funcs) { Check += f; }     // new CheckCol(""){Check += f} 编译错误, 为了一个 += 运算符折腾...
            }


        }


        abstract class CheckExcel<TEntity> {
            public StringBuilder Msg = new StringBuilder();
            public int rowNo = 0;

            public abstract string ColName(int index);
            //public abstract string this[int index] { get; }

            public CheckExcel<TEntity> Init() {

                return this;
            }

            public bool NotNull(object value, int rowNo, int iCol) {
                if (value is string && !string.IsNullOrWhiteSpace((string)value)
                || value != null) {
                    return true;
                }

                Msg.AppendFormat("行：{0}，列：{1}，不能为空。\n", rowNo, ColName(iCol));
                return false;
            }
            public bool NotInt(object value, int rowNo, int iCol, out int intVal) {
                if (null != value && int.TryParse(value.ToString(), out intVal)) {
                    return true;
                }

                intVal = 0;
                Msg.AppendFormat("行：{0}，列：{1}，不是数值。\n", rowNo, ColName(iCol));
                return false;
            }
        }


        class Col<TEntity> where TEntity : new() {
            public object Value;
            public string Name;
            public Func<Row<TEntity>, Col<TEntity>, bool> Verify;
            public Action<TEntity, object, bool> Result;
            public Col(string name, Action<TEntity, object, bool> result) {
                Name = name;
                Result = result;
            }
        }
        class Row<TEntity> where TEntity : new() {
            public TEntity Entity = new TEntity();
            public int No;
            public StringBuilder Msg;
            public Row(StringBuilder msg) => Msg = msg;
            public int Next() { return ++No; }
            public int End() { return 0;/* Entitys .len */ }
        }
        class Row<TEntity, TTag> where TEntity : new() where TTag : new() {
            public TEntity Entity = new TEntity();
            public TTag Tag = new TTag();
            public int No;
            public StringBuilder Msg;
            public Row(StringBuilder msg) => Msg = msg;
        }
        class Msg {
            public bool NotNull<T>(Row<T> row, Col<T> col) where T : new() {
                row.Msg.AppendFormat("行：{0}，列：{1}，不能为空。\n", row.No, col.Name);
                return this;
            }

            public static implicit operator bool(Msg m) {
                return false;
            }
        }

        //class Validator : IEnumerable<Validator> {
        //    public void Test() {
        //        var vs = new Validator {
        //            { ()=>true,()=>true },
        //            { ()=>true,()=>true,()=>false }
        //        };

        //        var dic = new Dictionary<int, int> {
        //            { 1, 2 },
        //            { 1, 2 }
        //        };
        //    }

        //    public bool F1() { return true; }
        //    public IEnumerator<Validator> GetEnumerator() => throw new NotImplementedException();
        //    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        //    public void Add(Func<bool> verify, Validator pass, Validator notPass) {

        //    }
        //    public void Add(Func<bool> verify, Func<bool> pass) {

        //    }
        //    public void Add(Func<bool> verify, Func<bool> pass, Func<bool> notPass) {

        //    }

        //    public static implicit operator Validator(Func<bool> verify) { return new Validator(); }


        //}


        class Company {
            public string Region;
            public string Name;
            public string Ndd;
            public string Tel;
            public string Email;
        }

        class CompanyExcel {
            public Msg msg;
            public CompanyExcel Init() {
                var col = new Col<Company>("电话", (e, v, b) => e.Tel = b ? v.ToString() : "120");
                col.Verify += NotNull;

                foreach (var i in col.Verify.GetInvocationList()) {

                }

                var com = new Company();
                Setter(ref com.Ndd);



                return this;
            }

            public bool Each(object row) {


                return true;
            }

            void Setter<T>(T pro, T def = default(T)) { }

            void Setter(ref string pro, string def = "") { }

            void Setter(Company row, object v, bool pass) { }
            void Setter<T>(Company row, object v, bool pass, T tag) { }

            void end(Row<Company> r, Col<Company> c, bool b) { }


            bool IsNull(object value) {
                return value == null || (value is string s && string.IsNullOrWhiteSpace(s));
            }

            bool IsNull(string value) {
                return string.IsNullOrWhiteSpace(value);
            }

            bool NotNull(Row<Company> row, Col<Company> col) {
                return !IsNull(col.Value) || msg.NotNull(row, col);
            }

            bool NotNull2(Row<Company> row, Col<Company> col) {
                return !IsNull(col.Value) || msg.NotNull(row, col);
            }
        }
        class TableReader {

        }



    }
}
