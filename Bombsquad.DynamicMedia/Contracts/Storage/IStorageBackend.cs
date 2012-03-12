using System.Web;
using Bombsquad.DynamicMedia.Contracts.Transformation;

namespace Bombsquad.DynamicMedia.Contracts.Storage
{
    public interface IStorageBackend
    {
        bool TryGetStorageFile(HttpRequestBase request, IMediaTransformer mediaTransformer, out IStorageFile storageFile);
    }
}