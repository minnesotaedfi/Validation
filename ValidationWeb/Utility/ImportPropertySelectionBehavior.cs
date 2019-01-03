namespace ValidationWeb.Utility
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using SimpleInjector.Advanced;

    public class ImportPropertySelectionBehavior : IPropertySelectionBehavior 
    {
        public bool SelectProperty(Type type, PropertyInfo prop) 
        {
            return prop.GetCustomAttributes(typeof(ImportAttribute)).Any();
        }
    }
}