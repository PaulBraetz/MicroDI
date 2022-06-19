using MicroDI.Abstractions;

namespace MicroDI
{
	public readonly struct ConstructorInjectionInstructions : IConstructorInjectionInstructions, IEquatable<ConstructorInjectionInstructions>
	{
		public ConstructorInjectionInstructions(Type serviceImplementationType, IEnumerable<Object> constructorArguments, IEnumerable<IServiceRegistration> constructorInjectionArguments)
		{
			InjectionInstructions = new InjectionInstructions(serviceImplementationType);
			ConstructorArguments = constructorArguments ?? throw new ArgumentNullException(nameof(constructorArguments));
			ConstructorInjectionArguments = constructorInjectionArguments ?? throw new ArgumentNullException(nameof(constructorInjectionArguments));
		}

		public static ConstructorInjectionInstructions Empty => new();

		private readonly IInjectionInstructions InjectionInstructions { get; }
		public readonly Type ServiceImplementationType => InjectionInstructions.ServiceImplementationType;
		public readonly IEnumerable<Object> ConstructorArguments { get; }
		public readonly IEnumerable<IServiceRegistration> ConstructorInjectionArguments { get; }

		public override String ToString()
		{
			return $"{InjectionInstructions}\nConstructor Arguments: {String.Join(", ", ConstructorArguments)}\nConstructor Injection Arguments: {String.Join(", ", ConstructorInjectionArguments)}";
		}

		public override Boolean Equals(Object? obj)
		{
			return obj is ConstructorInjectionInstructions instructions && Equals(instructions);
		}

		public Boolean Equals(ConstructorInjectionInstructions other)
		{
			return InjectionInstructions.Equals(other.InjectionInstructions) &&
				(ConstructorArguments?.SequenceEqual(other.ConstructorArguments) ?? other.ConstructorArguments == null) &&
				(ConstructorInjectionArguments?.SequenceEqual(other.ConstructorInjectionArguments) ?? other.ConstructorArguments == null);
		}

		public override Int32 GetHashCode()
		{
			return HashCode.Combine(InjectionInstructions, ConstructorArguments, ConstructorInjectionArguments);
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
