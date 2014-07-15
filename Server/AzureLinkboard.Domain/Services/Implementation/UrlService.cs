﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Core.Components;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using AccidentalFish.ApplicationSupport.Core.Queues;
using AccidentalFish.ApplicationSupport.Core.Extensions;
using AzureLinkboard.Api.Model;
using AzureLinkboard.Domain.Mappers;
using AzureLinkboard.Domain.Repositories;
using AzureLinkboard.Domain.Validation;
using AzureLinkboard.Storage.NoSql;
using AzureLinkboard.Storage.Queue;
using NoSqlSavedUrl = AzureLinkboard.Storage.NoSql.SavedUrl;
using SavedUrl = AzureLinkboard.Api.Model.SavedUrl;

namespace AzureLinkboard.Domain.Services.Implementation
{
    [ComponentIdentity(ComponentIdentities.UrlStoreFqn)]
    internal class UrlService : AbstractApplicationComponent, IUrlService
    {
        private readonly IUrlRepository _urlRepository;
        private readonly IMapperFactory _mapperFactory;
        private readonly IAsynchronousQueue<SavedUrlQueueItem> _queue; 

        public UrlService(
            IUrlRepository urlRepository,
            IApplicationResourceFactory applicationResourceFactory,
            IMapperFactory mapperFactory)
        {
            _urlRepository = urlRepository;
            _mapperFactory = mapperFactory;
            _queue = applicationResourceFactory.GetQueue<SavedUrlQueueItem>(ComponentIdentity);
        }

        public async Task<Page<SavedUrl>> Get(string userId, int pageSize, string continuationToken = null)
        {
            if (continuationToken != null)
            {
                continuationToken = continuationToken.Base64Decode();
            }

            PagedResultSegment<DateOrderedUrl> segment = await _urlRepository.GetDateOrderIndexesForUser(userId, pageSize, continuationToken);
            if (segment == null)
            {
                return new Page<SavedUrl>
                {
                    ContinuationToken = null,
                    Items = new List<SavedUrl>()
                };
            }
            PagedResultSegment<NoSqlSavedUrl> resultPage = await _urlRepository.GetByUrls(userId, segment.Page.Select(x => x.Url));
            List<NoSqlSavedUrl> results = resultPage.Page.OrderByDescending(r => r.SavedAt).ToList();
            return new Page<SavedUrl>
            {
                ContinuationToken = segment.ContinuationToken == null ? null : segment.ContinuationToken.Base64Encode(),
                Items = new List<SavedUrl>(_mapperFactory.GetSavedUrlMapper().Map(results))
            };
        }

        public async Task<ValidationResult<SavedUrl>> Save(string userId, SaveUrlRequest model)
        {
            NoSqlSavedUrl savedUrl = new NoSqlSavedUrl(userId, model.Url)
            {
                Description = model.Description,
                NumberOfVisits = 0,
                SavedAt = DateTimeOffset.UtcNow,
                Title = model.Title,
                Tags = model.Tags
            };

            SavedUrlQueueItem queueItem = new SavedUrlQueueItem
            {
                Url = model.Url,
                UserId = userId
            };

            try
            {
                await _urlRepository.Save(savedUrl);
            }
            catch (UniqueKeyViolation)
            {
                ValidationResult<SavedUrl> result = new ValidationResult<SavedUrl>();
                result.AddError("Link already exists");
                return result;
            }
            
            await Task.WhenAll(new[]
            {
                _urlRepository.Save(new DateOrderedUrl(userId, savedUrl.SavedAt, savedUrl.Url)),
                _queue.EnqueueAsync(queueItem)
            });

            return new ValidationResult<SavedUrl>(_mapperFactory.GetSavedUrlMapper().Map(savedUrl));
        }
    }
}
