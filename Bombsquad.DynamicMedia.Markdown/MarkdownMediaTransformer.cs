using System;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using MarkdownSharp;

namespace Bombsquad.DynamicMedia.Markdown
{
    public class MarkdownMediaTransformer : IMediaTransformer
    {
        private readonly Func<string, string> _modifyAbsolutePathFunction;

        public MarkdownMediaTransformer(Func<string, string> modifyAbsolutePathFunction, IFormatInfo outputFormat)
        {
            OutputFormat = outputFormat;
            _modifyAbsolutePathFunction = modifyAbsolutePathFunction;
        }

        public MediaTransformResult TransformStream(HttpRequestBase request, Stream stream, out Stream transformedStream)
        {
            string markdown;
            using (var reader = new StreamReader(stream))
            {
                markdown = reader.ReadToEnd();
            }

            var html = new MarkdownSharp.Markdown().Transform(markdown);
            transformedStream = new MemoryStream();
            var streamWriter = new StreamWriter(transformedStream);
            streamWriter.Write(html);
            streamWriter.Flush();
            transformedStream.Seek(0, SeekOrigin.Begin);
            return MediaTransformResult.Success;
        }

        public IFormatInfo OutputFormat { get; private set; }

        public string ModifyAbsolutePath(string absolutePath)
        {
            return _modifyAbsolutePathFunction(absolutePath);
        }
    }
}