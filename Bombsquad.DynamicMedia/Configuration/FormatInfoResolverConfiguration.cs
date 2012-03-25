using System.Configuration;

namespace Bombsquad.DynamicMedia.Configuration
{
    public class FormatInfoResolverConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("mappings")]
        public FormatInfoElementCollection Mappings
        {
            get { return this["mappings"] as FormatInfoElementCollection; }
        }
    }
}
