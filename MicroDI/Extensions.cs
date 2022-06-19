using MicroDI.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDI
{
	public static class Extensions
	{
		public static ServiceRegistrationBuilder SetServiceType<TService>(this ServiceRegistrationBuilder builder)
		{
			return builder.SetServiceType(typeof(TService));
		}
		public static ServiceRegistrationBuilder SetServiceImplementationType<TImplementation>(this ServiceRegistrationBuilder builder)
		{
			return builder.SetServiceImplementationType(typeof(TImplementation));
		}

		public static IContainer Add<TService>(this IContainer container)
		{
			return container.Add<TService, TService>();
		}
		public static IContainer Add<TService, TImplementation>(this IContainer container)
		{
			return container.Add(b => b.SetServiceType<TService>().SetServiceImplementationType<TImplementation>().BuildServiceRegistration());
		}
		public static IContainer Add(this IContainer container, Action<ServiceRegistrationBuilder> build)
		{
			var builder = new ServiceRegistrationBuilder();

			build.Invoke(builder);

			container.Add(builder.BuildServiceRegistration());

			return container;
		}

		public static Object Resolve(this IContainer container, Type serviceType, String? serviceName)
		{
			return container.Resolve(new ServiceDefinition(serviceType, serviceName));
		}

		public static TService Resolve<TService>(this IContainer container, String? serviceName)
		{
			return (TService)container.Resolve(typeof(TService), serviceName);
		}
		public static Object Resolve(this IContainer container, Type serviceType)
		{
			return container.Resolve(serviceType, null);
		}
		public static TService Resolve<TService>(this IContainer container)
		{
			return (TService)container.Resolve(typeof(TService));
		}

		public static Boolean IsRegistered(this IContainer container, Type serviceType, String serviceName)
		{
			return container.IsRegistered(new ServiceDefinition(serviceType, serviceName));
		}
		public static Boolean IsRegistered<TService>(this IContainer container, String serviceName)
		{
			return container.IsRegistered(typeof(TService), serviceName);
		}

		public static Boolean IsRegistered(this IContainer container, Type serviceType)
		{
			return container.IsRegistered(new ServiceDefinition(serviceType));
		}
		public static Boolean IsRegistered<TService>(this IContainer container)
		{
			return container.IsRegistered(typeof(TService));
		}
	}
}
