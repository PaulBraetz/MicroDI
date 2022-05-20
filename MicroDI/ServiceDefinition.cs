using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MicroDI.ServiceDefinitions;

namespace MicroDI
{
	public readonly struct ServiceDefinition : IServiceDefinition
	{
		public ServiceDefinition(Type serviceType, ServiceScope scope, params Object[] parameters):this(serviceType, serviceType, scope, parameters)
		{

		}
		public ServiceDefinition(Type serviceType, Type serviceImplementation, ServiceDefinitions.ServiceScope scope, params Object[] parameters)
		{
			ServiceType = serviceType;
			ServiceImplementation = serviceImplementation;
			Scope = scope;
			Arguments = parameters;
		}

		public readonly Type ServiceType { get; }
		public readonly Type ServiceImplementation { get; }
		public readonly IEnumerable<Object> Arguments { get; }
		public readonly ServiceDefinitions.ServiceScope Scope { get; }
	}
}
