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
			if (!String.IsNullOrEmpty(x.ServiceName) && !String.IsNullOrEmpty(y.ServiceName))
			{
				return String.Equals(x.ServiceName, y.ServiceName);
			}
			if (x.ServiceType != null && y.ServiceType != null)
			{
				return x.ServiceType == y.ServiceType;
			}
			return false;
		}

		public Int32 GetHashCode([DisallowNull] IServiceDefinition obj)
		{
			return obj.ServiceName?.GetHashCode() ??
				obj.ServiceType?.GetHashCode() ??
				throw new ArgumentException($"{nameof(obj)}.{nameof(IServiceDefinition.ServiceName)} or {nameof(obj)}.{nameof(IServiceDefinition.ServiceName)} must be assigned in order to calculate a hashcode.");
		}
	}
}
