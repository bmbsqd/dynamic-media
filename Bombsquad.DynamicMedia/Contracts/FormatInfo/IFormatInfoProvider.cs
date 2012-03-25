using System.Web;

namespace Bombsquad.DynamicMedia.Contracts.FormatInfo
{
    public interface IFormatInfoProvider
    {
        IFormatInfo ResolveFromExtension(string extension);
    }
}
