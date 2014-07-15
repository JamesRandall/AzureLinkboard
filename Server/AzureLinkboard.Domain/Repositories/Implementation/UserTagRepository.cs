using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Core.Components;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using AzureLinkboard.Storage.NoSql;

namespace AzureLinkboard.Domain.Repositories.Implementation
{
    internal class UserTagRepository : IUserTagRepository
    {
        private readonly IAsynchronousNoSqlRepository<UserTag> _userTagsTable;
        private readonly IAsynchronousNoSqlRepository<DateOrderedUserTagItem> _dateOrderedUserTagItemsTable;
        private readonly IAsynchronousNoSqlRepository<UniqueUserTagItem> _uniqueUserTagItemsTable;

        public UserTagRepository(IApplicationResourceFactory applicationResourceFactory)
        {
            string dateOrderedTagItemsTableName = applicationResourceFactory.Setting(ComponentIdentities.Tag, "dateordered-tag-items-tablename");
            string uniqueTagItemsTableName = applicationResourceFactory.Setting(ComponentIdentities.Tag, "unique-tag-items-tablename");
            _userTagsTable = applicationResourceFactory.GetNoSqlRepository<UserTag>(ComponentIdentities.Tag);
            _dateOrderedUserTagItemsTable = applicationResourceFactory.GetNoSqlRepository<DateOrderedUserTagItem>(dateOrderedTagItemsTableName, ComponentIdentities.Tag);
            _uniqueUserTagItemsTable = applicationResourceFactory.GetNoSqlRepository<UniqueUserTagItem>(uniqueTagItemsTableName, ComponentIdentities.Tag);
        }

        public async Task Save(UserTag userTag)
        {
            await _userTagsTable.InsertOrReplaceAsync(userTag);
        }

        public async Task Save(DateOrderedUserTagItem dateOrderedUserTagItem)
        {
            await _dateOrderedUserTagItemsTable.InsertAsync(dateOrderedUserTagItem);
        }

        public async Task Save(UniqueUserTagItem uniqueUserTagItem)
        {
            await _uniqueUserTagItemsTable.InsertAsync(uniqueUserTagItem);
        }

        public async Task<PagedResultSegment<DateOrderedUserTagItem>> GetForUserAndTag(string userId, string tag, int pageSize, string continuationToken)
        {
            PagedResultSegment<DateOrderedUserTagItem> segment =
                await
                    _dateOrderedUserTagItemsTable.PagedQueryAsync(
                        new Dictionary<string, object>
                        {
                            {"PartitionKey", DateOrderedUserTagItem.GetPartitionKey(userId, tag)}
                        }, pageSize,
                        continuationToken);
            return segment;
        }
    }
}
