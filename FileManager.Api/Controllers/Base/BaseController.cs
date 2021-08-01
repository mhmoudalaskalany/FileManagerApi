using Microsoft.AspNetCore.Mvc;

namespace FileManager.Api.Controllers.Base
{
    /// <inheritdoc />
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {

        /// <inheritdoc />
        public BaseController()
        {

        }

    }
}