using AccidentalFish.ApplicationSupport.Core.Components;

namespace AzureLinkboard.Domain
{
    public class ComponentIdentities
    {
        public static readonly IComponentIdentity UrlStore = new ComponentIdentity(UrlStoreFqn);
        public static readonly IComponentIdentity PostedUrlProcessor = new ComponentIdentity(PostedUrlProcessorFqn);
        public static readonly IComponentIdentity Tag = new ComponentIdentity(TagFqn);

        public const string UrlStoreFqn = "com.accidentalfish.azurelinkboard.urlstore";
        public const string PostedUrlProcessorFqn = "com.accidentalfish.azurelinkboard.posted-url-processor";
        public const string TagFqn = "com.accidentalfish.azurelinkboard.tag";
    }
}
