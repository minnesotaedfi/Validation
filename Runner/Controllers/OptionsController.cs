using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using Engine.Models;
// ReSharper disable InconsistentNaming

namespace Runner.Controllers
{
    public class Options
    {
        public IEnumerable<string> collections { get; set; }
        public IEnumerable<string> connections { get; set; }
    }

    [RoutePrefix("api/options")]
    public class OptionsController : ApiController
    {
        private readonly Model _model;

        public OptionsController(Model model)
        {
            _model = model;
        }

        [Route("")]
        public Options Get()
        {
            return new Options
            {
                collections = _model.Collections.Select(x => x.CollectionId),
                connections = from ConnectionStringSettings css
                              in ConfigurationManager.ConnectionStrings
                              select css.Name
            };
        }
    }
}
