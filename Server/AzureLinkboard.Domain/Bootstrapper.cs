using AccidentalFish.ApplicationSupport.Core.Components;
using AzureLinkboard.Domain.Helpers;
using AzureLinkboard.Domain.Helpers.Implementation;
using AzureLinkboard.Domain.Mappers;
using AzureLinkboard.Domain.Mappers.Implementation;
using AzureLinkboard.Domain.Processes;
using AzureLinkboard.Domain.Repositories;
using AzureLinkboard.Domain.Repositories.Implementation;
using AzureLinkboard.Domain.Services;
using AzureLinkboard.Domain.Services.Implementation;
using Microsoft.Practices.Unity;

namespace AzureLinkboard.Domain
{
    public static class Bootstrapper
    {
        public static void RegisterDependencies(IUnityContainer unityContainer)
        {
            // Helpers
            unityContainer.RegisterType<ITagParser, TagParser>();
            unityContainer.RegisterType<IMapperFactory, MapperFactory>();

            // Repositories
            unityContainer.RegisterType<IUrlRepository, UrlRepository>();
            unityContainer.RegisterType<IUserTagRepository, UserTagRepository>();
            unityContainer.RegisterType<IUrlStatisticsService, UrlStatisticsService>();

            // Services
            unityContainer.RegisterType<IUrlService, UrlService>();
            unityContainer.RegisterType<IUserTagService, UserTagService>();
        }

        public static void RegisterInfrastructure(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<IHostableComponent, PostedUrlProcessor>(ComponentIdentities.PostedUrlProcessorFqn);
        }
    }
}
