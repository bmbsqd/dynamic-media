using System.IO;
using System.Web;

namespace Bombsquad.DynamicMedia.Contracts
{
    public interface IMediaTransformer
    {
        Stream TransformStream(HttpRequestBase request, Stream stream);
        IFormatInfo OutputFormat { get; }
        string ModifyAbsolutePath(string absolutePath);
    }
}