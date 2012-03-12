using System.IO;

namespace Bombsquad.DynamicMedia.Contracts.Storage
{
    public interface IStorageFile : IResult
    {
    	Stream GetStream();
    }
}