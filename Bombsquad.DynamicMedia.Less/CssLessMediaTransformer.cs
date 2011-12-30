using System;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.Less
{
    public class CssLessMediaTransformer : IMediaTransformer
    {
        private readonly Func<string, string> _modifyAbsolutePathFunction;

        public CssLessMediaTransformer(Func<string, string> modifyAbsolutePathFunction, IFormatInfo outputFormat)
        {
            OutputFormat = outputFormat;
            _modifyAbsolutePathFunction = modifyAbsolutePathFunction;
        }

        public Stream TransformStream(HttpRequestBase request, Stream stream)
        {
            string less;
            using (var reader = new StreamReader(stream))
            {
                less = reader.ReadToEnd();
            }

            var parsed = dotless.Core.Less.Parse(less);
            var output = new MemoryStream();
            var streamWriter = new StreamWriter(output);
            streamWriter.Write(parsed);
            streamWriter.Flush();
            output.Seek(0, SeekOrigin.Begin);
            return output;
        }

        public IFormatInfo OutputFormat { get; private set; }

        public string ModifyAbsolutePath(string absolutePath)
        {
            return _modifyAbsolutePathFunction(absolutePath);
        }
    }
}