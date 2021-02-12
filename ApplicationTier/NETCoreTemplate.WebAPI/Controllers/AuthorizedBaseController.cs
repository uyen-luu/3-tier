using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCoreTemplate.WebAPI.Extensions;

namespace NETCoreTemplate.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class AuthorizedBaseController: ControllerBase
    {
        protected readonly IHttpContextAccessor HttpContextAccessor;
        public AuthorizedBaseController(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        protected string UserEmail => HttpContextAccessor.ExtractUserEmail();
    }
}
