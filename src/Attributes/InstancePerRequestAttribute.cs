using JetBrains.Annotations;

namespace Autofac.AttributeExtensions
{
    [MeansImplicitUse]
    public class InstancePerRequestAttribute : InstancePerLifetimeScopeAttribute
    {
        public InstancePerRequestAttribute() : base(RequestLifetimeScopeTag) { }
    }
}