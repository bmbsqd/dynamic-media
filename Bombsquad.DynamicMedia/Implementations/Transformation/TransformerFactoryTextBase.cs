using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts.Transformation;

namespace Bombsquad.DynamicMedia.Implementations.Transformation
{
    public abstract class TransformerFactoryTextBase : TransformerFactoryBase
    {
        protected override MediaTransformResult TransformStream(Stream stream, out Stream transformedStream)
        {
            string text;
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            string transformedText;
            var result = TransformText(text, out transformedText);

            transformedStream = new MemoryStream();
            var streamWriter = new StreamWriter(transformedStream);
            streamWriter.Write(transformedText);
            streamWriter.Flush();
            transformedStream.Seek(0, SeekOrigin.Begin);
            return result;
        }

        protected abstract MediaTransformResult TransformText(string text, out string transformedText);
    }
}