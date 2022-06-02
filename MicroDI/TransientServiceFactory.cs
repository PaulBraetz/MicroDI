using MicroDI.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MicroDI
{
	public readonly struct TransientServiceFactory : IServiceFactory
	{
		public TransientServiceFactory(IServiceFactoryInstructions instructions)
		{
			ConstructorInfo? ctorInfo = instructions.ServiceImplementationType.GetConstructors()
				.SingleOrDefault(isMatch);

			Boolean isMatch(ConstructorInfo constructorInfo)
			{
				ParameterInfo[] ctorParameters = constructorInfo.GetParameters();

				Type[] ctorParametersTypes = ctorParameters
					.Select(p => p.ParameterType)
					.ToArray();

				Type[] ctorArgsTypes = instructions.ConstructorArguments
					.Select(p => p.GetType())
					.ToArray();

				if (ctorParametersTypes.Length < ctorArgsTypes.Length)
				{
					return false;
				}

				for (int i = 0; i < ctorParametersTypes.Length; i++)
				{
					if (i < ctorArgsTypes.Length)
					{
						if (!ctorArgsTypes[i].IsAssignableTo(ctorParametersTypes[i]))
						{
							return false;
						}
					}
					else if (!(ctorParameters[i].Attributes.HasFlag(ParameterAttributes.Optional) || ctorParameters[i].Attributes.HasFlag(ParameterAttributes.HasDefault)))
					{
						return false;
					}
				}
				return true;
			}

			if (ctorInfo == null)
			{
				throw new ArgumentException($"{instructions.ServiceImplementationType.Name} does not provide a constructor like {instructions.ServiceImplementationType.Name}({String.Join(", ", instructions.ConstructorArguments.Select(p => p.GetType().Name))})");
			}

			IEnumerable<ConstantExpression>? ctorArgs = instructions.ConstructorArguments.Select(Expression.Constant);

			NewExpression? ctorExpr = Expression.New(ctorInfo, ctorArgs);

			LambdaExpression? ctor = Expression.Lambda(ctorExpr);

			factory = (Func<Object>)ctor.Compile();
		}

		private readonly Func<Object> factory;

		public override String ToString()
		{
			return "Transient Factory";
		}

		public Object BuildService()
		{
			return factory.Invoke();
		}
	}
}
