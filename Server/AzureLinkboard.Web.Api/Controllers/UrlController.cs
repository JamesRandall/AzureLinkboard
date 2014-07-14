using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AzureLinkboard.Api.Model;
using AzureLinkboard.Domain.Services;
using AzureLinkboard.Domain.Validation;
using AzureLinkboard.Web.Extensions;
using Microsoft.AspNet.Identity;

namespace AzureLinkboard.Web.Api.Controllers
{
    [Authorize]
    public class UrlController : ApiController
    {
        private readonly IUrlService _urlService;

        public UrlController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        public async Task<Page<SavedUrl>> Get(int pageSize = 20, string continuationToken = null)
        {
            return await _urlService.Get(User.Identity.GetUserId(), pageSize, continuationToken);
        }

        public async Task<HttpResponseMessage> Post(SaveUrlRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ValidationResult<SavedUrl> result = await _urlService.Save(User.Identity.GetUserId(), model);
            if (result.Failed)
            {
                result.AddToModelState(ModelState);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result.Item);
        }
    }
}