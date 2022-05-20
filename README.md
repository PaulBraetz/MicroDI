# MicroDI

MicroDI is a simple and lightweight DI Container.

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
IServiceDefinition serviceDefinition = new ServiceDefinition(serviceType, implementationType, ServiceScope.Transient, constructorArg1, constructorArg2);
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
Extension Methods found in `Extensions.cs` make using the container and its factory simple. The previous workflow is reduced to the following:

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
