using MicroDI.Abstractions;
using System.Collections;
using System.Collections.Concurrent;

namespace MicroDI
{
	public sealed class Container : IContainer
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
				throw new ArgumentException($"A Definition for {serviceRegistration.Definition} has already been registered.");
			}
		}

		public Object Resolve(IServiceDefinition serviceDefinition)
		{
			return registrations.TryGetValue(serviceDefinition, out IServiceFactory? factory) ?
				factory.BuildService() :
				throw new ArgumentException($"A Definition for {serviceDefinition} has not been registered.");
		}

		public Boolean IsRegistered(IServiceDefinition serviceDefinition)
		{
			return registrations.ContainsKey(serviceDefinition);
		}

		public IEnumerator<IServiceRegistration> GetEnumerator()
		{
			return registrations.Select(kvp => (IServiceRegistration)new ServiceRegistration(kvp.Key, kvp.Value)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private readonly ConcurrentDictionary<IServiceDefinition, IServiceFactory> registrations;
	}
}
