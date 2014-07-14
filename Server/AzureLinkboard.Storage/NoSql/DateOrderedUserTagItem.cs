using System;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLinkboard.Storage.NoSql
{
    public class DateOrderedUserTagItem : NoSqlEntity
    {
        public DateOrderedUserTagItem()
        {
            
        }

        public DateOrderedUserTagItem(string userId, string tag, string url, DateTimeOffset savedAt)
        {
            PartitionKey = GetPartitionKey(userId, tag);
            RowKey = String.Format("{0:D19}", DateTimeOffset.MaxValue.Ticks - savedAt.Ticks);
            Tag = tag;
            Url = url;
            UserId = userId;
        }

        public string UserId { get; set; }

        public string Tag { get; set; }

        public string Url { get; set; }

        public static string GetPartitionKey(string userId, string tag)
        {
            return String.Format("{0}-{1}", userId, tag);
        }
    }
}
