using System.Collections.Generic;
using System.Configuration;

namespace Bombsquad.DynamicMedia.Configuration
{
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