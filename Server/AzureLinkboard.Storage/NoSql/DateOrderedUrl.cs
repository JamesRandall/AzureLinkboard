using System;
using AccidentalFish.ApplicationSupport.Core.NoSql;

namespace AzureLinkboard.Storage.NoSql
{
    public class DateOrderedUrl : NoSqlEntity
    {
        public DateOrderedUrl()
        {
            
        }

        public DateOrderedUrl(string userId, DateTimeOffset savedAt, string url)
        {
            PartitionKey = userId;
            RowKey = String.Format("{0:D19}", DateTimeOffset.MaxValue.Ticks - savedAt.Ticks);
            Url = url;
        }

        public string Url { get; set; }
    }
}
