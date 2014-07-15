using System.Threading.Tasks;
using AccidentalFish.ApplicationSupport.Core.NoSql;
using AzureLinkboard.Domain.Repositories;
using AzureLinkboard.Storage.NoSql;

namespace AzureLinkboard.Domain.Services.Implementation
{
    internal class UrlStatisticsService : IUrlStatisticsService
    {
        private readonly IUrlRepository _urlRepository;

        public UrlStatisticsService(IUrlRepository urlRepository)
        {
            _urlRepository = urlRepository;
        }

        public async Task<int> IncrementNumberOfSaves(string url)
        {
            UrlStatistics statistic = new UrlStatistics(url)
            {
                NumberOfSaves = 1
            };
            bool needsIncrement = false;
            try
            {
                await _urlRepository.Save(statistic);
            }
            catch (UniqueKeyViolation)
            {
                needsIncrement = true;
            }
            if (needsIncrement)
            {
                return await _urlRepository.IncrementNumberOfSaves(url);
            }
            return 1;
        }
    }
}
