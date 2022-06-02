using MicroDI.Abstractions;
using System.Collections;
using System.Collections.Concurrent;

namespace MicroDI
{
	public sealed partial class Container : IContainer
	{
		public Container(IEqualityComparer<IServiceDefinition> definitionComparer)
		{
			registrations = new(definitionComparer);
		}
		public Container() : this(new ServiceDefinitionEqualityComparer())
		{

		}
		public Container(IEqualityComparer<IServiceDefinition> definitionComparer, IEnumerable<IServiceRegistration> serviceRegistrations) : this(definitionComparer)
		{
			foreach (IServiceRegistration serviceRegistration in serviceRegistrations)
			{
				Add(serviceRegistration);
			}
		}
		public Container(IEnumerable<IServiceRegistration> serviceRegistrations) : this(new ServiceDefinitionEqualityComparer(), serviceRegistrations)
		{
		}

		public void Add(IServiceRegistration serviceRegistration)
		{
			if (!registrations.TryAdd(serviceRegistration.Definition, serviceRegistration.Factory))
			{
				throw new ArgumentException($"{serviceRegistration.Definition.ServiceType.Name} has already been registered.");
			}
		}

		public Object Resolve(IServiceDefinition serviceDefinition)
		{
			return registrations.TryGetValue(serviceDefinition, out IServiceFactory? factory) ?
				factory.BuildService() :
				throw new ArgumentException($"{serviceDefinition.ServiceType.Name} has not been registered.");
		}

		public Boolean IsRegistered(IServiceDefinition serviceDefinition)
		{
			return registrations.ContainsKey(serviceDefinition);
		}

		private readonly ConcurrentDictionary<IServiceDefinition, IServiceFactory> registrations;
	}
}
