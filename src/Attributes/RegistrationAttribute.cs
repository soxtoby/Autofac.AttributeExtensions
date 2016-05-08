using System;

namespace Autofac.AttributeExtensions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public abstract class RegistrationAttribute : Attribute
    {
        protected RegistrationAttribute(Lifestyle lifestyle)
        {
            Lifestyle = lifestyle;
        }

        public Lifestyle Lifestyle { get; }
        public object[] LifetimeScopeTags { get; set; }
        public string Name { get; set; }
        public object Key { get; set; }
        public bool AsImplementedInterfaces { get; set; } = true;
        public Type[] As { get; set; }

        public static readonly object RequestLifetimeScopeTag = new object();
    }
}
