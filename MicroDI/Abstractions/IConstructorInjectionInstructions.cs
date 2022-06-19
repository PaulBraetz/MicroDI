namespace MicroDI.Abstractions
{
	public interface IConstructorInjectionInstructions:IInjectionInstructions
	{
		IEnumerable<Object> ConstructorArguments { get; }
		IEnumerable<IServiceRegistration> ConstructorInjectionArguments { get; }
	}
}
