using JetBrains.Annotations;

namespace Autofac.AttributeExtensions
{
    [MeansImplicitUse]
    public class InstancePerLifetimeScopeAttribute : RegistrationAttribute
    {
        public InstancePerLifetimeScopeAttribute(object[] lifetimeScopeTags = null)
            : base(Lifestyle.InstancePerLifetimeScope)
        {
            LifetimeScopeTags = lifetimeScopeTags;
        }
    }
}