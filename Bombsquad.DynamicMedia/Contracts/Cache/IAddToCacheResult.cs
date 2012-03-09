using System;

namespace Bombsquad.DynamicMedia.Contracts.Cache
{
    public interface IAddToCacheResult
    {
        DateTime LastModified { get; }
    }
}