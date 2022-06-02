# MicroDI

MicroDI is a simple and lightweight DI Container.

## How To Use

1. Instantiate a new `Container`

```cs
IContainer container = new Container();
```

2. Construct an `IServiceRegistration` and add it to the container

```cs
Type serviceType = typeof(TService);
String serviceName = "MyService";

IServiceDefinition definition = new ServiceDefinition(serviceType, serviceName);

Type implementationType = typeof(TImplementation);
Object constructorArg1 = new Object();
Object constructorArg2 = new Object();

IServiceFactoryInstructions instructions = new ServiceFactoryInstructions(serviceImplementationType, new Object[] { arg1, arg2 });

IServiceFactory factory = new TransientServiceFactory(instructions);
			
IServiceRegistration registration = new ServiceRegistration(definition, factory);
container.Add(registration);
```

3. Resolve your service

```cs
Type serviceType = typeof(TService);
String serviceName = "MyService";

IServiceDefinition definition = new ServiceDefinition(serviceType, serviceName);

Object service = container.Resolve(definition);
```

## Extension Methods
Extension Methods found in `Extensions.cs` simplify using the container. The previous workflow is reduced to the following:

1. Instantiate a new `Container`

```cs
IContainer container = new Container();
```

2. Add a service registration to the container

```cs
Object constructorArg1 = new Object();
Object constructorArg2 = new Object();
containerFactory.AddTransient<TService, TImplementation>("MyService", new Object[]{constructorArg1, constructorArg2});
```

4. Resolve your service by type or name

```cs
TService service = container.Resolve<TService>();
service = container.Resolve<TService>("MyService");
```
*Note: The provided `Container` class makes use of `ServiceDefinitionEqualityComparer.cs` for definition comparisons and resolving services based on definitions.*
