using System;
using System.Threading.Tasks;
using AzureLinkboard.Api.Model;

namespace AzureLinkboard.Domain.Services
{
    public interface ITagService
    {
        Task ProcessTags(string userId, string url, DateTimeOffset savedAt, string tagsString);
        Task<Page<Api.Model.SavedUrl>> GetTagItems(string userId, string tag, int pageSize, string continuationToken = null);
    }
}
