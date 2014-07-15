using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using AzureLinkboard.Storage.NoSql;

namespace AzureLinkboard.Domain.Repositories
{
    interface IUrlRepository
    {
        IAsynchronousNoSqlRepository<SavedUrl> Table { get; }
        IAsynchronousNoSqlRepository<DateOrderedUrl> DateOrderIndexTable { get; }
        Task Save(SavedUrl url);
        Task Save(DateOrderedUrl url);
        Task<PagedResultSegment<DateOrderedUrl>> GetDateOrderIndexesForUser(string userId, int pageSize, string continuationToken);
        Task<PagedResultSegment<SavedUrl>> GetByUrls(string userId, IEnumerable<string> urls);
    }
}
