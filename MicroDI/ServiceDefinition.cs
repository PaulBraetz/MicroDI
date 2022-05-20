using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MicroDI.IServiceDefinition;

namespace MicroDI
{
	public readonly struct ServiceDefinition : IServiceDefinition
	{
		public ServiceDefinition(Type serviceType, Scope serviceScope, params Object[] parameters):this(serviceType, serviceType, serviceScope, parameters)
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
	}
}
