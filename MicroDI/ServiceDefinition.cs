using MicroDI.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDI
{
	public readonly struct ServiceDefinition : IServiceDefinition, IEquatable<ServiceDefinition>
	{
		public ServiceDefinition(Type serviceType, String? serviceName = null)
		{
			ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
			ServiceName = serviceName;
		}

		public readonly Type ServiceType { get; }
		public readonly String? ServiceName { get; }

		public override String ToString()
		{
			return ServiceName != null ? $"Name: {ServiceName}, Type: {ServiceType.Name}" : $"Type: {ServiceType.Name}";
		}

		public override Boolean Equals(Object? obj)
		{
			return obj is ServiceDefinition definition && Equals(definition);
		}

		public Boolean Equals(ServiceDefinition other)
		{
			return ServiceDefinitionEqualityComparer.Instance.Equals(this, other);
		}

		public override Int32 GetHashCode()
		{
			return ServiceDefinitionEqualityComparer.Instance.GetHashCode(this);
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
