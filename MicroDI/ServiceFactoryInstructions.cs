using MicroDI.Abstractions;

namespace MicroDI
{
	public readonly struct ServiceFactoryInstructions : IServiceFactoryInstructions, IEquatable<ServiceFactoryInstructions>
	{
		public ServiceFactoryInstructions(Type serviceImplementationType, IEnumerable<Object> constructorArguments)
		{
			ServiceImplementationType = serviceImplementationType ?? throw new ArgumentNullException(nameof(serviceImplementationType));
			ConstructorArguments = constructorArguments ?? throw new ArgumentNullException(nameof(constructorArguments));
		}

		public readonly Type ServiceImplementationType { get; }
		public readonly IEnumerable<Object> ConstructorArguments { get; }

		public override Boolean Equals(Object? obj)
		{
			return obj is ServiceFactoryInstructions instructions && Equals(instructions);
		}

		public Boolean Equals(ServiceFactoryInstructions other)
		{
			return ServiceImplementationType == other.ServiceImplementationType && ConstructorArguments.SequenceEqual(other.ConstructorArguments);
		}

		public override Int32 GetHashCode()
		{
			return HashCode.Combine(ServiceImplementationType, ConstructorArguments);
		}

		public static Boolean operator ==(ServiceFactoryInstructions left, ServiceFactoryInstructions right)
		{
			return left.Equals(right);
		}

		public static Boolean operator !=(ServiceFactoryInstructions left, ServiceFactoryInstructions right)
		{
			return !(left == right);
		}
	}
}
