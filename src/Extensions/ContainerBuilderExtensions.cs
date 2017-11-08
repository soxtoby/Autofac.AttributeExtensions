using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Core.Lifetime;
using Autofac.Util;
using Registration = Autofac.Builder.IRegistrationBuilder<object, Autofac.Builder.ConcreteReflectionActivatorData, Autofac.Builder.SingleRegistrationStyle>;

namespace Autofac.AttributeExtensions
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterAttributedClasses(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            IEnumerable<Type> attributedTypes = assemblies.SelectMany(a => a.GetLoadableTypes());

            foreach (Type type in attributedTypes)
            {
                IEnumerable<RegistrationAttribute> registrationAttributes = type.GetCustomAttributes<RegistrationAttribute>(false);

                foreach (RegistrationAttribute attribute in registrationAttributes)
                {
                    Registration registration = builder.RegisterType(type);

                    ConfigureParameters(registration, type);

                    SetLifestyle(registration, attribute);

                    RegisterAs(registration, attribute, type);
                }
            }
        }

        private static void ConfigureParameters(Registration registration, Type type)
        {
            var attributedParameters = type.GetConstructors()
                .SelectMany(c => c.GetParameters())
                .Select(p => new { info = p, attribute = p.GetCustomAttribute<ParameterRegistrationAttribute>() })
                .Where(p => p.attribute != null);

            foreach (var parameter in attributedParameters)
            {
                if (parameter.attribute.Named != null)
                    registration.WithParameter((p, c) => p == parameter.info, (p, c) => c.ResolveNamed(parameter.attribute.Named, parameter.info.ParameterType));
            }
        }

        private static void SetLifestyle(Registration registration, RegistrationAttribute attribute)
        {
            switch (attribute.Lifestyle)
            {
                case Lifestyle.InstancePerDependency:
                    registration.InstancePerDependency();
                    break;
                case Lifestyle.InstancePerLifetimeScope:
                    if (attribute.LifetimeScopeTags?.Any() ?? false)
                        registration.InstancePerMatchingLifetimeScope(FixedScopeTags(attribute).ToArray());
                    else
                        registration.InstancePerLifetimeScope();
                    break;
                case Lifestyle.SingleInstance:
                    registration.SingleInstance();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerable<object> FixedScopeTags(RegistrationAttribute attribute)
        {
            // Turns RequestLifetimeScopeTag into Autofac's version
            return attribute.LifetimeScopeTags.Select(t => t == RegistrationAttribute.RequestLifetimeScopeTag ? MatchingScopeLifetimeTags.RequestLifetimeScopeTag : t);
        }

        private static void RegisterAs(Registration registration, RegistrationAttribute attribute, Type type)
        {
            if (attribute.Name != null || attribute.Key != null)
            {
                foreach (Type asType in RegisterAsTypes(attribute, type))
                {
                    RegisterNamed(registration, attribute, asType);
                    RegisterKeyed(registration, attribute, asType);
                }
                if (attribute.As == null)
                    registration.AsSelf();
            }
            else
            {
                foreach (Type asType in RegisterAsTypes(attribute, type))
                    registration.As(asType);
            }
        }

        private static IEnumerable<Type> RegisterAsTypes(RegistrationAttribute attribute, Type type)
        {
            return attribute.As
                ?? (attribute.AsImplementedInterfaces
                    ? type.GetInterfaces().Concat(new[] { type })
                    : new[] { type });
        }

        private static void RegisterNamed(Registration registration, RegistrationAttribute attribute, Type asType)
        {
            if (attribute.Name != null)
                registration.Named(attribute.Name, asType);
        }

        private static void RegisterKeyed(Registration registration, RegistrationAttribute attribute, Type asType)
        {
            if (attribute.Key != null)
                registration.Keyed(attribute.Key, asType);
        }
    }
}
