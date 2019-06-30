using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd {

    [AttributeUsage(AttributeTargets.Property| AttributeTargets.ReturnValue| AttributeTargets.Field)]
    class FieldMaxLenAttribute : Attribute {
        public int MaxLen;
    }
    class FjUeObj {
        public Guid Guid { get; private set; }

        [FieldMaxLen(MaxLen = 30)]
        public string Name { get; set; }
        [FieldMaxLen(MaxLen = 40)]
        public string Name2 { get; set; }
        [FieldMaxLen(MaxLen = 30)]
        public string Name3 { get; set; }
        public int Qty;
        public decimal Unit;

        public int ReadOnly { get; set; }

        public decimal Amount => Qty * Unit;
                
        [return: FieldMaxLen(MaxLen = 60)]
        public string Info(string s) {
            
            return s + $" [Name:{Name}, Qty:{Qty}, Unit:{Unit}]";
        }

        public static string StaticMethod() {
            return "sss";
        }

        public void VoidMethod() {
            Console.WriteLine("VoidMethod");
        }

        public Delegate GetDele(string methodName) {
            switch (methodName) {
                case nameof(VoidMethod):
                    return (Action)VoidMethod;
                case nameof(StaticMethod):
                    return (Func<string>)StaticMethod;
                default:
                    throw new ArgumentException(" 没有找到匹配项 ", nameof(methodName));
            }
        }


    }

    [Flags]
    enum Accounts {
        Saving = 1,
        Checking = 1 << 1,
        Brokerage = 1 << 2
    }

    // 382

    class AccountsAttrbute : Attribute {
        public readonly Accounts Accounts;
        public AccountsAttrbute(Accounts accounts) {
            Accounts = accounts;
        }

        public override bool Match(object obj) {
            if(obj is AccountsAttrbute other) {
                // other 账户是否是 this 账户的子集
                return ((other.Accounts & this.Accounts) != this.Accounts);
            }
            return false;
        }

        public override bool Equals(object obj) {
            if(obj is AccountsAttrbute other) { return other.Accounts == this.Accounts; }
            return false;
        }
        // 因为重写了 Equals 方法
        public override int GetHashCode() {
            return (int)this.Accounts;
        }
    }


    class Order<T> {

    }
    
    class FjUe {
        
        public FjUe() {

            //Expression1();
            //FjXk1();
            Attribute1();
            //TeXkMatch();

        }


        #region 反射 Emit
        // 优化反射性能的总结 http://www.cnblogs.com/fish-li/archive/2013/02/18/2916253.html
        // [2018/4/22 16:33:24], 没看懂, 主要是通过 Emit 生成IL代码

        public delegate void SetValueDelegate(object target, object arg);

        public static class DynamicMethodFactory {
            public static SetValueDelegate CreatePropertySetter(PropertyInfo property) {
                if (property == null) { throw new ArgumentNullException("property"); }

                if (!property.CanWrite) { return null; }

                MethodInfo setMethod = property.GetSetMethod(true);

                DynamicMethod dm = new DynamicMethod("PropertySetter", null
                    , new Type[] { typeof(object), typeof(object) }
                    , property.DeclaringType, true);

                ILGenerator il = dm.GetILGenerator();

                if (!setMethod.IsStatic) {
                    il.Emit(OpCodes.Ldarg_0);
                }
                il.Emit(OpCodes.Ldarg_1);

                EmitCastToReference(il, property.PropertyType);
                if (!setMethod.IsStatic && !property.DeclaringType.IsValueType) {
                    il.EmitCall(OpCodes.Callvirt, setMethod, null);
                } else {
                    il.EmitCall(OpCodes.Call, setMethod, null);
                }

                il.Emit(OpCodes.Ret);

                return (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
            }

            private static void EmitCastToReference(ILGenerator il, Type type) {
                if (type.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, type);
                else
                    il.Emit(OpCodes.Castclass, type);
            }
        }
        #endregion

        #region 委托  
        public void WwTo() {
            //Delegate.CreateDelegate();
        }

        //泛型反射
        FjUe FjXk1() {
            Type type = typeof(Action<,>).MakeGenericType(typeof(FjUeObj), typeof(Guid));
            Delegate dele = Delegate.CreateDelegate(type, typeof(FjUeObj).GetProperty("Guid").SetMethod);
            Console.WriteLine("dele is Action<FjUeObj,Guid> ? " + (dele is Action<FjUeObj, Guid>));
            var obj = new FjUeObj();
            ((Action<FjUeObj, Guid>)dele)(obj, Guid.NewGuid());

            return this;
        }
        #endregion

        #region 特性

        public FjUe Attribute1() {
            var pGuid = typeof(FjUeObj).GetProperty("Guid");

            var p1 = typeof(FjUeObj).GetProperty("Name");
            var cAttribute1 = p1.GetCustomAttributesData();
            var cAttribute2 = p1.GetCustomAttribute(typeof(FieldMaxLenAttribute));
            var cAttribute4 = p1.GetCustomAttributesData();
            
            var cAttribute3 = p1.GetCustomAttribute<FieldMaxLenAttribute>();

            return this;
        }


        public void TeXk() {
            var p1 = typeof(FjUeObj).GetProperty("Name");
            p1.IsDefined(typeof(FieldMaxLenAttribute));     // 优先使用 IsDefined，他不会构造特性实例
            var attr1 = p1.GetCustomAttribute(typeof(FieldMaxLenAttribute));
            var attr2 = p1.GetCustomAttributes();
            var typeAttr = typeof(FjUeObj).GetCustomAttributes();
            var typeAttr2 = typeof(FjUeObj).GetCustomAttributes<Attribute>();

        }

        public void TeXkMatch() {
            var nameAttr = typeof(FjUeObj).GetMember(nameof(FjUeObj.Name))[0].GetCustomAttribute<FieldMaxLenAttribute>();
            var name2Attr = typeof(FjUeObj).GetMember(nameof(FjUeObj.Name2))[0].GetCustomAttribute<FieldMaxLenAttribute>();
            var name3Attr = typeof(FjUeObj).GetMember(nameof(FjUeObj.Name3))[0].GetCustomAttribute<FieldMaxLenAttribute>();
            var b = nameAttr.Equals(name2Attr);     // 基类 Attribute 重写了 Equals 函数，会反射比较实例内容

            var infoAttr = typeof(FjUeObj).GetMethod(nameof(FjUeObj.Info)).GetCustomAttribute<FieldMaxLenAttribute>();


        }

        public void TeXk2 (){
            // CustomAttributeData 该类在查找特性的同时禁止执行特性类中的代码
            var a = CustomAttributeData.GetCustomAttributes(typeof(FjUeObj));

        }


        #endregion

        #region Expression

        /// <summary>
        /// 字符串连接
        /// </summary>
        /// <returns></returns>
        FjUe Expression1() {
            var pDate1 = Expression.Parameter(typeof(DateTime), "date1");
            var pStr = Expression.Parameter(typeof(string), "str");

            // string.Concat(date1.ToString(),str)
            var body = Expression.Call(
                typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) })
                , Expression.Call(pDate1
                    , typeof(DateTime).GetMethod("ToString", new Type[] { }))
                , pStr);

            var fun = Expression.Lambda<Func<DateTime, string, string>>(body, pDate1, pStr);

            return this;
        }

        FjUe Expression2() {
            var methodInfo = MethodToDelegate(typeof(FjUeObj).GetMethod("VoidMethod"));

            return this;
        }
        
        FjUe Expression3() {
            var pInstance = Expression.Parameter(typeof(FjUeObj), "instance");
            var body = Expression.Assign(
                Expression.Property(pInstance, "Name"), Expression.Constant("111"));
            var lambda = Expression.Lambda<Action<FjUeObj>>(body, pInstance);
            var fun = lambda.Compile();

            var fjueObj = new FjUeObj { Name = "AA", Qty = 20 };
            fun(fjueObj);

            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        /// [2018/4/22 16:23:55]
        Func<object, object[], object> GetExecutor(MethodInfo methodInfo) {

            var pInstance = Expression.Parameter(typeof(object), "instance");
            var pParameters = Expression.Parameter(typeof(object[]), "parameters");

            //((T)instance).Method((T)(parameters[0]),(T)(parameters[1]...)
            var callMethod = Expression.Call(
                methodInfo.IsStatic ? null : Expression.Convert(pInstance, methodInfo.ReflectedType)     // IsStatic ? null : (T)instance
                , methodInfo
                , methodInfo.GetParameters().Select((p, i) => {
                    //(T)(parameters[i])
                    return Expression.Convert(
                        Expression.ArrayIndex(pParameters, Expression.Constant(i))
                        , p.ParameterType);
                }).ToArray());

            if (methodInfo.ReturnType == typeof(void)) {
                var execute = Expression
                    .Lambda<Action<object, object[]>>(callMethod, pInstance, pParameters)
                    .Compile();

                return (instance, parameters) => {
                    execute(instance, parameters);
                    return null;
                };
            } else{
                return Expression
                    .Lambda<Func<object, object[], object>>(callMethod, pInstance, pParameters)
                    .Compile();
            }
            
        }

        TDelegate GetExecutor<TDelegate> (MethodInfo methodInfo) {
            var pInstance = Expression.Parameter(typeof(object), "instance");
            var pParameters = Expression.Parameter(typeof(object[]), "parameters");


            //((T)instance).Method((T)(parameters[0]),(T)(parameters[1]...)
            var callMethod = Expression.Call(
                methodInfo.IsStatic ? null : Expression.Convert(pInstance, methodInfo.ReflectedType)     // IsStatic ? null : (T)instance
                , methodInfo
                , methodInfo.GetParameters().Select((p, i) => {
                    //(T)(parameters[i])
                    return Expression.Convert(
                        Expression.ArrayIndex(pParameters, Expression.Constant(i))
                        , p.ParameterType);
                }).ToArray());

            if (methodInfo.ReturnType == typeof(void)) {
                return Expression
                    .Lambda<TDelegate>(callMethod, pParameters)
                    .Compile();
                
            } else {
                return Expression
                    .Lambda<TDelegate>(callMethod, pInstance, pParameters)
                    .Compile();
            }
        }

        Delegate MethodToDelegate(MethodInfo method) {
            // param0, param1...
            var pParams = method.GetParameters()
                .Select((p, i) => Expression.Parameter(p.ParameterType, "param" + i.ToString()))  
                .ToArray();

            if (method.IsStatic) {
                // (param0, param1...) => Method(param0, param1...)
                return Expression
                    .Lambda(Expression.Call(method, pParams), pParams)
                    .Compile();
            } else {
                // (instance, param0, param1...) => instance.Method(param0, param1...)
                var pInstance = Expression.Parameter(method.ReflectedType, "instance");
                return Expression
                    .Lambda(Expression.Call(pInstance, method, pParams), new[] { pInstance }.Concat(pParams))
                    .Compile();
            } 
        }

        private Func<object, object[], object> GetExecuteDelegate(MethodInfo methodInfo) {
            // 赵劼  http://www.cnblogs.com/JeffreyZhao/archive/2008/11/24/1338682.html

            // parameters to execute
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            List<Expression> parameterExpressions = new List<Expression>();
            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++) {
                // (Ti)parameters[i]
                BinaryExpression valueObj = Expression.ArrayIndex(
                    parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(
                    valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            // non-instance for static method, or ((TInstance)instance)
            Expression instanceCast = methodInfo.IsStatic ? null :
                Expression.Convert(instanceParameter, methodInfo.ReflectedType);

            // static invoke or ((TInstance)instance).Method
            MethodCallExpression methodCall = Expression.Call(
                instanceCast, methodInfo, parameterExpressions);

            // ((TInstance)instance).Method((T0)parameters[0], (T1)parameters[1], ...)
            if (methodCall.Type == typeof(void)) {
                Expression<Action<object, object[]>> lambda =
                    Expression.Lambda<Action<object, object[]>>(
                        methodCall, instanceParameter, parametersParameter);

                Action<object, object[]> execute = lambda.Compile();
                return (instance, parameters) =>
                {
                    execute(instance, parameters);
                    return null;
                };
            } else {
                UnaryExpression castMethodCall = Expression.Convert(
                    methodCall, typeof(object));
                Expression<Func<object, object[], object>> lambda =
                    Expression.Lambda<Func<object, object[], object>>(
                        castMethodCall, instanceParameter, parametersParameter);

                return lambda.Compile();
            }
        }



        #endregion

    }


}
