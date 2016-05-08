namespace Autofac.AttributeExtensions
{
    public class NamedAttribute : ParameterRegistrationAttribute
    {
        public NamedAttribute(string name)
        {
            Named = name;
        }
    }
}