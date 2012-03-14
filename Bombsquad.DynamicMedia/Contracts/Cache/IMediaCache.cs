using System;
using System.IO;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Contracts.Cache
{
    public interface IMediaCache
    {
        bool TryServeRequestFromCache(string path, IFormatInfo outputFormat, out IResult result);
        bool TryAddToCache(string path, Func<Stream> stream, IFormatInfo outputFormat, out IAddToCacheResult result);
    }
}