namespace MicroDI
{
	public interface IServiceDefinition
	{
		public enum Scope
		{
			Transient,
			Singleton
		}
		Type ServiceType { get; }
		Type ServiceImplementation { get; }
		Scope ServiceScope { get; }
		IEnumerable<Object> ConstructorArguments { get; }
	}
}
