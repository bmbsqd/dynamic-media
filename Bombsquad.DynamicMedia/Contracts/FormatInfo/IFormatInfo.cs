using System;

namespace Bombsquad.DynamicMedia.Contracts.FormatInfo
{
    public interface IFormatInfo
    {
        string Extension { get; }
        string ContentType { get; }
        bool AllowCompression { get; }
        TimeSpan? ClientCacheMaxAge { get; }
    }
}