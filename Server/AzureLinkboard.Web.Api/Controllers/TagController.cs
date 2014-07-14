using System.Threading.Tasks;
using System.Web.Http;
using AzureLinkboard.Api.Model;
using AzureLinkboard.Domain.Services;
using Microsoft.AspNet.Identity;

namespace AzureLinkboard.Web.Api.Controllers
{
    [Authorize]
    public class TagController : ApiController
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        public async Task<Page<SavedUrl>> Get(string id, int pageSize = 20, string continuationToken = null)
        {
            return await _tagService.GetTagItems(User.Identity.GetUserId(), id, pageSize, continuationToken);
        }
    }
}