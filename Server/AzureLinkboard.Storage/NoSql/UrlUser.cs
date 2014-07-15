using AccidentalFish.ApplicationSupport.Core.Extensions;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLinkboard.Storage.NoSql
{
    public class UrlUser : NoSqlEntity
    {
        public UrlUser()
        {
            
        }

        public UrlUser(string url, string userId)
        {
            PartitionKey = url.Base64Encode();
            RowKey = userId;
        }

        [IgnoreProperty]
        public string UserId { get { return RowKey; } }

        public string Url { get { return PartitionKey.Base64Decode(); } }
    }
}
