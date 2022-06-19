using MicroDI.Abstractions;

namespace MicroDI
{
	public readonly struct InjectionInstructions : IInjectionInstructions, IEquatable<InjectionInstructions>
	{
		public InjectionInstructions(Type serviceImplementationType)
		{
			ServiceImplementationType = serviceImplementationType;
		}

		public static InjectionInstructions Empty => new();

		public readonly Type ServiceImplementationType { get; }

		public override String ToString()
		{
			return $"Service Implementation Type: {Helpers.GetTypeString(ServiceImplementationType)}";
		}

		public override Boolean Equals(Object? obj)
		{
			return obj is InjectionInstructions instructions && Equals(instructions);
		}

		public Boolean Equals(InjectionInstructions other)
		{
			return ServiceImplementationType == other.ServiceImplementationType;
		}

		public override Int32 GetHashCode()
		{
			return HashCode.Combine(ServiceImplementationType);
		}

		public static Boolean operator ==(InjectionInstructions left, InjectionInstructions right)
		{
			return left.Equals(right);
		}

		public static Boolean operator !=(InjectionInstructions left, InjectionInstructions right)
		{
			return !(left == right);
		}
	}
}
