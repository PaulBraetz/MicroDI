# MicroDI

MicroDI is a simple and lightweight DI Container.

## Versioning

This library uses semantic versioning, i.e.:
A change from a.b.c to a.b.X represents a non breaking change that does not add new interfaces.
A change from a.b.c to a.X.0 represents a non breaking change that adds new interfaces.
A change from a.b.c to X.0.0 represents a breaking change.

## How To Use

1. Instantiate a new `ContainerFactory`

```cs
IContainerFactory containerFactory = new ContainerFactory();
```

2. Add an `IServiceDefinition` to the container

```cs
Type serviceType = typeof(TService);
Type implementationType = typeof(TImplementation);
Object constructorArg1 = new Object();
Object constructorArg2 = new Object();
IServiceDefinition serviceDefinition = new ServiceDefinition(serviceType, implementationType, Scope.Transient, constructorArg1, constructorArg2);
containerFactory.Add(serviceDefinition);
```

3. Build an `IContainer`

```cs
IContainer container = containerFactory.Build();
```

4. Resolve your service

```cs
TService service = (TService)container.Resolve(typeof(TService));
```

## Extension Methods
Extension Methods found in `Extensions.cs` simplify using the container and its factory. The previous workflow is reduced to the following:

1. Instantiate a new `ContainerFactory`

```cs
IContainerFactory containerFactory = new ContainerFactory();
```

2. Add a service definition to the container

```cs
Object constructorArg1 = new Object();
Object constructorArg2 = new Object();
containerFactory.AddTransient<TService, TImplementation>(constructorArg1, constructorArg2);
```

3. Build an `IContainer`

```cs
IContainer container = containerFactory.Build();
```

4. Resolve your service

```cs
TService service = container.Resolve<TService>();
```
