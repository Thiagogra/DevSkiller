using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ThirdPartyApi.Api;
using ThirdPartyApi.Model;

namespace BackendAlone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PathsController : ControllerBase
    {
        private readonly IThirdPartyApi _thirdPartyApi;
        public PathsController(IThirdPartyApi thirdPartyApi)
        {
            _thirdPartyApi = thirdPartyApi;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromForm]IList<Path> paths)
        {
            foreach (var path in paths)
            {
                _thirdPartyApi.AddPath(path);
            }
        }
    }
}
