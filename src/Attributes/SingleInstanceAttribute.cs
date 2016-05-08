using JetBrains.Annotations;

namespace Autofac.AttributeExtensions
{
    [MeansImplicitUse]
    public class SingleInstanceAttribute : RegistrationAttribute
    {
        public SingleInstanceAttribute() : base(Lifestyle.SingleInstance) { }
    }
}