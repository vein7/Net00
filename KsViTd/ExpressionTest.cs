using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd {
    static class ExpressionTest {
        public static void BiBc() {
            var a = 3;
            Expression<Func<int, int>> fn1 = (a1) => a1 + a;

        }
        class User {
            public int Id;
            public string Name;
        }

        public static void Modify1() {
            var id = 10;
            Expression<Func<User, bool>> expr = f => f.Id == id || f.Name == "ss";
            Expression<Func<User, bool>> other = u => u.Name == "ds";
            var expr2 = Expression.Lambda(expr.Body, Expression.Parameter(expr.Parameters[0].Type, "f2"));

            var pm = new ParameterUpdate(expr, "f3");
            var expr3 = pm.Visit(expr);

            var modifier = new MyVisitor();
            expr = (Expression<Func<User, bool>>)modifier.Modify(expr, other);
            var fn1 = expr.Compile();
        }

        public class MyVisitor : ExpressionVisitor {
            private LambdaExpression visitor;
            public Expression Modify(Expression expression, LambdaExpression visitor) {
                this.visitor = visitor;
                return Visit(expression);
            }

            protected override Expression VisitBinary(BinaryExpression b) {
                var binary = visitor.Body as BinaryExpression;

                return Expression.MakeBinary(ExpressionType.AndAlso, b, binary);
            }
        }

        public class ParameterUpdate: ExpressionVisitor {
            //public static ParameterUpdate P = new ParameterUpdate();
            public LambdaExpression lambda;
            public ParameterExpression[] newParam;

            public ParameterUpdate(LambdaExpression lambda, params string[] newParam) {
                if (newParam.Length < lambda.Parameters.Count) { 
                    throw new ArgumentException("newParam.Length < lambda.Parameters.Count"); 
                }
                this.lambda = lambda;
                this.newParam = lambda.Parameters.Zip(newParam, (p, s) => Expression.Parameter(p.Type, s)).ToArray();
            }

            protected override Expression VisitParameter(ParameterExpression node) {
                var i = lambda.Parameters.IndexOf(node);
                return i < 0 ? node : newParam[i];
            }

        }

    }
}

