using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDI
{
	public static class Extensions
	{
		public static void Add<TService, TImplementation>(this IContainerFactory factory, ServiceDefinitions.ServiceScope scope, params Object[] arguments)
		{
			factory.Add(new ServiceDefinition(typeof(TService), typeof(TImplementation), scope, arguments));
		}
		public static void Add<TService>(this IContainerFactory factory, ServiceDefinitions.ServiceScope scope, params Object[] arguments)
		{
			factory.Add<TService, TService>(scope, arguments);
		}

		public static void AddTransient<TService, TImplementation>(this IContainerFactory factory, params Object[] arguments)
		{
			factory.Add<TService, TImplementation>(ServiceDefinitions.ServiceScope.Transient, arguments);
		}
		public static void AddTransient<TService>(this IContainerFactory factory, params Object[] arguments)
		{
			factory.Add<TService>(ServiceDefinitions.ServiceScope.Transient, arguments);
		}

		public static void AddSingleton<TService, TImplementation>(this IContainerFactory factory, params Object[] arguments)
		{
			factory.Add<TService, TImplementation>(ServiceDefinitions.ServiceScope.Singleton, arguments);
		}
		public static void AddSingleton<TService>(this IContainerFactory factory, params Object[] arguments)
		{
			factory.Add<TService>(ServiceDefinitions.ServiceScope.Singleton, arguments);
		}

		public static TService Resolve<TService>(this IContainer container)
		{
			return (TService)container.Resolve(typeof(TService));
		}
		public static Boolean IsRegistered<TService>(this IContainer container)
		{
			return container.IsRegistered(typeof(TService));
		}
	}
}
