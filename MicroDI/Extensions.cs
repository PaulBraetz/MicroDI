using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MicroDI.IServiceDefinition;

namespace MicroDI
{
	public static class Extensions
	{
		public static IContainerFactory Add<TService, TImplementation>(this IContainerFactory factory, Scope serviceScope, params Object[] constructorArguments)
		{
			factory.Add(new ServiceDefinition(typeof(TService), typeof(TImplementation), serviceScope, constructorArguments));
			return factory;
		}
		public static IContainerFactory Add<TService>(this IContainerFactory factory, Scope serviceScope, params Object[] constructorArguments)
		{
			factory.Add<TService, TService>(serviceScope, constructorArguments);
			return factory;
		}

		public static IContainerFactory AddTransient<TService, TImplementation>(this IContainerFactory factory, params Object[] constructorArguments)
		{
			factory.Add<TService, TImplementation>(Scope.Transient, constructorArguments);
			return factory;
		}
		public static IContainerFactory AddTransient<TService>(this IContainerFactory factory, params Object[] arguments)
		{
			factory.Add<TService>(Scope.Transient, arguments);
			return factory;
		}

		public static IContainerFactory AddSingleton<TService, TImplementation>(this IContainerFactory factory, params Object[] constructorArguments)
		{
			factory.Add<TService, TImplementation>(Scope.Singleton, constructorArguments);
			return factory;
		}
		public static IContainerFactory AddSingleton<TService>(this IContainerFactory factory, params Object[] arguments)
		{
			factory.Add<TService>(Scope.Singleton, arguments);
			return factory;
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
