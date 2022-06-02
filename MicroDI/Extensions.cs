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
		public static IContainer AddTransient(this IContainer container, Type serviceType, String? serviceName, Type serviceImplementationType, params Object[] constructorArguments)
		{
			var instructions = new ServiceFactoryInstructions(serviceImplementationType, constructorArguments);
			var factory = new TransientServiceFactory(instructions);

			var definition = new ServiceDefinition(serviceType, serviceName);

			var registration = new ServiceRegistration(definition, factory);

			container.Add(registration);

			return container;
		}
		public static IContainer AddTransient<TService, TServiceImplementation>(this IContainer container, String serviceName, IEnumerable<Object> constructorArguments)
		{
			return container.AddTransient(typeof(TService), serviceName, typeof(TServiceImplementation), constructorArguments.ToArray());
		}
		public static IContainer AddTransient<TService, TServiceImplementation>(this IContainer container, params Object[] constructorArguments)
		{
			return container.AddTransient(typeof(TService), null, typeof(TServiceImplementation), constructorArguments);
		}

		public static IContainer AddTransient(this IContainer container, Type serviceType, Type serviceImplementationType, params Object[] constructorArguments)
		{
			return container.AddTransient(serviceType, null, serviceImplementationType, constructorArguments);
		}

		public static IContainer AddSingleton(this IContainer container, Type serviceType, String? serviceName, Type serviceImplementationType, params Object[] constructorArguments)
		{
			var instructions = new ServiceFactoryInstructions(serviceImplementationType, constructorArguments);
			var factory = new SingletonServiceFactory(instructions);

			var definition = new ServiceDefinition(serviceType, serviceName);

			var registration = new ServiceRegistration(definition, factory);

			container.Add(registration);

			return container;
		}
		public static IContainer AddSingleton<TService, TServiceImplementation>(this IContainer container, String serviceName, IEnumerable<Object> constructorArguments)
		{
			return container.AddSingleton(typeof(TService), serviceName, typeof(TServiceImplementation), constructorArguments);
		}
		public static IContainer AddSingleton<TService, TServiceImplementation>(this IContainer container, params Object[] constructorArguments)
		{
			return container.AddSingleton(typeof(TService), null, typeof(TServiceImplementation), constructorArguments);
		}

		public static IContainer AddSingleton(this IContainer container, Type serviceType, Type serviceImplementationType, params Object[] constructorArguments)
		{
			return container.AddSingleton(serviceType, null, serviceImplementationType, constructorArguments);
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
