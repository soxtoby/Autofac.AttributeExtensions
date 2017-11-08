using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac.AttributeExtensions;
using Autofac.Core;
using Autofac.Core.Lifetime;
using EasyAssertions;
using Xunit;

namespace Autofac.Attributes.Tests
{
    public class Tests : IDisposable
    {
        private readonly IContainer _sut;

        public Tests()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterAttributedClasses(GetType().Assembly);
            _sut = builder.Build();
        }

        public void Dispose()
        {
            _sut.Dispose();
        }

        [Fact]
        public void InstancePerDependency()
        {
            InstancePerDependency result1 = _sut.Resolve<InstancePerDependency>();
            InstancePerDependency result2 = _sut.Resolve<InstancePerDependency>();

            result1.ShouldNotBe(result2);
        }

        [Fact]
        public void SingleInstance()
        {
            SingleInstance result1 = _sut.Resolve<SingleInstance>();
            SingleInstance result2 = _sut.Resolve<SingleInstance>();

            SingleInstance result3;
            using (ILifetimeScope innerScope = _sut.BeginLifetimeScope())
                result3 = innerScope.Resolve<SingleInstance>();

            result1.ShouldBe(result2)
                .And.ShouldBe(result3);
        }

        [Fact]
        public void DerivedClass_BaseClassHasAttribute_IsNotRegistered()
        {
            _sut.IsRegistered<DerivedClassWithoutAttribute>().ShouldBe(false);
        }

        [Fact]
        public void DerivedClassWithAttribute_BaseClassHasAttribute_IsRegistered()
        {
            _sut.IsRegistered<DerivedClassWithAttribute>().ShouldBe(true);
            _sut.Resolve<IEnumerable<IInterface>>()
                .Select(t => t.GetType()).ShouldMatch(typeof(DerivedClassWithAttribute), typeof(ImplementsInterface));
        }

        [Fact]
        public void InstancePerLifetimeScope()
        {
            InstancePerLifetimeScope result1 = _sut.Resolve<InstancePerLifetimeScope>();
            InstancePerLifetimeScope result2 = _sut.Resolve<InstancePerLifetimeScope>();

            InstancePerLifetimeScope innerResult;
            using (ILifetimeScope innerScope = _sut.BeginLifetimeScope())
                innerResult = innerScope.Resolve<InstancePerLifetimeScope>();

            result1.ShouldBe(result2)
                .And.ShouldNotBe(innerResult);
        }

        [Fact]
        public void InstancePerMatchingLifetimeScope()
        {
            Should.Throw<DependencyResolutionException>(() => _sut.Resolve<InstancePerMatchingLifetimeScope>());

            using (ILifetimeScope wrongTagScope = _sut.BeginLifetimeScope("some other tag"))
                Should.Throw<DependencyResolutionException>(() => wrongTagScope.Resolve<InstancePerMatchingLifetimeScope>());

            using (ILifetimeScope firstTagScope = _sut.BeginLifetimeScope("tag one"))
                firstTagScope.Resolve<InstancePerMatchingLifetimeScope>();

            using (ILifetimeScope secondTagScope = _sut.BeginLifetimeScope("tag two"))
                secondTagScope.Resolve<InstancePerMatchingLifetimeScope>();
        }

        [Fact]
        public void InstancePerRequest()
        {
            Should.Throw<DependencyResolutionException>(() => _sut.Resolve<InstancePerRequest>());

            using (ILifetimeScope requestScope = _sut.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag))
                requestScope.Resolve<InstancePerRequest>();
        }

        [Fact]
        public void RegisteredAsSelfAndImplementedInterfacesByDefault()
        {
            _sut.IsRegistered<BaseClass>().ShouldBe(false);
            _sut.Resolve<IInterface>()
                .ShouldBe(_sut.Resolve<ImplementsInterface>());
        }

        [Fact]
        public void RegisteredAsSelfOnly()
        {
            _sut.IsRegistered<IUnregistered>().ShouldBe(false);
            _sut.IsRegistered<RegisteredAsSelfOnly>().ShouldBe(true);
        }

        [Fact]
        public void RegisteredAsSpecifiedTypes()
        {
            _sut.IsRegistered<IUnspecifiedInterface>().ShouldBe(false);
            _sut.IsRegistered<RegisteredAsSpecifiedTypes>().ShouldBe(false);

            ISpecifiedInterface result1 = _sut.Resolve<ISpecifiedInterface>();
            result1.ShouldBeA<RegisteredAsSpecifiedTypes>();

            SpecifiedClass result2 = _sut.Resolve<SpecifiedClass>();
            result2.ShouldBeA<RegisteredAsSpecifiedTypes>();

            ((RegisteredAsSpecifiedTypes)result1).ShouldBe((RegisteredAsSpecifiedTypes)result2);
        }

        [Fact]
        public void NamedRegistration()
        {
            _sut.IsRegistered<INamedRegistration>().ShouldBe(false);
            _sut.IsRegistered<NamedRegistration>().ShouldBe(true);
            _sut.IsRegisteredWithName<NamedRegistration>("name").ShouldBe(true);
        }

        [Fact]
        public void KeyedRegistration()
        {
            _sut.IsRegistered<IKeyedRegistration>().ShouldBe(false);
            _sut.IsRegistered<KeyedRegistration>().ShouldBe(true);
            _sut.IsRegisteredWithKey<KeyedRegistration>("key").ShouldBe(true);
        }

        [Fact]
        public void MultipleRegistrations()
        {
            MultipleRegistrations single1 = _sut.ResolveNamed<MultipleRegistrations>("single");
            MultipleRegistrations single2 = _sut.ResolveNamed<MultipleRegistrations>("single");

            MultipleRegistrations transient1 = _sut.ResolveNamed<MultipleRegistrations>("transient");
            MultipleRegistrations transient2 = _sut.ResolveNamed<MultipleRegistrations>("transient");

            single1.ShouldBe(single2)
                .And.ShouldNotBe(transient1)
                .And.ShouldNotBe(transient2);
            transient1.ShouldNotBe(transient2);
        }

        [Fact]
        public void NamedParameter()
        {
            _sut.Resolve<NamedParameter>()
                .Dependency.ShouldBeA<NamedDependency>();
        }
    }

    [InstancePerDependency]
    class InstancePerDependency { }

    [SingleInstance]
    class SingleInstance { }

    [InstancePerLifetimeScope]
    class InstancePerLifetimeScope { }

    [InstancePerLifetimeScope("tag one", "tag two")]
    class InstancePerMatchingLifetimeScope { }

    [InstancePerRequest]
    class InstancePerRequest { }

    interface IInterface { }

    class BaseClass { }

    [SingleInstance]
    class ImplementsInterface : BaseClass, IInterface { }
    class DerivedClassWithoutAttribute : ImplementsInterface { }
    [SingleInstance]
    class DerivedClassWithAttribute : ImplementsInterface { }

    interface IUnregistered { }

    [InstancePerDependency(AsImplementedInterfaces = false)]
    class RegisteredAsSelfOnly : IUnregistered { }

    interface IUnspecifiedInterface { }

    interface ISpecifiedInterface { }

    class SpecifiedClass { }

    [SingleInstance(As = new[] { typeof(ISpecifiedInterface), typeof(SpecifiedClass) })]
    class RegisteredAsSpecifiedTypes : SpecifiedClass, IUnspecifiedInterface, ISpecifiedInterface { }

    interface INamedRegistration { }

    [InstancePerDependency(Name = "name")]
    class NamedRegistration : INamedRegistration { }

    interface IKeyedRegistration { }

    [InstancePerDependency(Key = "key")]
    class KeyedRegistration : IKeyedRegistration { }

    [SingleInstance(Name = "single")]
    [InstancePerDependency(Name = "transient")]
    class MultipleRegistrations { }

    interface IDependency { }

    [InstancePerDependency(Name = "name")]
    class NamedDependency : IDependency { }

    [InstancePerDependency]
    class UnnamedDependency : IDependency { }

    [InstancePerDependency]
    class NamedParameter
    {
        public IDependency Dependency { get; set; }

        public NamedParameter([Named("name")]IDependency dependency)
        {
            Dependency = dependency;
        }
    }
}
