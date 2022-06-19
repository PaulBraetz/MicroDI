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
		private TransientServiceFactory(LambdaExpression lambda)
		{
			factory = (Func<Object>)lambda.Compile();
		}

		public TransientServiceFactory(IInjectionInstructions instructions)
			: this(Helpers.GetDefaultConstructorLambda(instructions)) { }
		public TransientServiceFactory(IConstructorInjectionInstructions instructions)
			: this(Helpers.GetConstructorInjectionConstructorLambda(instructions)) { }
		public TransientServiceFactory(IPropertynInjectionInstructions instructions)
			: this(Helpers.GetConstructorInjectionConstructorLambda(instructions)) { }

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
