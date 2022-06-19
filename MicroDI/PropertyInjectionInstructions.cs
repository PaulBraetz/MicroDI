using MicroDI.Abstractions;

namespace MicroDI
{
	public readonly struct PropertyInjectionInstructions : IPropertynInjectionInstructions, IEquatable<PropertyInjectionInstructions>
	{
		public PropertyInjectionInstructions(Type serviceImplementationType,
											  IEnumerable<Object> constructorArguments,
											  IEnumerable<IServiceRegistration> constructorInjectionArguments,
											  IDictionary<String, Object> propertyValues,
											  IDictionary<String, IServiceRegistration> propertyInjectionValues)
		{
			ctorInjectionInstructions = new ConstructorInjectionInstructions(serviceImplementationType, constructorArguments, constructorInjectionArguments);
			PropertyValues = propertyValues ?? throw new ArgumentNullException(nameof(propertyValues));
			PropertyInjectionValues = propertyInjectionValues ?? throw new ArgumentNullException(nameof(propertyInjectionValues));
		}

		public static ConstructorInjectionInstructions Empty => new();

		private readonly IConstructorInjectionInstructions ctorInjectionInstructions { get; }
		public IEnumerable<Object> ConstructorArguments => ctorInjectionInstructions.ConstructorArguments;
		public IEnumerable<IServiceRegistration> ConstructorInjectionArguments => ctorInjectionInstructions.ConstructorInjectionArguments;
		public Type ServiceImplementationType => ctorInjectionInstructions.ServiceImplementationType;

		public readonly IDictionary<String, Object> PropertyValues { get; }
		public readonly IDictionary<String, IServiceRegistration> PropertyInjectionValues { get; }

		public override String ToString()
		{
			return $"{ctorInjectionInstructions}\nProperty Values: {String.Join(", ", PropertyValues.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}\nProperty Injection Values: {String.Join(", ", PropertyInjectionValues.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}";
		}

		public override Boolean Equals(Object? obj)
		{
			return obj is PropertyInjectionInstructions instructions && Equals(instructions);
		}

		public Boolean Equals(PropertyInjectionInstructions other)
		{
			return ctorInjectionInstructions.Equals(other.ctorInjectionInstructions) &&
				(PropertyValues?.SequenceEqual(other.PropertyValues) ?? other.PropertyValues == null) &&
				(PropertyInjectionValues?.SequenceEqual(other.PropertyInjectionValues) ?? other.PropertyInjectionValues == null);
		}

		public override Int32 GetHashCode()
		{
			return HashCode.Combine(ctorInjectionInstructions, PropertyValues, PropertyInjectionValues);
		}

		public static Boolean operator ==(PropertyInjectionInstructions left, PropertyInjectionInstructions right)
		{
			return left.Equals(right);
		}

		public static Boolean operator !=(PropertyInjectionInstructions left, PropertyInjectionInstructions right)
		{
			return !(left == right);
		}
	}
}
