using System;
using System.Linq.Expressions;
using MuonLab.Validation;

namespace MustardBlack.Handlers.Binding
{
	public sealed class ViolationPropertyNameResolver : IViolationPropertyNameResolver
	{
		public string ResolvePropertyName(IViolation violation)
		{
			var expression = violation.Property as LambdaExpression;

			var propertyName = ReflectionHelper.PropertyChainToString(violation.Property, '.');

			if (!propertyName.EndsWith(".Value"))
				return propertyName;

			var memberExpression = expression.Body as MemberExpression;
			if (!memberExpression.Member.ReflectedType.IsGenericType || memberExpression.Member.ReflectedType.GetGenericTypeDefinition() != typeof(Nullable<>))
				return propertyName;

			return propertyName.Substring(0, propertyName.Length - 6);
		}
	}
}