using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Threading;

namespace ValidationWeb
{
    /// <summary>
    /// This formatter uses Newtonsoft JSON.Net and handles Camel Casing, returns Enums as Strings, and converts the date into the ISO format JavaScript loves.
    /// </summary>
    public class JsonNetFormatter : MediaTypeFormatter
    {
        public JsonNetFormatter()
        {
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        }

        public override bool CanWriteType(Type type) => true;

        public override bool CanReadType(Type type) => true;

        public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var token = new CancellationToken();
            return ReadFromStreamAsync(type, stream, content, formatterLogger, token);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger, CancellationToken token)
        {
            var task = Task<object>.Factory.StartNew(() =>
            {
                var sr = new StreamReader(stream);
                var jreader = new JsonTextReader(sr);

                var ser = new JsonSerializer();
                ser.Converters.Add(new IsoDateTimeConverter());

                object val = ser.Deserialize(jreader, type);
                return val;
            });

            return task;
        }

        public override System.Threading.Tasks.Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            var token = new CancellationToken();
            return WriteToStreamAsync(type, value, stream, content, transportContext, token);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext, CancellationToken token)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = new List<JsonConverter> { new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() }, new IsoDateTimeConverter() },
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.None,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                };

                string json = JsonConvert.SerializeObject(value, Formatting.Indented, settings);

                byte[] buf = System.Text.Encoding.Default.GetBytes(json);
                stream.Write(buf, 0, buf.Length);
                stream.Flush();
            });

            return task;
        }
    }
}