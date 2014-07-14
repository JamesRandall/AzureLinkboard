using System.Threading.Tasks;
using AzureLinkboard.Api.Model;
using AzureLinkboard.Domain.Validation;

namespace AzureLinkboard.Domain.Services
{
    public interface IUrlService
    {
        Task<ValidationResult<SavedUrl>> Save(string userId, SaveUrlRequest model);

        Task<Page<SavedUrl>> Get(string userId, int pageSize, string continuationToken = null);
    }
}
