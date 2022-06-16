using MicroDI.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MicroDI
{
	public static class Helpers
	{
		public static LambdaExpression GetConstructorExpression(ConstructorInfo ctorInfo, IEnumerable<Object> passedArgs, IEnumerable<IServiceRegistration> dependencies)
		{
			var ctorParameters = ctorInfo.GetParameters();

			var ctorArgs = new Queue<Object>(passedArgs);

			var factoryExpressionArgs = new Queue<Expression>();

			foreach (var ctorParameter in ctorParameters)
			{
				var ctorParameterType = ctorParameter.ParameterType;

				if (ctorArgs.TryPeek(out var arg) && arg.GetType().IsAssignableTo(ctorParameterType))
				{
					factoryExpressionArgs.Enqueue(Expression.Constant(ctorArgs.Dequeue()));
				}
				else
				{
					var dependency = dependencies
							.FirstOrDefault(r => r.Definition.ServiceType?.IsAssignableTo(ctorParameterType) ?? false && String.Equals(r.Definition.ServiceName, ctorParameter.Name)) ??
						dependencies
							.FirstOrDefault(r => r.Definition.ServiceType?.IsAssignableTo(ctorParameterType) ?? false);
					if (dependency != null)
					{
						ConstantExpression instanceExpr = Expression.Constant(dependency.Factory);
						MethodInfo factoryMethod = dependency.Factory.GetType().GetMethod(nameof(IServiceRegistration.Factory.BuildService))!;
						MethodCallExpression callExpr = Expression.Call(instanceExpr, factoryMethod);
						UnaryExpression castCallExpr = Expression.TypeAs(callExpr, dependency.Definition.ServiceType!);
						factoryExpressionArgs.Enqueue(castCallExpr);
					}
					else //if (!(ctorParameter.Attributes.HasFlag(ParameterAttributes.Optional) || ctorParameter.Attributes.HasFlag(ParameterAttributes.HasDefault)))
					{
						throw new ArgumentException("Could not match arguments and/or dependencies to constructor parameters.");
					}
				}
			}

			if (ctorArgs.Any())
			{
				throw new ArgumentException("Too many arguments passed.");
			}

			NewExpression ctorExpr = Expression.New(ctorInfo, factoryExpressionArgs);

			return Expression.Lambda(ctorExpr);
		}
	}
}
