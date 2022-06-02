using MicroDI.Abstractions;

namespace MicroDI
{
	public readonly struct SingletonServiceFactory : IServiceFactory
	{
		public SingletonServiceFactory(IServiceFactoryInstructions instructions)
		{
			factory = new TransientServiceFactory(instructions);
			instance = new Lazy<Object>(factory.BuildService);
		}

		private readonly IServiceFactory factory;
		private readonly Lazy<Object> instance;

		public override String ToString()
		{
			return "Singleton Factory";
		}

		public Object BuildService()
		{
			return instance.Value;
		}
	}
}
