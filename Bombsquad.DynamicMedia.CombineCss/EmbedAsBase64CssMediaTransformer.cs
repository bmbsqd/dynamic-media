using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia.CombineCss
{
    public class EmbedAsBase64CssMediaTransformer : IMediaTransformer
    {
        private readonly Uri _requestUri;
        private readonly Func<string, string> _modifyAbsolutePathFunc;

        public EmbedAsBase64CssMediaTransformer(Uri requestUri, IFormatInfo outputFormat, Func<string, string> modifyAbsolutePathFunc)
        {
            OutputFormat = outputFormat;
            _requestUri = requestUri;
            _modifyAbsolutePathFunc = modifyAbsolutePathFunc;
        }

        private static readonly Regex BackgroundImagesRexgex = new Regex(@"background-image:\s+url\s*\((?<Url>.*)\);", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

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
            
            mergedCss = BackgroundImagesRexgex.Replace(content, delegate(Match m)
            {
                var url = m.Groups["Url"].Value.Trim(new[] { '\'', '"' });

                string base64;
                string contentType;
                if (!TryGetResourceAsBase64(new Uri(_requestUri, url), out base64, out contentType))
                {
                    result = MediaTransformResult.FailedWithFallback;
                    return m.Value;
                }

                var output = new StringBuilder();
                output.Append("background-image: url(data:");
                output.Append(contentType);
                output.Append(";base64,");
                output.Append(base64);
                output.Append(");");
                return output.ToString();
            });
            return result;
        }

        private bool TryGetResourceAsBase64(Uri url, out string base64, out string contentType)
        {
            var request = WebRequest.Create(url);
            using (var response = request.GetResponse())
            {
                contentType = response.ContentType;
                using (var stream = response.GetResponseStream())
                {
                    using (var memoryStream = new MemoryStream((int)response.ContentLength))
                    {
                        stream.CopyTo(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        base64 = Convert.ToBase64String(memoryStream.ToArray());
                        return true;
                    }
                }
            }
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

        public IFormatInfo OutputFormat { get; private set; }

        public string ModifyAbsolutePath(string absolutePath)
        {
            return _modifyAbsolutePathFunc(absolutePath);
        }
    }
}