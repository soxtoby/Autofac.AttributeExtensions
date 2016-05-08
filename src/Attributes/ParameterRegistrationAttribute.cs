using System;

namespace Autofac.AttributeExtensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class ParameterRegistrationAttribute : Attribute
    {
        public string Named { get; set; }
    }
}