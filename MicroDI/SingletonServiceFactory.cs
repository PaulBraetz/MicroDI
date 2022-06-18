using MicroDI.Abstractions;

namespace MicroDI
{
	public readonly struct SingletonServiceFactory : IServiceFactory
	{
		public SingletonServiceFactory(IConstructorInjectionInstructions instructions):this(instructions, Array.Empty<IServiceRegistration>()) { }
		public SingletonServiceFactory(IConstructorInjectionInstructions instructions, IEnumerable<IServiceRegistration> dependencies)
		{
			factory = new TransientServiceFactory(instructions, dependencies);
			instance = new Lazy<Object>(factory.BuildService);
		}

		private readonly IServiceFactory factory;
		private readonly Lazy<Object> instance;

		public override String ToString()
		{
			return "Singleton Service Factory";
		}

		public Object BuildService()
		{
			return instance.Value;
		}
	}
}
