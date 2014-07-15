using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using AzureLinkboard.Storage.NoSql;

namespace AzureLinkboard.Domain.Repositories
{
    internal interface IUserTagRepository
    {
        Task Save(UserTag userTag);
        Task Save(DateOrderedUserTagItem dateOrderedUserTagItem);
        Task Save(UniqueUserTagItem uniqueUserTagItem);
        Task<PagedResultSegment<DateOrderedUserTagItem>> GetForUserAndTag(string userId, string tag, int pageSize, string continuationToken);
    }
}
