using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Minimod.ExpressionExtensions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Converts an expression to the name of the property returned by the expression.
        /// 
        /// Examples:
        /// (() => myObject.MyProperty).PropertyName() == "MyProperty"
        /// (() => myObject.MyComplexProperty.MyProperty).PropertyName() == "MyProperty"
        /// </summary>
        /// <typeparam name="TProperty">the type of the property</typeparam>
        /// <param name="projection">The expression that refers to the property</param>
        public static string PropertyName<TProperty>(this Expression<Func<TProperty>> projection)
        {
            MemberExpression memberExpression = (MemberExpression)projection.Body;
            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Converts an expression to the full path to the property returned by the expression.
        /// 
        /// Examples:
        /// (() => myObject.MyProperty).PropertyName() == "MyProperty"
        /// (() => myObject.MyComplexProperty.MyProperty).PropertyName() == "MyComplexProperty.MyProperty"
        /// </summary>
        /// <typeparam name="TProperty">the type of the property</typeparam>
        /// <param name="projection">The expression that refers to the property</param>
        public static string PropertyNameRecursive<TProperty>(this Expression<Func<TProperty>> projection)
        {
            List<string> names = new List<string>();
            MemberExpression memberExpression = (MemberExpression)projection.Body;


            while (memberExpression != null)
            {
                names.Add(memberExpression.Member.Name);
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            names.Reverse();

            var name = names.Aggregate((l, r) => l + "." + r);
            return name;
        }

        /// <summary>
        /// Reads the value of an objects property using an expression to the full path to the property.
        /// 
        /// Example:
        /// myObject.MyProperty = 17;
        /// (() => myObject.MyProperty).GetValue(myObject) == 17
        /// </summary>
        /// <typeparam name="TObject">the type of the object that contains the property</typeparam>
        /// <typeparam name="TProperty">the type of the property</typeparam>
        /// <param name="projection">The expression that refers to the property</param>
        /// <param name="obj">The object from which the value will be read</param>
        public static TProperty GetValue<TObject, TProperty>(this Expression<Func<TObject, TProperty>> projection, TObject obj)
        {
            return projection.Compile()(obj);
        }
    }
}
