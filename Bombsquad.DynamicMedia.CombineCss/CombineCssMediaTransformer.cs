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

        public MediaTransformResult TransformStream(HttpRequestBase request, Stream stream, out Stream transformedStream)
        {
            string content;

            using (var reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }

            string mergedCss;
            var result = MergeCss(content, out mergedCss);
            transformedStream = ServeResultString(mergedCss);
            return result;
        }

        private MediaTransformResult MergeCss(string content, out string mergedCss)
        {
            var result = MediaTransformResult.Success;
            mergedCss = CssImportRegex.Replace(content, delegate(Match m)
            {
                var stylesheetUrl = m.Groups["StylesheetUrl"].Value.Trim(new[] { '\'', '"' });
                string downloadedContent;
                if (TryGetStylesheetContent(new Uri(_requestUri, stylesheetUrl), out downloadedContent))
                {
                    return downloadedContent;
                }

                result = MediaTransformResult.FailedWithFallback;
                return m.Value;
            });
            return result;
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

        private bool TryGetStylesheetContent(Uri url, out string content)
        {
            try
            {
                var request = WebRequest.Create(url);
                using (var response = request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        content = reader.ReadToEnd();
                        return true;
                    }
                }
            }
            catch
            {
                content = null;
                return false;
            }
        }

        public IFormatInfo OutputFormat { get; private set; }

        public string ModifyAbsolutePath(string absolutePath)
        {
            return _modifyAbsolutePathFunction(absolutePath);
        }
    }
}