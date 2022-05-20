namespace MicroDI
{
	public interface IContainerFactory:IEnumerable<IServiceDefinition>
	{
		void Add(IServiceDefinition serviceDefinition);
		IContainer Build();
	}
}
