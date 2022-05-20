using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MicroDI
{
	public sealed class ContainerFactory : IContainerFactory
	{
		private interface IServiceFactory
		{
			Object Build();
		}

		private readonly struct SingletonServiceFactory : IServiceFactory
		{
			public SingletonServiceFactory(IServiceDefinition definition)
			{
				instance = new TransientServiceFactory(definition).Build();
			}

			private readonly Object instance;

			public Object Build()
			{
				return instance;
			}
		}

		private readonly struct TransientServiceFactory : IServiceFactory
		{
			public TransientServiceFactory(IServiceDefinition definition)
			{
				var ctorInfo = definition.ServiceImplementation.GetConstructors()
					.Single(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(definition.Arguments.Select(p=>p.GetType())));

				if(ctorInfo == null)
				{
					throw new ArgumentException($"{definition.ServiceImplementation.Name} does not provide a constructor accepting the arguments {String.Join(", ", definition.Arguments.Select(p => p.GetType().Name))}");
				}

				var ctorArgs = definition.Arguments.Select(Expression.Constant);

				var ctorExpr = Expression.New(ctorInfo, ctorArgs);

				var ctor = Expression.Lambda(ctorExpr);

				factory = (Func<Object>)ctor.Compile();
			}

			private readonly Func<Object> factory;

			public Object Build()
			{
				return factory.Invoke();
			}
		}

		private readonly struct Container : IContainer
		{
			public Container(IEnumerable<IServiceDefinition> definitions)
			{
				this.registrations = new ConcurrentDictionary<Type, IServiceFactory>(definitions
					.Select(d => new KeyValuePair<Type, IServiceFactory>(d.ServiceType, (d.Scope) switch
					{
						ServiceDefinitions.ServiceScope.Transient => new TransientServiceFactory(d),
						ServiceDefinitions.ServiceScope.Singleton => new SingletonServiceFactory(d),
						_=>throw new ArgumentException($"Unable to create service factory for scope {d.Scope}.")
					})));
			}

			public Object Resolve(Type serviceType)
			{
				if (!IsRegistered(serviceType))
				{
					throw new ArgumentException($"No service for type {serviceType.Name} has been registered.");
				}
				return registrations[serviceType].Build();
			}

			public Boolean IsRegistered(Type serviceType)
			{
				return registrations.ContainsKey(serviceType);
			}

			private readonly ConcurrentDictionary<Type, IServiceFactory> registrations;
		}

		private ConcurrentBag<IServiceDefinition> definitions = new ConcurrentBag<IServiceDefinition>();

		public IContainer Build()
		{
			return new Container(this);
		}

		public void Add(IServiceDefinition serviceDefinition)
		{
			definitions.Add(serviceDefinition);
		}

		public IEnumerator<IServiceDefinition> GetEnumerator()
		{
			return definitions.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return definitions.GetEnumerator();
		}
	}
}
