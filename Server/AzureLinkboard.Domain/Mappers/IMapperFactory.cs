using AccidentalFish.ApplicationSupport.Core.Mappers;

namespace AzureLinkboard.Domain.Mappers
{
    internal interface IMapperFactory
    {
        IMapper<Storage.NoSql.SavedUrl, Api.Model.SavedUrl> GetSavedUrlMapper();
    }
}
