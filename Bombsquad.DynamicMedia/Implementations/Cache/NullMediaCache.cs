using System.IO;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.Cache;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Implementations.Cache
{
    public class NullMediaCache : IMediaCache
    {
        public bool TryServeRequestFromCache(string path, IFormatInfo outputFormat, out IResult result)
        {
            result = null;
            return false;
        }

        public bool TryAddToCache(string path, Stream stream, IFormatInfo outputFormat, out IAddToCacheResult result)
        {
            result = null;
            return false;
        }
    }
}