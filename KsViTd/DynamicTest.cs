using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Linq.Expressions;

namespace KsViTd {
    class DynamicTest : IDynamicMetaObjectProvider {
        Dictionary<string, int> cluesCount;
        public int Count;

        public void Test() {
            
            dynamic eobj = new ExpandoObject();
            eobj.A = 3;
            //var dymObj = new DynamicObject();
            var a = new DynamicTest();
            dynamic dymP = new DymProvider();
            dymP.Count = 1;
        }

        public DynamicMetaObject GetMetaObject(Expression parameter) {
            throw new NotImplementedException();
        }
    }

    class DymProvider: DynamicObject {
        Dictionary<string, object> cluesCount;
        public int ComId;
        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            cluesCount[binder.Name] = "1";
            result = cluesCount;
            return base.TryGetMember(binder, out result);
        }
    }
}
