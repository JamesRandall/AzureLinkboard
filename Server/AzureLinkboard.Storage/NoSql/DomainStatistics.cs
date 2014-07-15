using AccidentalFish.ApplicationSupport.Core.NoSql;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureLinkboard.Storage.NoSql
{
    /// <summary>
    /// Number of times a specific domain has been saved (as opposed to full URL)
    /// </summary>
    public class DomainStatistics : NoSqlEntity
    {
        public DomainStatistics()
        {
            
        }

        public DomainStatistics(string domain)
        {
            // shouldn't need base 64 encoding as should only include [a-z][A-Z][0-9][.] characters
            // should be validated in service
            PartitionKey = domain;
        }

        [IgnoreProperty]
        public string Domain { get { return PartitionKey; } }

        public int NumberOfSaves { get; set; }

        public int NumberOfClicks { get; set; }
    }
}
