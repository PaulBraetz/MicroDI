using MicroDI.Abstractions;

namespace MicroDI
{
	public readonly struct ServiceRegistration : IServiceRegistration, IEquatable<ServiceRegistration>
	{
		public ServiceRegistration(IServiceDefinition definition, IServiceFactory factory)
		{
			Definition = definition ?? throw new ArgumentNullException(nameof(definition));
			Factory = factory ?? throw new ArgumentNullException(nameof(factory));
		}

		public readonly IServiceDefinition Definition { get; }
		public readonly IServiceFactory Factory { get; }

		public override Boolean Equals(Object? obj)
		{
			return obj is ServiceRegistration registration && Equals(registration);
		}

		public Boolean Equals(ServiceRegistration other)
		{
			return Definition.Equals(other.Definition);
		}

		public override Int32 GetHashCode()
		{
			return HashCode.Combine(Definition);
		}

		public static Boolean operator ==(ServiceRegistration left, ServiceRegistration right)
		{
			return left.Equals(right);
		}

		public static Boolean operator !=(ServiceRegistration left, ServiceRegistration right)
		{
			return !(left == right);
		}
	}
}
