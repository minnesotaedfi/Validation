using System.Collections.Generic;

namespace Engine.Utility
{
    public interface IConstantValueProvider
    {
        IDictionary<string, string> Values { get; }
    }
}