using System.IO;
using System.Text;

namespace Engine.Utility
{
    public static class StringExt
    {
        public static Stream ToStream(this string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str ?? ""));
        }
    }

    public static class StreamExt
    {
        public static string ReadToString(this Stream stream)
        {
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
