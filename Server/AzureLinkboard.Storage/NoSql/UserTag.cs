using AccidentalFish.ApplicationSupport.Core.Extensions;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLinkboard.Storage.NoSql
{
    public class UserTag : NoSqlEntity
    {
        public UserTag()
        {
            
        }

        public UserTag(string userId, string tag)
        {
            PartitionKey = userId;
            RowKey = tag.Base64Encode();
        }

        [IgnoreProperty]
        public string UserId { get { return PartitionKey; } }

        [IgnoreProperty]
        public string Tag { get { return RowKey.Base64Decode(); } }
    }

}
