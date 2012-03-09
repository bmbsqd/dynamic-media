using System;
using System.IO;

namespace Bombsquad.DynamicMedia.Contracts.Storage
{
    public interface IOriginal
    {
        Stream Stream { get; }
        DateTime LastModified { get; }
    }
}