using System.Web;
using Bombsquad.DynamicMedia.Contracts.Transformation;

namespace Bombsquad.DynamicMedia.Contracts.Storage
{
    public interface IStorageBackend
    {
        bool TryGetOriginalStream(HttpRequestBase request, IMediaTransformer mediaTransformer, out IOriginal output);
        bool TryServeOriginal(HttpRequestBase request, out IResult result);
    }
}