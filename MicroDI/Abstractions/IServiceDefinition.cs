namespace MicroDI.Abstractions
{
	public interface IServiceDefinition
	{
		String? ServiceName { get; }
		Type? ServiceType { get; }
	}
}
