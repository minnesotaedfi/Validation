using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Engine.Utility;
using Newtonsoft.Json;

namespace Runner.Controllers
{
    [RoutePrefix("api/validate")]
    public class ValidateController : ApiController
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private class Model
        {
            [JsonProperty("persist")]
            public bool Persist { get; set; }
            [JsonProperty("connection")]
            public string Connection { get; set; }
            [JsonProperty("collection")]
            public string CollectionId { get; set; }
        }

        private readonly RuleRunner _runner;

        public ValidateController(RuleRunner runner)
        {
            _runner = runner;
        }

        [Route("")]
        public async Task Post()
        {
            var obj = await Request.Content.ReadAsAsync<Model>();
            if (obj.Persist)
            {
                await Task.Factory.StartNew(async () => { await _runner.Run(obj.Connection, obj.CollectionId); });
            }
            else
            {
                await Task.Factory.StartNew(async () => { await _runner.Test(obj.Connection, obj.CollectionId); });
            }
        }
    }
}
