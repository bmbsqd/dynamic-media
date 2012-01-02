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

            if (transformMedia)
            {
                TransformMedia(response, original, outputFormat, request, mediaTransformer);
            }
            else
            {
                ServeOriginal(response, request, original, outputFormat);
            }

            original.Dispose();
        }

        private void ServeOriginal(HttpResponseWrapper response, HttpRequestWrapper request, Stream original,
                                   IFormatInfo outputFormat)
        {
            ServeResultStream(original, outputFormat, response);
            original.Seek(0, SeekOrigin.Begin);

            MediaCache.AddToCache(request, original, outputFormat);
        }

        private void TransformMedia(HttpResponseBase response, Stream original, IFormatInfo outputFormat,
                                    HttpRequestBase request, IMediaTransformer mediaTransformer)
        {
            Stream stream;
            var result = mediaTransformer.TransformStream(request, original, out stream);

            ServeResultStream(stream, outputFormat, response);

            if (result == MediaTransformResult.Success)
            {
                MediaCache.AddToCache(request, stream, outputFormat);
            }

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

        protected virtual void ServeResultStream(Stream stream, IFormatInfo outputFormat, HttpResponseBase response)
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