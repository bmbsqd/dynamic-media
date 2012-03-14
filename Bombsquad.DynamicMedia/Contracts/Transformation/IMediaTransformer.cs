using System.IO;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;

namespace Bombsquad.DynamicMedia.Contracts.Transformation
{
    public interface IMediaTransformer
    {
        MediaTransformResult TransformStream(Stream stream, out Stream transformedStream);
        IFormatInfo OutputFormat { get; }
        string ModifyAbsolutePath(string absolutePath);
    }

    public enum MediaTransformResult
    {
        Success,
        FailedWithFallback
    }
}