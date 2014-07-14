using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Core.Components;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using AccidentalFish.ApplicationSupport.Core.Queues;
using AccidentalFish.ApplicationSupport.Core.Extensions;
using AzureLinkboard.Api.Model;
using AzureLinkboard.Domain.Mappers;
using AzureLinkboard.Domain.Validation;
using AzureLinkboard.Storage.NoSql;
using AzureLinkboard.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using NoSqlSavedUrl = AzureLinkboard.Storage.NoSql.SavedUrl;
using SavedUrl = AzureLinkboard.Api.Model.SavedUrl;

namespace AzureLinkboard.Domain.Services.Implementation
{
    [ComponentIdentity(ComponentIdentities.UrlStoreFqn)]
    internal class UrlService : AbstractApplicationComponent, IUrlService
    {
        private readonly IMapperFactory _mapperFactory;
        private readonly IAsynchronousNoSqlRepository<NoSqlSavedUrl> _repository;
        private readonly IAsynchronousNoSqlRepository<DateOrderedUrl> _dateOrderedRepository; 
        private readonly IAsynchronousQueue<SavedUrlQueueItem> _queue; 

        public UrlService(
            IApplicationResourceFactory applicationResourceFactory,
            IMapperFactory mapperFactory)
        {
            string dateOrderedTableName = applicationResourceFactory.Setting(ComponentIdentity, "date-ordered-tablename");

            _mapperFactory = mapperFactory;
            _repository = applicationResourceFactory.GetNoSqlRepository<NoSqlSavedUrl>(ComponentIdentity);
            _dateOrderedRepository = applicationResourceFactory.GetNoSqlRepository<DateOrderedUrl>(dateOrderedTableName, ComponentIdentity);
            _queue = applicationResourceFactory.GetQueue<SavedUrlQueueItem>(ComponentIdentity);
        }

        public async Task<Page<SavedUrl>> Get(string userId, int pageSize, string continuationToken = null)
        {
            if (continuationToken != null)
            {
                continuationToken = continuationToken.Base64Decode();
            }
            PagedResultSegment<DateOrderedUrl> segment = await _dateOrderedRepository.PagedQueryAsync(new Dictionary<string, object> {{"PartitionKey", userId}}, pageSize, continuationToken);
            if (!segment.Page.Any())
            {
                return new Page<SavedUrl>
                {
                    ContinuationToken = segment.ContinuationToken == null ? null : segment.ContinuationToken.Base64Encode(),
                    Items = new List<SavedUrl>()
                };
            }
            

            string partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId);
            List<string> rowFilters = segment.Page.Select(orderedUrl => TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, orderedUrl.Url.Base64Encode())).ToList();

            string rowFilter = rowFilters[0];
            for (int index = 1; index < rowFilters.Count; index++)
            {
                string subQueryString = rowFilters[index];
                rowFilter = TableQuery.CombineFilters(rowFilter, TableOperators.Or, subQueryString);
            }

            string filter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, rowFilter);

            PagedResultSegment<NoSqlSavedUrl> resultPage = await _repository.PagedQueryAsync(filter, segment.Page.Count(), continuationToken);
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
                await _repository.InsertAsync(savedUrl);
            }
            catch (UniqueKeyViolation)
            {
                ValidationResult<SavedUrl> result = new ValidationResult<SavedUrl>();
                result.AddError("Link already exists");
                return result;
            }
            
            await Task.WhenAll(new[]
            {
                _dateOrderedRepository.InsertAsync(new DateOrderedUrl(userId, savedUrl.SavedAt, savedUrl.Url)),
                _queue.EnqueueAsync(queueItem)
            });

            return new ValidationResult<SavedUrl>(_mapperFactory.GetSavedUrlMapper().Map(savedUrl));
        }
    }
}
