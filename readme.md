# Autofac.AttributeExtensions
[![Build status](https://ci.appveyor.com/api/projects/status/xkws6h45ise2vv7c/branch/master?svg=true)](https://ci.appveyor.com/project/soxtoby/autofac-attributeextensions/branch/master)

Most Autofac registrations tend to look a lot like this:

```c#
builder.RegisterType<MyService>()
    .AsSelf().AsImplementedInterfaces()
    .InstancePerRequest();
```

This hides all the _interesting_ configuration behind a tonne of boilerplate code. Additionally, instance lifetimes tend to depend more on the implementation of the class, rather than a particular configuration.

By configuring registrations with attributes on the classes themselves, you can eliminate the boilerplate, and the lifetime of the class becomes much clearer.

## Getting Started
AttributeExtensions is split into two packages.
[Autofac.AttributeExtensions](https://www.nuget.org/packages/Autofac.AttributeExtensions) is the main package, and contains the extensions used to configure Autofac. [Autofac.AttributeExtensions.Attributes](https://www.nuget.org/packages/Autofac.AttributeExtensions.Attributes) contains the attributes used to specify the configuration for a class, and has no dependency on Autofac.

Install the `Autofac.AttributeExtensions` package to the project where you configure Autofac.

Before any manual configuration, add

```c#
builder.RegisterAttributedClasses(assemblies);
```

...passing in an array of assemblies to search through for attributed classes.

To configure the registration for a class, add the `Autofac.AttributeExtensions.Attributes` package to its project, then add one of the available attributes to the class:

```c#
[InstancePerDependency]
public class MyClass { }
```

## Configuration Options
### Lifetime
There are four attributes for specifying lifetime, which correspond to the registration extensions provided by Autofac: `InstancePerDependency`, `SingleInstance`, `InstancePerLifetimeScope`, and `InstancePerRequest`.

LifetimeScope tags can be specified for `InstancePerLifetimeScope` by passing them into the attribute constructor:
```c#
[InstancePerLifetimeScope("tag1", "tag2")]
public class MyClass { }
```

### Register As
By default, classes are registered as themselves and all implemented interfaces.

You can disable registering as all implemented interfaces by setting `foo` to false:

```c#
[InstancePerDependency(AsImplementedInterfaces = false)]
public class MyClass { }
```

Alternatively, you can specify the exact types the class is registered as:
```c#
[InstancePerDependency(As = new[] { typeof(IRegisteredInterface) })]
public class MyClass : IRegisteredInterface, IOtherInterface { }
```

### Named and Keyed registrations
All attributes allow setting a Name or Key to register the class as:

```c#
[InstancePerDependency(Name = "MyName")]
public class MyNamedClass { }

[InstancePerDependency(Key = "MyKey")]
public class MyKeyedClass { }
```

The class will be registered as named or keyed versions of all the types it would normally be registered as, and unless the types are specified explicitly, it will also be registered as itself without a name or key.

### Multiple registrations
A class can have multiple registrations. Just add more than one registration attribute:

```c#
[InstancePerDependency]
[InstancePerLifetimeScope("SharingScope")]
public class MyClass { }
```

### Named parameters
Classes can specify a named dependency by adding a `Name` attribute to the appropriate constructor dependency:

```c#
public class MyClass
{
    public MyClass([Named("MyName")] IDependency dependency)
    {
        // constructor logic...
    }
}
```