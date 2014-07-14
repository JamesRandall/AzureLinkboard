using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Core.Extensions;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLinkboard.Storage.NoSql
{
    public class UniqueUserTagItem : NoSqlEntity
    {
        public UniqueUserTagItem()
        {
            
        }

        public UniqueUserTagItem(string userId, string tag, string url, DateTimeOffset savedAt)
        {
            PartitionKey = PartitionKey = String.Format("{0}-{1}", userId, tag);
            RowKey = url.Base64Encode();
            Tag = tag;
            UserId = userId;
            DateOrderedRowKey = String.Format("{0:D19}", DateTimeOffset.MaxValue.Ticks - savedAt.Ticks);
        }

        public string UserId { get; set; }

        public string Tag { get; set; }

        public string DateOrderedRowKey { get; set; }

        [IgnoreProperty]
        public string Url { get { return RowKey.Base64Decode(); } }
    }
}
