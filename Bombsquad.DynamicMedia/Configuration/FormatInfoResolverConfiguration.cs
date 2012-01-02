using System.Collections.Generic;
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
    }

    public class FormatInfoElementCollection : ConfigurationElementCollection, IEnumerable<FormatInfoElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FormatInfoElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FormatInfoElement)element).Extension;
        }

        public IEnumerator<FormatInfoElement> GetEnumerator()
        {
            for(var i = 0; i < Count; i++)
            {
                yield return BaseGet(i) as FormatInfoElement;
            }
        }
    }
}
