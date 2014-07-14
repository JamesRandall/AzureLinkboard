using System.Collections.Generic;

namespace AzureLinkboard.Api.Model
{
    public class Page<T> where T : class
    {
        public string ContinuationToken { get; set; }

        public List<T> Items { get; set; } 
    }
}
