using System.IO;

namespace Engine.Utility
{
    public interface IRulesStreamsProvider
    {
        Stream[] Streams { get; }
    }
}