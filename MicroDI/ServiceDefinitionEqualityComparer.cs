using MicroDI.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace MicroDI
{
	public sealed class ServiceDefinitionEqualityComparer : IEqualityComparer<IServiceDefinition>
	{
		public static readonly ServiceDefinitionEqualityComparer Instance = new();

		public Boolean Equals(IServiceDefinition? x, IServiceDefinition? y)
		{
			if (x == null)
			{
				return y == null;
			}
			if (y == null)
			{
				return x == null;
			}
			return String.Equals(x.ServiceName, y.ServiceName) && Object.ReferenceEquals(x.ServiceType, y.ServiceType);
		}

		public Int32 GetHashCode([DisallowNull] IServiceDefinition obj)
		{
			return HashCode.Combine(obj.ServiceName, obj.ServiceType);
		}
	}
}
