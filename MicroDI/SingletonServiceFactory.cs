using MicroDI.Abstractions;
using System.Linq.Expressions;

namespace MicroDI
{
	public readonly struct SingletonServiceFactory : IServiceFactory
	{
		private SingletonServiceFactory(LambdaExpression lambda)
		{
			instance = new Lazy<Object>((Func<Object>)lambda.Compile());
		}

		public SingletonServiceFactory(IInjectionInstructions instructions)
			: this(Helpers.GetDefaultConstructorLambda(instructions)) { }
		public SingletonServiceFactory(IConstructorInjectionInstructions instructions)
			: this(Helpers.GetConstructorInjectionConstructorLambda(instructions)) { }
		public SingletonServiceFactory(IPropertynInjectionInstructions instructions)
			: this(Helpers.GetConstructorInjectionConstructorLambda(instructions)) { }

		private readonly Lazy<Object> instance;

		public override String ToString()
		{
			return "Singleton Service Factory";
		}

		public Object BuildService()
		{
			return instance?.Value ?? throw new InvalidOperationException("Factory has not been initialized correctly. Use one of the available constructors.");
		}
	}
}
