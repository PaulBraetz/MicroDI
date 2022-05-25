using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MicroDI.IServiceDefinition;

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
				IServiceFactory factory = new TransientServiceFactory(definition);
				instance = new Lazy<Object>(factory.Build);
			}

			private readonly Lazy<Object> instance;

			public Object Build()
			{
				return instance.Value;
			}
		}

		private readonly struct TransientServiceFactory : IServiceFactory
		{
			public TransientServiceFactory(IServiceDefinition definition)
			{
				var ctorInfo = definition.ServiceImplementation.GetConstructors()
					.SingleOrDefault(isMatch);

				Boolean isMatch(ConstructorInfo constructorInfo)
				{
					ParameterInfo[] ctorParameters = constructorInfo.GetParameters();
					
					Type[] ctorParametersTypes = ctorParameters
						.Select(p => p.GetType())
						.ToArray();
					
					Type[] ctorArgsTypes = definition.ConstructorArguments
						.Select(p => p.GetType())
						.ToArray();

					if (ctorParametersTypes.Length < ctorArgsTypes.Length)
					{
						return false;
					}

					for (int i = 0; i < ctorParametersTypes.Length; i++)
					{
						if (i < ctorArgsTypes.Length)
						{
							if (!ctorArgsTypes[i].IsAssignableTo(ctorParametersTypes[i]))
							{
								return false;
							}
						}
						if (!(ctorParameters[i].Attributes.HasFlag(ParameterAttributes.Optional) ||
							ctorParameters[i].Attributes.HasFlag(ParameterAttributes.HasDefault)))
						{
							return false;
						}
					}
					return true;
				}

				if (ctorInfo == null)
				{
					throw new ArgumentException($"{definition.ServiceImplementation.Name} does not provide a constructor accepting the arguments {String.Join(", ", definition.ConstructorArguments.Select(p => p.GetType().Name))}");
				}

				var ctorArgs = definition.ConstructorArguments.Select(Expression.Constant);

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
				this.registrations = new Dictionary<Type, IServiceFactory>(definitions
					.Select(d => new KeyValuePair<Type, IServiceFactory>(d.ServiceType, (d.ServiceScope) switch
					{
						Scope.Transient => new TransientServiceFactory(d),
						Scope.Singleton => new SingletonServiceFactory(d),
						_ => throw new ArgumentException($"Unable to create service factory for scope {d.ServiceScope}.")
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

			private readonly Dictionary<Type, IServiceFactory> registrations;
		}

		private List<IServiceDefinition> definitions = new List<IServiceDefinition>();

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
