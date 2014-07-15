using AccidentalFish.ApplicationSupport.Core.Extensions;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLinkboard.Storage.NoSql
{
    /// <summary>
    /// Used to track the number of times a full URL is saved and clicked
    /// </summary>
    public class UrlStatistics : NoSqlEntity
    {
        public UrlStatistics()
        {
            
        }

        public UrlStatistics(string url)
        {
            PartitionKey = url.Base64Encode();
            RowKey = "";
        }

        [IgnoreProperty]
        public string Url { get { return PartitionKey.Base64Decode(); } }

        public int NumberOfSaves { get; set; }

        public int NumberOfClicks { get; set; }
    }
}
