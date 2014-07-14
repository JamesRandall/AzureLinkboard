using System;
using System.Threading;
using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Core.Components;
using AccidentalFish.ApplicationSupport.Core.Extensions;
using AccidentalFish.ApplicationSupport.Core.Logging;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using AccidentalFish.ApplicationSupport.Core.Policies;
using AccidentalFish.ApplicationSupport.Core.Queues;
using AzureLinkboard.Domain.Services;
using AzureLinkboard.Storage.NoSql;
using AzureLinkboard.Storage.Queue;

namespace AzureLinkboard.Domain.Processes
{
    [ComponentIdentity(ComponentIdentities.PostedUrlProcessorFqn)]
    class PostedUrlProcessor : AbstractApplicationComponent, IHostableComponent
    {
        private const int MaxDequeues = 5;

        private readonly IAsynchronousBackoffPolicy _backoffPolicy;
        private readonly ITagService _tagService;
        private readonly IAsynchronousQueue<SavedUrlQueueItem> _queue;
        private readonly IAsynchronousQueue<SavedUrlQueueItem> _poisonQueue;
        private readonly IAsynchronousNoSqlRepository<SavedUrl> _savedUrlTable; 
        private readonly ILogger _logger;

        public PostedUrlProcessor(IApplicationResourceFactory applicationResourceFactory,
            ILoggerFactory loggerFactory,
            IAsynchronousBackoffPolicy backoffPolicy,
            ITagService tagService)
        {
            string poisonQueueName = applicationResourceFactory.Setting(ComponentIdentities.UrlStore, "poison-queuename");
            _backoffPolicy = backoffPolicy;
            _tagService = tagService;
            _queue = applicationResourceFactory.GetQueue<SavedUrlQueueItem>(ComponentIdentities.UrlStore);
            _poisonQueue = applicationResourceFactory.GetQueue<SavedUrlQueueItem>(poisonQueueName, ComponentIdentities.UrlStore);
            _logger = loggerFactory.CreateLongLivedLogger(ComponentIdentity);
            _savedUrlTable = applicationResourceFactory.GetNoSqlRepository<SavedUrl>(ComponentIdentities.UrlStore);
        }

        public async Task Start(CancellationToken token)
        {
            _backoffPolicy.Execute(AttemptDequeue, token);
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(500, token);
            }
        }

        private async void AttemptDequeue(Action<bool> backoffResultAction)
        {
            try
            {
                await _queue.DequeueAsync(item => ProcessQueueItem(item, backoffResultAction));
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to dequeue posted URL", ex).Wait();
            }
        }

        private async Task<bool> ProcessQueueItem(IQueueItem<SavedUrlQueueItem> queueItem, Action<bool> backoffResultAction)
        {
            if (queueItem == null)
            {
                backoffResultAction(false);
                return false;
            }
            SavedUrlQueueItem item = null;
            try
            {
                item = queueItem.Item;
                SavedUrl savedUrl = await _savedUrlTable.GetAsync(item.UserId, item.Url.Base64Encode());
                if (savedUrl != null)
                {
                    await _tagService.ProcessTags(savedUrl.UserId, savedUrl.Url, savedUrl.SavedAt, savedUrl.Tags);
                }
                else
                {
                    await _logger.Warning(String.Format("Unable to find saved url userid:{0} url:{1}", item.UserId, item.Url));
                }
                return true;
            }
            catch (Exception ex)
            {
                if (queueItem.DequeueCount >= MaxDequeues && item != null)
                {
                    _logger.Error("Unable to process queue item after multiple attempts. Giving up and attempting to place on poison queue for manual attention / later requeueing.", ex).Wait();
                    try
                    {
                        _poisonQueue.EnqueueAsync(item).Wait(TimeSpan.FromSeconds(30));
                    }
                    catch
                    {
                        // swallow any problem here, storage is likely dying around us
                    }
                }
                return queueItem.DequeueCount >= MaxDequeues;
            }
            finally
            {
                backoffResultAction(true);
            }
        }
    }
}
