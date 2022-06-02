namespace MicroDI.Abstractions
{
	public interface IServiceFactoryInstructions
	{
		Type ServiceImplementationType { get; }
		IEnumerable<Object> ConstructorArguments { get; }
	}
}
