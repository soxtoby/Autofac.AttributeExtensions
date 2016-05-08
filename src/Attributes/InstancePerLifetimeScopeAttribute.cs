using JetBrains.Annotations;

namespace Autofac.AttributeExtensions
{
    [MeansImplicitUse]
    public class InstancePerLifetimeScopeAttribute : RegistrationAttribute
    {
        public InstancePerLifetimeScopeAttribute() 
            : base(Lifestyle.InstancePerLifetimeScope)
        {
        }

        public InstancePerLifetimeScopeAttribute(params object[] lifetimeScopeTags)
            : base(Lifestyle.InstancePerLifetimeScope)
        {
            LifetimeScopeTags = lifetimeScopeTags;
        }
    }
}