using AccidentalFish.ApplicationSupport.Core.Extensions;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLinkboard.Storage.NoSql
{
    public class UrlTagItem : NoSqlEntity
    {
        public UrlTagItem()
        {
            
        }

        public UrlTagItem(string url, string tag)
        {
            PartitionKey = url.Base64Encode();
            RowKey = tag.Base64Encode();
        }

        [IgnoreProperty]
        public string Url { get { return PartitionKey.Base64Decode(); } }

        public string Tag { get { return RowKey.Base64Decode(); } }
    }
}
