using System.Collections.Generic;
using AccidentalFish.ApplicationSupport.Core.Mappers;
using AzureLinkboard.Api.Model;
using AzureLinkboard.Domain.Helpers;

namespace AzureLinkboard.Domain.Mappers.Implementation
{
    internal class SavedUrlMapper : AbstractMapper<Storage.NoSql.SavedUrl, Api.Model.SavedUrl>
    {
        private readonly ITagParser _tagParser;

        public SavedUrlMapper(ITagParser tagParser)
        {
            _tagParser = tagParser;
        }

        public override SavedUrl Map(Storage.NoSql.SavedUrl @from)
        {
            return new SavedUrl
            {
                Description = @from.Description,
                LastVisited = @from.LastVisited,
                NumberOfVisits = @from.NumberOfVisits,
                SavedAt = @from.SavedAt,
                Tags = new List<string>(_tagParser.FromString(@from.Tags)),
                Title = @from.Title,
                Url = @from.Url,
                UserId = @from.UserId
            };
        }

        public override Storage.NoSql.SavedUrl Map(SavedUrl @from)
        {
            return new Storage.NoSql.SavedUrl(@from.UserId, @from.Url)
            {
                Description = @from.Description,
                LastVisited = @from.LastVisited,
                NumberOfVisits = @from.NumberOfVisits,
                SavedAt = @from.SavedAt,
                Tags = _tagParser.ToString(@from.Tags),
                Title = @from.Title
            };
        }
    }
}
