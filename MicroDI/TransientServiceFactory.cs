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
		public TransientServiceFactory(IServiceFactoryInstructions instructions) : this(instructions, Array.Empty<IServiceRegistration>()) { }
		public TransientServiceFactory(IServiceFactoryInstructions instructions, IEnumerable<IServiceRegistration> registrations)
		{
			LambdaExpression? ctorExpression = null;

			var constructors = instructions.ServiceImplementationType.GetConstructors();

			if (constructors.Length == 0)
			{	
				throw new ArgumentException("No Constructor found.");
			}
			if(constructors.Length == 1)
			{
				ctorExpression = Helpers.GetConstructorExpression(constructors.Single(), instructions.ConstructorArguments, registrations);
			}
			else
			{
				foreach (var ctorInfo in instructions.ServiceImplementationType.GetConstructors())
				{
					try
					{
						ctorExpression = Helpers.GetConstructorExpression(ctorInfo, instructions.ConstructorArguments, registrations);
						break;
					}
					catch
					{
						continue;
					}
				}
			}

			if (ctorExpression == null)
			{
				throw new ArgumentException("No Constructor found matching args and/or registrations.");
			}

			factory = (Func<Object>)ctorExpression.Compile();
		}

		private readonly Func<Object> factory;

		public override String ToString()
		{
			return "Transient Service Factory";
		}

		public Object BuildService()
		{
			return factory.Invoke();
		}
	}
}
