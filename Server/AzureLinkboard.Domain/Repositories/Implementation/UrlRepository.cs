using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Core.Components;
using AccidentalFish.ApplicationSupport.Core.Extensions;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using AzureLinkboard.Storage.NoSql;
using Microsoft.WindowsAzure.Storage.Table;
using SavedUrl = AzureLinkboard.Storage.NoSql.SavedUrl;

namespace AzureLinkboard.Domain.Repositories.Implementation
{
    internal class UrlRepository : IUrlRepository
    {
        private readonly INoSqlConcurrencyManager _concurrencyManager;
        private readonly IAsynchronousNoSqlRepository<SavedUrl> _table;
        private readonly IAsynchronousNoSqlRepository<DateOrderedUrl> _dateOrderIndexTable;
        private readonly IAsynchronousNoSqlRepository<UrlStatistics> _statisticsTable;
        private readonly IAsynchronousNoSqlRepository<UrlUser> _urlUserTable;

        public UrlRepository(IApplicationResourceFactory applicationResourceFactory,
            INoSqlConcurrencyManager concurrencyManager)
        {
            _concurrencyManager = concurrencyManager;
            string dateOrderedTableName = applicationResourceFactory.Setting(ComponentIdentities.UrlStore, "date-ordered-tablename");
            string statisticsTableName = applicationResourceFactory.Setting(ComponentIdentities.UrlStore, "statistics-tablename");
            string urlUsersTableName = applicationResourceFactory.Setting(ComponentIdentities.UrlStore, "url-users-tablename");

            _table = applicationResourceFactory.GetNoSqlRepository<SavedUrl>(ComponentIdentities.UrlStore);
            _dateOrderIndexTable = applicationResourceFactory.GetNoSqlRepository<DateOrderedUrl>(dateOrderedTableName, ComponentIdentities.UrlStore);
            _statisticsTable = applicationResourceFactory.GetNoSqlRepository<UrlStatistics>(statisticsTableName, ComponentIdentities.UrlStore);
            _urlUserTable = applicationResourceFactory.GetNoSqlRepository<UrlUser>(urlUsersTableName, ComponentIdentities.UrlStore);
        }

        public IAsynchronousNoSqlRepository<SavedUrl> Table { get { return _table; } }

        public IAsynchronousNoSqlRepository<DateOrderedUrl> DateOrderIndexTable { get { return _dateOrderIndexTable; } }

        public Task Save(SavedUrl url)
        {
            return _table.InsertAsync(url);
        }

        public Task Save(DateOrderedUrl url)
        {
            return _dateOrderIndexTable.InsertAsync(url);
        }

        public Task Save(UrlStatistics statistics)
        {
            return _statisticsTable.InsertAsync(statistics);
        }

        public Task Save(UrlUser urlUser)
        {
            return _urlUserTable.InsertOrReplaceAsync(urlUser);
        }

        public async Task<PagedResultSegment<DateOrderedUrl>> GetDateOrderIndexesForUser(string userId, int pageSize, string continuationToken)
        {
            PagedResultSegment<DateOrderedUrl> segment = await _dateOrderIndexTable.PagedQueryAsync(new Dictionary<string, object> { { "PartitionKey", userId } }, pageSize, continuationToken);
            if (!segment.Page.Any())
            {
                return null;
            }
            return segment;
        }

        public async Task<PagedResultSegment<SavedUrl>> GetByUrls(string userId, IEnumerable<string> urls)
        {
            string partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId);
            List<string> rowFilters = urls.Select(url => TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal,url.Base64Encode())).ToList();

            string rowFilter = rowFilters[0];
            for (int index = 1; index < rowFilters.Count; index++)
            {
                string subQueryString = rowFilters[index];
                rowFilter = TableQuery.CombineFilters(rowFilter, TableOperators.Or, subQueryString);
            }

            string filter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, rowFilter);

            return await _table.PagedQueryAsync(filter, rowFilters.Count, null);
        }

        public async Task<int> IncrementNumberOfSaves(string url)
        {
            UrlStatistics statistics = await _concurrencyManager.Update(_statisticsTable, url.Base64Encode(), "", s =>
            {
                s.NumberOfSaves++;
            });
            if (statistics == null)
            {
                return -1;
            }
            return statistics.NumberOfSaves;
        }
    }
}
