using System.Web;

namespace Bombsquad.DynamicMedia.Contracts
{
    public interface IMediaTransformerFactory
    {
        bool TryCreateTransformer(HttpRequestBase request, IFormatInfo originalFormat, out IMediaTransformer mediaTransformer);
    }
}