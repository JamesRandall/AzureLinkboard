using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AzureLinkboard.Api.Model
{
    public class SavedUrl
    {
        public string UserId { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTimeOffset SavedAt { get; set; }

        public DateTimeOffset? LastVisited { get; set; }

        public int NumberOfVisits { get; set; }

        public List<string> Tags { get; set; } 
    }
}
