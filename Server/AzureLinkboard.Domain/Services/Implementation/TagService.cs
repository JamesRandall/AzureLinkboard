using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Core.Components;
using AccidentalFish.ApplicationSupport.Core.Extensions;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using AzureLinkboard.Api.Model;
using AzureLinkboard.Domain.Helpers;
using AzureLinkboard.Domain.Mappers;
using AzureLinkboard.Storage.NoSql;
using Microsoft.WindowsAzure.Storage.Table;
using SavedUrl = AzureLinkboard.Storage.NoSql.SavedUrl;

namespace AzureLinkboard.Domain.Services.Implementation
{
    [ComponentIdentity(ComponentIdentities.TagFqn)]
    internal class TagService : AbstractApplicationComponent, ITagService
    {
        private readonly ITagParser _tagParser;
        private readonly IMapperFactory _mapperFactory;
        private readonly IAsynchronousNoSqlRepository<UserTag> _userTagsTable;
        private readonly IAsynchronousNoSqlRepository<DateOrderedUserTagItem> _dateOrderedUserTagItemsTable;
        private readonly IAsynchronousNoSqlRepository<UniqueUserTagItem> _uniqueUserTagItemsTable;
        private readonly IAsynchronousNoSqlRepository<SavedUrl> _repository; 

        public TagService(
            IApplicationResourceFactory applicationResourceFactory,
            ITagParser tagParser,
            IMapperFactory mapperFactory)
        {
            _tagParser = tagParser;
            _mapperFactory = mapperFactory;
            string dateOrderedTagItemsTableName = applicationResourceFactory.Setting(ComponentIdentity, "dateordered-tag-items-tablename");
            string uniqueTagItemsTableName = applicationResourceFactory.Setting(ComponentIdentity, "unique-tag-items-tablename");
            _userTagsTable = applicationResourceFactory.GetNoSqlRepository<UserTag>(ComponentIdentity);
            _dateOrderedUserTagItemsTable = applicationResourceFactory.GetNoSqlRepository<DateOrderedUserTagItem>(dateOrderedTagItemsTableName, ComponentIdentity);
            _uniqueUserTagItemsTable = applicationResourceFactory.GetNoSqlRepository<UniqueUserTagItem>(uniqueTagItemsTableName, ComponentIdentity);
            _repository = applicationResourceFactory.GetNoSqlRepository<SavedUrl>(ComponentIdentities.UrlStore);
        }

        public async Task ProcessTags(string userId, string url, DateTimeOffset savedAt, string tagsString)
        {
            List<string> tags = new List<string>(_tagParser.FromString(tagsString));
            foreach (string tagString in tags)
            {
                bool exists = false;
                UserTag tag = new UserTag(userId, tagString);
                DateOrderedUserTagItem dateOrderedTagItem = new DateOrderedUserTagItem(userId, tagString, url, savedAt);
                UniqueUserTagItem uniqueTagItem = new UniqueUserTagItem(userId, tagString, url, savedAt);
                await _userTagsTable.InsertOrReplaceAsync(tag);
                try
                {
                    await _uniqueUserTagItemsTable.InsertAsync(uniqueTagItem);
                }
                catch (UniqueKeyViolation)
                {
                    exists = true;
                }
                if (!exists)
                {
                    await _dateOrderedUserTagItemsTable.InsertAsync(dateOrderedTagItem);
                }
            }
        }

        public async Task<Page<Api.Model.SavedUrl>> GetTagItems(string userId, string tag, int pageSize, string continuationToken = null)
        {
            if (continuationToken != null)
            {
                continuationToken = continuationToken.Base64Decode();
            }
            PagedResultSegment<DateOrderedUserTagItem> segment =
                await
                    _dateOrderedUserTagItemsTable.PagedQueryAsync(
                        new Dictionary<string, object>
                        {
                            {"PartitionKey", DateOrderedUserTagItem.GetPartitionKey(userId, tag)}
                        }, pageSize,
                        continuationToken);
            if (!segment.Page.Any())
            {
                return new Page<Api.Model.SavedUrl>
                {
                    ContinuationToken = segment.ContinuationToken == null ? null : segment.ContinuationToken.Base64Encode(),
                    Items = new List<Api.Model.SavedUrl>()
                };
            }

            string partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId);
            List<string> rowFilters = segment.Page.Select(orderedUrl => TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, orderedUrl.Url.Base64Encode())).ToList();

            string rowFilter = rowFilters[0];
            for (int index = 1; index < rowFilters.Count; index++)
            {
                string subQueryString = rowFilters[index];
                rowFilter = TableQuery.CombineFilters(rowFilter, TableOperators.Or, subQueryString);
            }

            string filter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, rowFilter);

            PagedResultSegment<SavedUrl> resultPage = await _repository.PagedQueryAsync(filter, segment.Page.Count(), continuationToken);
            List<SavedUrl> results = resultPage.Page.OrderByDescending(r => r.SavedAt).ToList();
            return new Page<Api.Model.SavedUrl>
            {
                ContinuationToken = segment.ContinuationToken == null ? null : segment.ContinuationToken.Base64Encode(),
                Items = new List<Api.Model.SavedUrl>(_mapperFactory.GetSavedUrlMapper().Map(results))
            };
        }
    }
}
