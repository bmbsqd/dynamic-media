using System.IO;
using System.Web;

namespace Bombsquad.DynamicMedia.Contracts
{
    public interface IStorageBackend
    {
        bool TryGetOriginalStream(HttpRequestBase request, IMediaTransformer mediaTransformer, out Stream outputStream);
    }
}