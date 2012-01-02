using System.IO;
using System.Web;

namespace Bombsquad.DynamicMedia.Contracts
{
    public interface IMediaTransformer
    {
        MediaTransformResult TransformStream(HttpRequestBase request, Stream stream, out Stream transformedStream);
        IFormatInfo OutputFormat { get; }
        string ModifyAbsolutePath(string absolutePath);
    }

    public enum MediaTransformResult
    {
        Success,
        FailedWithFallback
    }
}