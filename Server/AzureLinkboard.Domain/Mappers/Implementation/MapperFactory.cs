using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Core.Mappers;
using AzureLinkboard.Domain.Helpers;
using AzureLinkboard.Storage.NoSql;

namespace AzureLinkboard.Domain.Mappers.Implementation
{
    internal class MapperFactory : IMapperFactory
    {
        private readonly ITagParser _tagParser;

        public MapperFactory(ITagParser tagParser)
        {
            _tagParser = tagParser;
        }

        public IMapper<SavedUrl, Api.Model.SavedUrl> GetSavedUrlMapper()
        {
            return new SavedUrlMapper(_tagParser);
        }
    }
}
