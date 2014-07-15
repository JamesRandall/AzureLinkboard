using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Core.Components;
using AccidentalFish.ApplicationSupport.Core.Extensions;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using AzureLinkboard.Api.Model;
using AzureLinkboard.Domain.Helpers;
using AzureLinkboard.Domain.Mappers;
using AzureLinkboard.Domain.Repositories;
using AzureLinkboard.Storage.NoSql;
using SavedUrl = AzureLinkboard.Storage.NoSql.SavedUrl;

namespace AzureLinkboard.Domain.Services.Implementation
{
    [ComponentIdentity(ComponentIdentities.TagFqn)]
    internal class TagService : AbstractApplicationComponent, ITagService
    {
        private readonly ITagParser _tagParser;
        private readonly IMapperFactory _mapperFactory;
        private readonly IUrlRepository _urlRepository;
        private readonly IUserTagRepository _userTagRepository;


        public TagService(
            ITagParser tagParser,
            IMapperFactory mapperFactory,
            IUrlRepository urlRepository,
            IUserTagRepository userTagRepository)
        {
            _tagParser = tagParser;
            _mapperFactory = mapperFactory;
            _urlRepository = urlRepository;
            _userTagRepository = userTagRepository;
        }

        public async Task ProcessTags(string userId, string url, DateTimeOffset savedAt, string tagsString)
        {
            List<string> tags = new List<string>(_tagParser.FromString(tagsString));
            foreach (string tagString in tags)
            {
                bool exists = false;
                UserTag tag = new UserTag(userId, tagString);
                DateOrderedUserTagItem dateOrderedTagItem = new DateOrderedUserTagItem(userId, tagString, url, savedAt);
                UniqueUserTagItem uniqueTagItem = new UniqueUserTagItem(userId, tagString, url, savedAt);
                await _userTagRepository.Save(tag);
                try
                {
                    await _userTagRepository.Save(uniqueTagItem);
                }
                catch (UniqueKeyViolation)
                {
                    exists = true;
                }
                if (!exists)
                {
                    await _userTagRepository.Save(dateOrderedTagItem);
                }
            }
        }

        public async Task<Page<Api.Model.SavedUrl>> GetTagItems(string userId, string tag, int pageSize, string continuationToken = null)
        {
            if (continuationToken != null)
            {
                continuationToken = continuationToken.Base64Decode();
            }
            PagedResultSegment<DateOrderedUserTagItem> segment = await _userTagRepository.GetForUserAndTag(userId, tag, pageSize, continuationToken);
            
            if (!segment.Page.Any())
            {
                return new Page<Api.Model.SavedUrl>
                {
                    ContinuationToken = segment.ContinuationToken == null ? null : segment.ContinuationToken.Base64Encode(),
                    Items = new List<Api.Model.SavedUrl>()
                };
            }

            PagedResultSegment<SavedUrl> resultPage = await _urlRepository.GetByUrls(userId, segment.Page.Select(x => x.Url));
            List<SavedUrl> results = resultPage.Page.OrderByDescending(r => r.SavedAt).ToList();
            return new Page<Api.Model.SavedUrl>
            {
                ContinuationToken = segment.ContinuationToken == null ? null : segment.ContinuationToken.Base64Encode(),
                Items = new List<Api.Model.SavedUrl>(_mapperFactory.GetSavedUrlMapper().Map(results))
            };
        }
    }
}
