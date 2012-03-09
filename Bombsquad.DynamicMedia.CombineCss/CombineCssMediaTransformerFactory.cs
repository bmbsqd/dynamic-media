using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Contracts.Transformation;
using Bombsquad.DynamicMedia.Implementations;
using Bombsquad.DynamicMedia.Implementations.Transformation;

namespace Bombsquad.DynamicMedia.CombineCss
{
    public class CombineCssMediaTransformerFactory : TransformerFactoryTextBase
    {
        protected override bool IsValidFilePath(string absolutePath)
        {
            return absolutePath.Contains(".combine.");
        }

        protected override bool CanHandleFormat(IFormatInfo format)
        {
            return string.Equals(format.ContentType, "text/css", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override string ModifyAbsolutePath(string absolutePath)
        {
            return absolutePath.Replace(".combine", "");
        }

        private static readonly Regex CssImportRegex = new Regex(@"@import\s+url\s*\((?<StylesheetUrl>.*)\);", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        protected override MediaTransformResult TransformText(string text, out string transformedText)
        {
            var result = MediaTransformResult.Success;
            transformedText = CssImportRegex.Replace(text, delegate(Match m)
            {
                var stylesheetUrl = m.Groups["StylesheetUrl"].Value.Trim(new[] { '\'', '"' });
                string downloadedContent;
                if (TryGetStylesheetContent(new Uri(HttpContext.Current.Request.Url, stylesheetUrl), out downloadedContent))
                {
                    return downloadedContent;
                }

                result = MediaTransformResult.FailedWithFallback;
                return m.Value;
            });
            return result;
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
    }
}
