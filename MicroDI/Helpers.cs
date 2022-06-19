using MicroDI.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MicroDI
{
	public static class Helpers
	{
		public static LambdaExpression GetDefaultConstructorLambda(IInjectionInstructions instructions)
		{
			return GetDefaultConstructorLambda(instructions.ServiceImplementationType);
		}
		public static LambdaExpression GetDefaultConstructorLambda(Type type)
		{
			ConstructorInfo? ctor = type.GetConstructor(Type.EmptyTypes);

			if (ctor == null) throw new ArgumentException($"{nameof(type)} does not provide a public default constructor.");

			return Expression.Lambda(Expression.New(ctor));
		}

		public static LambdaExpression GetConstructorInjectionConstructorLambda(IConstructorInjectionInstructions instructions)
		{
			return GetConstructorInjectionConstructorLambda(instructions.ServiceImplementationType, instructions.ConstructorArguments, instructions.ConstructorInjectionArguments);
		}
		public static LambdaExpression GetConstructorInjectionConstructorLambda(Type type, IEnumerable<Object> ctorArgs, IEnumerable<IServiceRegistration> dependencies)
		{
			ConstructorInfo[] constructors = type.GetConstructors();

			if (constructors.Length == 0)
			{
				throw new ArgumentException("No Constructor found.");
			}
			if (constructors.Length == 1)
			{
				return Helpers.GetConstructorInjectionConstructorLambda(constructors.Single(), ctorArgs, dependencies);
			}
			else
			{
				foreach (var ctorInfo in constructors)
				{
					try
					{
						return Helpers.GetConstructorInjectionConstructorLambda(ctorInfo, ctorArgs, dependencies);
					}
					catch
					{
						continue;
					}
				}
			}

			throw new ArgumentException("No Constructor found matching args and/or registrations.");
		}
		public static LambdaExpression GetConstructorInjectionConstructorLambda(ConstructorInfo ctorInfo, IEnumerable<Object> passedArgs, IEnumerable<IServiceRegistration> dependencies)
		{
			if (ctorInfo == null) throw new ArgumentNullException(nameof(ctorInfo));
			if (passedArgs == null) throw new ArgumentNullException(nameof(passedArgs));
			if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));

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
						UnaryExpression castCallExpr = GetBuildCallExpression(dependency);
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
		
		public static LambdaExpression GetParameterInjectionConstructorLambda(IPropertynInjectionInstructions instructions)
		{
			return GetParameterInjectionConstructorLambda(instructions.ServiceImplementationType, GetConstructorInjectionConstructorLambda(instructions), instructions.PropertyValues, instructions.PropertyInjectionValues);
		}
		public static LambdaExpression GetParameterInjectionConstructorLambda(Type type, IEnumerable<Object> values, IEnumerable<IServiceRegistration> dependencies)
		{
			LambdaExpression ctorCallLambda = GetDefaultConstructorLambda(type);

			var propInfos = new List<PropertyInfo>(type.GetProperties());

			var valueDict = new Dictionary<String, Object>();
			foreach (var value in values)
			{
				if (value == null) throw new ArgumentException($"Cannot match null value to property.");

				Type valueType = value.GetType();
				PropertyInfo? propInfo = propInfos.FirstOrDefault(p => p.PropertyType == valueType);

				if (propInfo == null) throw new ArgumentException($"{nameof(type)} does not provide a property of type {GetTypeString(valueType)} to match against value {value}.");

				valueDict.Add(propInfo.Name, value);
				propInfos.Remove(propInfo);
			}

			var dependencyDict = new Dictionary<String, IServiceRegistration>();
			foreach (var dependency in dependencies)
			{
				if (dependency == null) throw new ArgumentException($"Cannot match null dependency to property.");
				if (dependency.Definition == null) throw new ArgumentException($"Cannot match null dependency definition to property.");
				if (dependency.Definition.ServiceType == null) throw new ArgumentException($"Cannot match null dependency definition type to property.");

				Type serviceType = dependency.Definition.ServiceType;
				PropertyInfo? propInfo = propInfos.FirstOrDefault(p => p.PropertyType == serviceType);

				if (propInfo == null) throw new ArgumentException($"{nameof(type)} does not provide a property of type {GetTypeString(serviceType)} to match against dependency {dependency}.");

				dependencyDict.Add(propInfo.Name, dependency);
				propInfos.Remove(propInfo);
			}

			return GetParameterInjectionConstructorLambda(type, ctorCallLambda, valueDict, dependencyDict);
		}
		public static LambdaExpression GetParameterInjectionConstructorLambda(Type type, IDictionary<String, Object> values, IDictionary<String, IServiceRegistration> dependencies)
		{
			ConstructorInfo? ctor = type.GetConstructor(Type.EmptyTypes);

			if (ctor == null) throw new ArgumentException($"{nameof(type)} does not provide a public default constructor.");

			NewExpression ctorCallExpression = Expression.New(ctor);
			LambdaExpression ctorCallLambda = Expression.Lambda(ctorCallExpression);

			return GetParameterInjectionConstructorLambda(type, ctorCallLambda, values, dependencies);
		}
		public static LambdaExpression GetParameterInjectionConstructorLambda(Type type, LambdaExpression ctorCallExpression, IDictionary<String, Object> values, IDictionary<String, IServiceRegistration> dependencies)
		{
			Func<Object> ctorCallFunc = (Func<Object>)ctorCallExpression.Compile();
			MethodInfo ctorCallFuncInvoke = ctorCallFunc.GetType().GetMethod(nameof(Func<Object>.Invoke))!;

			ConstantExpression ctorCallFuncConstantExpression = Expression.Constant(ctorCallFunc);
			MethodCallExpression ctorCallFuncCallExpression = Expression.Call(ctorCallFuncConstantExpression, ctorCallFuncInvoke);

			ParameterExpression instanceExpression = Expression.Variable(type);
			BinaryExpression instanceAssignment = Expression.Assign(instanceExpression, ctorCallFuncCallExpression);

			ParameterExpression[] blockVariables = new[]
			{
				instanceExpression
			};
			var blockExpressions = new List<Expression>()
			{
				instanceAssignment
			};

			blockExpressions.AddRange(GetReverseSetterCalls(type, values, dependencies, instanceExpression));

			blockExpressions.Add(instanceExpression);

			BlockExpression block = Expression.Block(blockVariables, blockExpressions);

			return Expression.Lambda(block);
		}

		public static IEnumerable<LambdaExpression> GetPropertySettingExpressions(Type type, IDictionary<String, Object> values, IDictionary<String, IServiceRegistration> dependencies)
		{
			return GetReverseSetters(type, values, dependencies, GetReverseSetterCallLambda);
		}
		public static IEnumerable<MethodCallExpression> GetReverseSetterCalls(Type type, IDictionary<String, Object> values, IDictionary<String, IServiceRegistration> dependencies, Expression instanceExpression)
		{
			return GetReverseSetters(type, values, dependencies, (propInfo, argExpression) => GetReverseSetterCall(propInfo, argExpression, instanceExpression));
		}

		private static IEnumerable<TResult> GetReverseSetters<TResult>(Type type, IDictionary<String, Object> values, IDictionary<String, IServiceRegistration> dependencies, Func<PropertyInfo, Expression, TResult> resultFactory)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (values == null) throw new ArgumentNullException(nameof(values));
			if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
			if (dependencies.Any(kvp => values.ContainsKey(kvp.Key))) throw new ArgumentException($"{nameof(values)} and {nameof(dependencies)} must not have common keys.");

			var retVal = new List<TResult>();

			AddReverseSetterCalls(type, nameof(values), values, Expression.Constant, resultFactory, retVal);
			AddReverseSetterCalls(type, nameof(dependencies), dependencies, GetBuildCallExpression, resultFactory, retVal);

			return retVal;
		}
		private static void AddReverseSetterCalls<TValue, TResult>(Type type, String name, IDictionary<String, TValue> propertyValues, Func<TValue, Expression> selector, Func<PropertyInfo, Expression, TResult> resultFactory, ICollection<TResult> expressions)
		{
			foreach (KeyValuePair<String, TValue> kvp in propertyValues)
			{
				PropertyInfo? propInfo = type.GetProperty(kvp.Key);
				if (propInfo == null)
				{
					throw new ArgumentException($"{kvp.Key} in {name} could not be found as a property of {GetTypeString(type)}.");
				}
				Expression argExpression = selector.Invoke(kvp.Value);
				TResult callExpression = resultFactory.Invoke(propInfo, argExpression);
				expressions.Add(callExpression);
			}
		}

		public static UnaryExpression GetBuildCallExpression(IServiceRegistration? dependency)
		{
			if (dependency == null) throw new ArgumentNullException(nameof(dependency));
			if (dependency.Definition.ServiceType == null) throw new ArgumentNullException($"{nameof(dependency)}.{nameof(dependency.Definition)}.{nameof(dependency.Definition.ServiceType)}");

			ConstantExpression instanceExpr = Expression.Constant(dependency.Factory);
			MethodInfo factoryMethod = dependency.Factory.GetType().GetMethod(nameof(IServiceRegistration.Factory.BuildService))!;
			MethodCallExpression callExpr = Expression.Call(instanceExpr, factoryMethod);
			UnaryExpression castCallExpr = Expression.TypeAs(callExpr, dependency.Definition.ServiceType);
			return castCallExpr;
		}
		public static LambdaExpression GetReverseSetterCallLambda(PropertyInfo propInfo, Expression argExpression)
		{
			if (propInfo == null) throw new ArgumentNullException(nameof(propInfo));
			if (propInfo.GetIndexParameters().Any()) throw new ArgumentException($"{nameof(propInfo)} cannot be indexed.");
			if (propInfo.DeclaringType == null) throw new ArgumentException($"{nameof(propInfo)} must have declaring type.");
			if (propInfo.PropertyType != argExpression.Type) throw new ArgumentException($"Type of {nameof(argExpression)} does not match property type.");

			ParameterExpression instanceParameter = Expression.Parameter(propInfo.DeclaringType);

			MethodCallExpression? setterCall = GetReverseSetterCall(propInfo, argExpression, instanceParameter);

			return Expression.Lambda(setterCall, instanceParameter);
		}
		public static MethodCallExpression GetReverseSetterCall(PropertyInfo propInfo, Expression argExpression, Expression instanceExpression)
		{
			if (propInfo == null) throw new ArgumentNullException(nameof(propInfo));
			if (propInfo.GetIndexParameters().Any()) throw new ArgumentException($"{nameof(propInfo)} cannot be indexed.");
			if (propInfo.DeclaringType == null) throw new ArgumentException($"{nameof(propInfo)} must have declaring type.");
			if (propInfo.PropertyType != argExpression.Type) throw new ArgumentException($"Type of {nameof(argExpression)} does not match property type.");

			MethodInfo? setter = propInfo.GetSetMethod(false);

			if (setter == null) throw new ArgumentException($"{propInfo.Name} does not provide a public setter.");

			return Expression.Call(instanceExpression, setter, argExpression);
		}

		public static String GetTypeString(Type type)
		{
			var builder = new StringBuilder();

			GetTypeString(type, builder);

			return builder.ToString();
		}
		private static Regex TypeNamePattern = new Regex(@".*(?=`[0-9]+)");
		private static void GetTypeString(Type type, StringBuilder builder)
		{
			var typeName = type.IsGenericType ? TypeNamePattern.Match(type.Name).Value : type.Name;
			builder.Append(typeName);
			if (type.IsGenericType)
			{
				builder.Append("<");
				for (int i = 0; i < type.GenericTypeArguments.Length - 1; i++)
				{
					builder.Append(GetTypeString(type.GenericTypeArguments[i]))
						.Append(", ");
				}
				builder.Append(GetTypeString(type.GenericTypeArguments[type.GenericTypeArguments.Length - 1]));
				builder.Append(">");
			}
		}
	}
}
