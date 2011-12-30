using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;

namespace Bombsquad.DynamicMedia
{
    public abstract class RequestHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var request = new HttpRequestWrapper(context.Request);
            var response = new HttpResponseWrapper(context.Response);

            var originalFormat = FormatInfoResolver.ResolveFromRequest(request);

            if (originalFormat == null)
            {
                ServeNotFoundResult(response);
                return;
            }

            IMediaTransformer mediaTransformer;
            var transformMedia = MediaTransformerFactory.TryCreateTransformer(request, originalFormat, out mediaTransformer);
            var outputFormat = transformMedia ? mediaTransformer.OutputFormat : originalFormat;

            if (MediaCache.TryServeRequestFromCache(request, response, outputFormat))
            {
                return;
            }

            Stream original;
            if (!StorageBackend.TryGetOriginalStream(request, mediaTransformer, out original))
            {
                ServeNotFoundResult(response);
                return;
            }

            if (!transformMedia)
            {
                ServeResultAndAddToCache(MediaCache, request, original, outputFormat, response);
                return;
            }

            var stream = mediaTransformer.TransformStream(request, original);
            original.Dispose();
            ServeResultAndAddToCache(MediaCache, request, stream, outputFormat, response);
        }

        private void ServeResultAndAddToCache(IMediaCache mediaCache, HttpRequestWrapper request, Stream stream, IFormatInfo outputFormat, HttpResponseWrapper response)
        {
            ServeResultStream(stream, outputFormat, response);
            stream.Seek(0, SeekOrigin.Begin);

            mediaCache.AddToCache(request, stream, outputFormat);
            stream.Dispose();
        }

        protected abstract IMediaCache MediaCache { get; }
        protected abstract IStorageBackend StorageBackend { get; }
        protected abstract IMediaTransformerFactory MediaTransformerFactory { get; }
        protected abstract IFormatInfoResolver FormatInfoResolver { get; }

        protected virtual void ServeNotFoundResult(HttpResponseBase response)
        {
            response.StatusCode = 404;
            response.Output.Write("Resource not found.");
        }

        protected virtual void ServeResultStream(Stream stream, IFormatInfo outputFormat, HttpResponseWrapper response)
        {
            response.ContentType = outputFormat.ContentType;
            stream.CopyTo(response.OutputStream);
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}