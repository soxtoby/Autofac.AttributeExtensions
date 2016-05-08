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
        public object[] LifetimeScopeTags { get; protected set; }
        public string Name { get; set; }
        public object Key { get; set; }
        public bool AsImplementedInterfaces { get; set; } = true;
        public Type[] As { get; set; }

        /// <summary>
        /// Used internally to avoid a dependency on Autofac.
        /// </summary>
        public static readonly object RequestLifetimeScopeTag = new object();
    }
}
