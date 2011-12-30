using System.Web;

namespace Bombsquad.DynamicMedia.Contracts
{
    public interface IFormatInfoResolver
    {
        IFormatInfo ResolveFromRequest(HttpRequestBase request);
    }
}
