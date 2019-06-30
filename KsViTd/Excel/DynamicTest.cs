using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;

namespace KsViTd_c {
    class DnmTest {
        public DnmTest() {
            var a = new ExpandoObject();
            
        }
    }


    class Dnm1 : IDynamicMetaObjectProvider {
        Dictionary<string, object> member = new Dictionary<string, object>();
        public DynamicMetaObject GetMetaObject(Expression parameter) {

            throw new NotImplementedException();
        }
    }


    //[System.Diagnostics.DebuggerDisplay("{member}")]
    class Dnm2 : DynamicObject {
        public string P;
        public string 实体1;
        Dictionary<string, object> member = new Dictionary<string, object>();
        int Length;

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            return member.TryGetValue(binder.Name, out result);
        }
        public override bool TrySetMember(SetMemberBinder binder, object value) {
            member[binder.Name] = value;
            return true;
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result) {
            switch (binder.Operation) {
            case ExpressionType.Add:
                result = (int)arg + Length;
                return true;

            }
            return base.TryBinaryOperation(binder, arg, out result);
        }


        public override string ToString() {
            var stw = new System.IO.StringWriter();
            return base.ToString();
        }
    }
}
