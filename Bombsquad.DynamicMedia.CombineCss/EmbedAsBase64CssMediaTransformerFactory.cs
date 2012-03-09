using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Contracts.Transformation;
using Bombsquad.DynamicMedia.Implementations;
using Bombsquad.DynamicMedia.Implementations.Transformation;

namespace Bombsquad.DynamicMedia.CombineCss
{
    public class EmbedAsBase64CssMediaTransformerFactory : TransformerFactoryTextBase
    {
        protected override bool IsValidFilePath(string absolutePath)
        {
            return absolutePath.Contains(".embed.");
        }

        protected override bool CanHandleFormat(IFormatInfo format)
        {
            return string.Equals(format.ContentType, "text/css", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override string ModifyAbsolutePath(string absolutePath)
        {
            return absolutePath.Replace(".embed", "");
        }

        private static readonly Regex BackgroundImagesRexgex = new Regex(@"background-image:\s+url\s*\((?<Url>.*)\);", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        protected override MediaTransformResult TransformText(string text, out string transformedText)
        {
            var result = MediaTransformResult.Success;

            transformedText = BackgroundImagesRexgex.Replace(text, delegate(Match m)
            {
                var url = m.Groups["Url"].Value.Trim(new[] { '\'', '"' });

                string base64;
                string contentType;
                if (!TryGetResourceAsBase64(new Uri(HttpContext.Current.Request.Url, url), out base64, out contentType))
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
    }
}