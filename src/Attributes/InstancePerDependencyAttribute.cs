using JetBrains.Annotations;

namespace Autofac.AttributeExtensions
{
    [MeansImplicitUse]
    public class InstancePerDependencyAttribute : RegistrationAttribute
    {
        public InstancePerDependencyAttribute() : base(Lifestyle.InstancePerDependency) { }
    }
}