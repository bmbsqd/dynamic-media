using System.Configuration;

namespace Bombsquad.DynamicMedia.Configuration
{
    public class FormatInfoElement : ConfigurationElement
    {
        [ConfigurationProperty("extension", IsRequired = true)]
        public string Extension
        {
            get
            {
                return this["extension"] as string;
            }
        }

        [ConfigurationProperty("contentType", IsRequired = true)]
        public string ContentType
        {
            get
            {
                return this["contentType"] as string;
            }
        }

        [ConfigurationProperty("allowCompression", IsRequired = false, DefaultValue = false)]
        public bool AllowCompression
        {
            get
            {
                return (bool)this["allowCompression"];
            }
        }

        [ConfigurationProperty("clientCacheMaxAge", IsRequired = false, DefaultValue = null)]
        public int? ClientCacheMaxAge
        {
            get
            {
                return (int?)this["clientCacheMaxAge"];
            }
        }
    }
}