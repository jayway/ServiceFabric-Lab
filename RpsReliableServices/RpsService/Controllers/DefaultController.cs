using System;
using System.Web.Http;
using Microsoft.ServiceFabric.Data;

namespace RpsService.Controllers
{
    [RoutePrefix("")]
    public class DefaultController : ApiController
    {
        private readonly IReliableStateManager _stateManager;

        public DefaultController(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(DateTime.UtcNow);
        }
    }
}
