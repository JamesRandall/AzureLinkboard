using System;
using AccidentalFish.ApplicationSupport.Core.Extensions;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLinkboard.Storage.NoSql
{
    public class SavedUrl : NoSqlEntity
    {
        public SavedUrl()
        {
            
        }

        public SavedUrl(string userId, string url)
        {
            PartitionKey = userId;
            RowKey = url.Base64Encode();
        }

        [IgnoreProperty]
        public string UserId { get { return PartitionKey; } }

        [IgnoreProperty]
        public string Url { get { return RowKey.Base64Decode(); } }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTimeOffset SavedAt { get; set; }

        public DateTimeOffset? LastVisited { get; set; }

        public int NumberOfVisits { get; set; }

        public string Tags { get; set; }
    }
}
