using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.CombineCss
{
    public class CombineCssMediaTransformer : IMediaTransformer
    {
        private readonly Uri _requestUri;
        private readonly Func<string, string> _modifyAbsolutePathFunction;

        public CombineCssMediaTransformer(Uri requestUri, IFormatInfo outputFormat, Func<string, string> modifyAbsolutePathFunction)
        {
            OutputFormat = outputFormat;
            _requestUri = requestUri;
            _modifyAbsolutePathFunction = modifyAbsolutePathFunction;
        }

        private static readonly Regex CssImportRegex = new Regex(@"@import\s+url\s*\((?<StylesheetUrl>.*)\);", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public Stream TransformStream(HttpRequestBase request, Stream stream)
        {
            string content;

            using( var reader = new StreamReader(stream) )
            {
                content = reader.ReadToEnd();
            }

            return ServeResultString(MergeCss(content));
        }

        private string MergeCss(string content)
        {
            return CssImportRegex.Replace(content, delegate(Match m)
            {
                var stylesheetUrl = m.Groups["StylesheetUrl"].Value.Trim(new[] {'\'', '"'});
                return GetStylesheetContent(new Uri(_requestUri, stylesheetUrl));
            });
        }

        private static Stream ServeResultString(string content)
        {
            var output = new MemoryStream();
            var writer = new StreamWriter(output);
            writer.Write(content);
            writer.Flush();
            output.Seek(0, SeekOrigin.Begin);
            return output;
        }

        private string GetStylesheetContent(Uri url)
        {
            var request = WebRequest.Create(url);
            using(var response = request.GetResponse())
            {
                using(var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public IFormatInfo OutputFormat { get; private set; }

        public string ModifyAbsolutePath(string absolutePath)
        {
            return _modifyAbsolutePathFunction(absolutePath);
        }
    }
}