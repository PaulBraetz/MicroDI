using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MicroDI.IServiceDefinition;

namespace MicroDI
{
	public readonly struct ServiceDefinition : IServiceDefinition, IEquatable<ServiceDefinition>
	{
		public ServiceDefinition(Type serviceType, Scope serviceScope, params Object[] parameters) : this(serviceType, serviceType, serviceScope, parameters)
		{

		}
		public ServiceDefinition(Type serviceType, Type serviceImplementation, Scope serviceScope, params Object[] parameters)
		{
			ServiceType = serviceType;
			ServiceImplementation = serviceImplementation;
			ServiceScope = serviceScope;
			ConstructorArguments = parameters;
		}

		public readonly Type ServiceType { get; }
		public readonly Type ServiceImplementation { get; }
		public readonly Scope ServiceScope { get; }
		public readonly IEnumerable<Object> ConstructorArguments { get; }

		public override Boolean Equals(Object? obj)
		{
			return obj is ServiceDefinition definition && Equals(definition);
		}

		public Boolean Equals(ServiceDefinition other)
		{
			return ServiceType == other.ServiceType;
		}

		public override Int32 GetHashCode()
		{
			return HashCode.Combine(ServiceType);
		}

		public override String? ToString()
		{
			return $"{ServiceType.Name}->{ServiceImplementation.Name} [{ServiceScope}][{String.Join(", ", ConstructorArguments)}]";
		}

		public static Boolean operator ==(ServiceDefinition left, ServiceDefinition right)
		{
			return left.Equals(right);
		}

		public static Boolean operator !=(ServiceDefinition left, ServiceDefinition right)
		{
			return !(left == right);
		}
	}
}
