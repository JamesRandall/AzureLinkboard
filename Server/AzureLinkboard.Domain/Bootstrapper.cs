using AccidentalFish.ApplicationSupport.Core.Components;
using AzureLinkboard.Domain.Helpers;
using AzureLinkboard.Domain.Helpers.Implementation;
using AzureLinkboard.Domain.Mappers;
using AzureLinkboard.Domain.Mappers.Implementation;
using AzureLinkboard.Domain.Processes;
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

            // Services
            unityContainer.RegisterType<IUrlService, UrlService>();
            unityContainer.RegisterType<ITagService, TagService>();
        }

        public static void RegisterInfrastructure(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<IHostableComponent, PostedUrlProcessor>(ComponentIdentities.PostedUrlProcessorFqn);
        }
    }
}
