using System.Web;

namespace Bombsquad.DynamicMedia.Contracts.FormatInfo
{
    public interface IFormatInfoResolver
    {
        IFormatInfo ResolveFromRequest(HttpRequestBase request);
    }
}
