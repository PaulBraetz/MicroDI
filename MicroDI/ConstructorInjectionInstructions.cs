using MicroDI.Abstractions;

namespace MicroDI
{
	public readonly struct ConstructorInjectionInstructions : IConstructorInjectionInstructions, IEquatable<ConstructorInjectionInstructions>
	{
		public ConstructorInjectionInstructions(Type serviceImplementationType, IEnumerable<Object> constructorArguments)
		{
			ServiceImplementationType = serviceImplementationType ?? throw new ArgumentNullException(nameof(serviceImplementationType));
			ConstructorArguments = constructorArguments ?? throw new ArgumentNullException(nameof(constructorArguments));
		}

		public readonly Type ServiceImplementationType { get; }
		public readonly IEnumerable<Object> ConstructorArguments { get; }

		public override String ToString()
		{
			return $"Type: {Helpers.GetTypeString(ServiceImplementationType)}, Arguments: {String.Join(",", ConstructorArguments)}";
		}

		public override Boolean Equals(Object? obj)
		{
			return obj is ConstructorInjectionInstructions instructions && Equals(instructions);
		}

		public Boolean Equals(ConstructorInjectionInstructions other)
		{
			return ServiceImplementationType == other.ServiceImplementationType && ConstructorArguments.SequenceEqual(other.ConstructorArguments);
		}

		public override Int32 GetHashCode()
		{
			return HashCode.Combine(ServiceImplementationType, ConstructorArguments);
		}

		public static Boolean operator ==(ConstructorInjectionInstructions left, ConstructorInjectionInstructions right)
		{
			return left.Equals(right);
		}

		public static Boolean operator !=(ConstructorInjectionInstructions left, ConstructorInjectionInstructions right)
		{
			return !(left == right);
		}
	}
}
