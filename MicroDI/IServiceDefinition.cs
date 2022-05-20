namespace MicroDI
{
	public interface IServiceDefinition
	{
		Type ServiceType { get; }
		Type ServiceImplementation { get; }
		IEnumerable<Object> Arguments { get; }
		ServiceDefinitions.ServiceScope Scope { get;}
	}
	public static class ServiceDefinitions
	{
		public enum ServiceScope
		{
			Transient,
			Singleton
		}
	}
}
