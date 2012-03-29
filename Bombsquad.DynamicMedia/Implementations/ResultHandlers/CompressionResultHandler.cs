using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Implementations.Compression;

namespace Bombsquad.DynamicMedia.Implementations.ResultHandlers
{
    public class CompressionResultHandler : IResultHandler
    {
        private readonly List<ICompression> m_compressions;

        public CompressionResultHandler()
        {
            m_compressions = new List<ICompression>
            {
                new GzipCompression(),
                new DeflateCompression()
            };
        }

        public bool HandleResult(IResult result, IFormatInfo outputFormat, HttpRequestBase request, HttpResponseBase response)
        {
            response.Cache.VaryByHeaders["Accept-Encoding"] = true;

            if (!outputFormat.AllowCompression)
            {
                return false;
            }

            ICompression compression;
            if (!TryGetCompressionFromRequest(request, out compression))
            {
                return false;
            }

            response.Filter = compression.Compress(response.Filter);
            response.AddHeader("Content-Encoding", compression.ContentEncoding);
            return false;
        }

        public bool TryGetCompressionFromRequest(HttpRequestBase request, out ICompression compression)
        {
            var acceptEncodingHeader = request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(acceptEncodingHeader))
            {
                compression = null;
                return false;
            }

            compression = m_compressions.FirstOrDefault(c => acceptEncodingHeader.IndexOf(c.ContentEncoding, StringComparison.InvariantCultureIgnoreCase) > -1);
            return compression != null;
        }
    }
}