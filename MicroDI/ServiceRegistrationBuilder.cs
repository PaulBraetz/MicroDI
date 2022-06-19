using MicroDI.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDI
{
	public class ServiceRegistrationBuilder
	{
		public ServiceRegistrationBuilder() { }
		public ServiceRegistrationBuilder(Type serviceType, Type serviceImplementationType, String? name = null)
		{
			Name = name;
			ServiceType = serviceType;
			ServiceImplementationType = serviceImplementationType;
		}

		public const String TRANSIENT_SCOPE = "TRANSIENT";
		public const String SINGLETON_SCOPE = "SINGLETON";

		protected String? Name { get; private set; }
		protected Type? ServiceType { get; private set; }
		protected Type? ServiceImplementationType { get; private set; }
		protected List<Object> ConstructorArguments { get; } = new();
		protected List<IServiceRegistration> ConstructorInjectionArguments { get; } = new();
		protected Dictionary<String, Object> PropertyValues { get; } = new();
		protected Dictionary<String, IServiceRegistration> PropertyInjectionValues { get; } = new();
		protected String? Scope { get; set; }

		public ServiceRegistrationBuilder SetName(String name)
		{
			Name = name;
			return this;
		}
		public ServiceRegistrationBuilder SetServiceType(Type serviceType)
		{
			ServiceType = serviceType;
			return this;
		}
		public ServiceRegistrationBuilder SetServiceImplementationType(Type serviceImplementationType)
		{
			ServiceImplementationType = serviceImplementationType;
			return this;
		}
		public ServiceRegistrationBuilder AddConstructorArgument(Object arg)
		{
			ConstructorArguments.Add(arg);
			return this;
		}
		public ServiceRegistrationBuilder AddCOnstructorArguments(IEnumerable<Object> args)
		{
			ConstructorArguments.AddRange(args);
			return this;
		}
		public ServiceRegistrationBuilder AddConstructorInjectionArgument(IServiceRegistration arg)
		{
			ConstructorInjectionArguments.Add(arg);
			return this;
		}
		public ServiceRegistrationBuilder AddCOnstructorInjectionArguments(IEnumerable<IServiceRegistration> args)
		{
			ConstructorInjectionArguments.AddRange(args);
			return this;
		}
		public ServiceRegistrationBuilder AddPropertyValue(String name, Object value)
		{
			PropertyValues.Add(name, value);
			return this;
		}
		public ServiceRegistrationBuilder AddPropertyInjectionValue(String name, IServiceRegistration value)
		{
			PropertyInjectionValues.Add(name, value);
			return this;
		}
		public ServiceRegistrationBuilder SetScope(String scope)
		{
			Scope = scope;
			return this;
		}

		public IServiceRegistration BuildServiceRegistration()
		{
			IServiceFactory? factory = BuildFactory();
			var definition = new ServiceDefinition(ServiceType, Name);
			return new ServiceRegistration(definition, factory);
		}

		protected virtual IServiceFactory ResolveScope(IInjectionInstructions instructions)
		{
			return new TransientServiceFactory(instructions);
		}
		protected virtual IServiceFactory ResolveScope(IConstructorInjectionInstructions instructions)
		{
			return new TransientServiceFactory(instructions);
		}
		protected virtual IServiceFactory ResolveScope(IPropertynInjectionInstructions instructions)
		{
			return new TransientServiceFactory(instructions);
		}

		public IServiceFactory BuildFactory()
		{
			if (ServiceType == null) throw new InvalidOperationException($"{nameof(ServiceType)} has not been set.");
			if (ServiceImplementationType == null) throw new InvalidOperationException($"{nameof(ServiceImplementationType)} has not been set.");

			if (PropertyValues.Any() || PropertyInjectionValues.Any())
			{
				var instructions = new PropertyInjectionInstructions(ServiceImplementationType, ConstructorArguments, ConstructorInjectionArguments, PropertyValues, PropertyInjectionValues);
				return Scope switch
				{
					SINGLETON_SCOPE => new SingletonServiceFactory(instructions),
					TRANSIENT_SCOPE => new TransientServiceFactory(instructions),
					_ => ResolveScope(instructions)
				};
			}
			else if (ConstructorArguments.Any() || ConstructorInjectionArguments.Any())
			{
				var instructions = new ConstructorInjectionInstructions(ServiceImplementationType, ConstructorArguments, ConstructorInjectionArguments);
				return Scope switch
				{
					SINGLETON_SCOPE => new SingletonServiceFactory(instructions),
					TRANSIENT_SCOPE => new TransientServiceFactory(instructions),
					_ => ResolveScope(instructions)
				};
			}
			else
			{
				var instructions = new InjectionInstructions(ServiceImplementationType);
				return Scope switch
				{
					SINGLETON_SCOPE => new SingletonServiceFactory(instructions),
					TRANSIENT_SCOPE => new TransientServiceFactory(instructions),
					_ => ResolveScope(instructions)
				};
			}
		}
	}
}
