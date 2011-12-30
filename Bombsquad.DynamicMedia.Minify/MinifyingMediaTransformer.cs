using System;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.Minify
{
    public class MinifyingMediaTransformer : IMediaTransformer
    {
        private readonly Func<string, string> _minifyFunction;
        private readonly Func<string, string> _modifyAbsolutePathFunction;

        public MinifyingMediaTransformer(IFormatInfo formatInfo, Func<string,string> minifyFunction, Func<string,string> modifyAbsolutePathFunction )
        {
            OutputFormat = formatInfo;
            _minifyFunction = minifyFunction;
            _modifyAbsolutePathFunction = modifyAbsolutePathFunction;
        }

        public Stream TransformStream(HttpRequestBase request, Stream stream)
        {
            string content;

            using( var reader = new StreamReader(stream) )
            {
                content = reader.ReadToEnd();
            }

            var output = new MemoryStream();
            var writer = new StreamWriter(output);
            writer.Write(_minifyFunction(content));
            writer.Flush();
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