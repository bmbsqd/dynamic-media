using System;
using System.IO;
using System.Web;
using Bombsquad.DynamicMedia.Contracts;
using Bombsquad.DynamicMedia.Contracts.Cache;
using Bombsquad.DynamicMedia.Contracts.FormatInfo;
using Bombsquad.DynamicMedia.Contracts.Storage;
using Bombsquad.DynamicMedia.Contracts.Transformation;
using Bombsquad.DynamicMedia.Implementations.ResultHandlers;
using Bombsquad.DynamicMedia.Implementations.Results;

namespace Bombsquad.DynamicMedia
{
    public abstract class DynamicMediaHandlerBase : IHttpHandler
    {
        private IResultHandler _defaultResultHandler;

        protected DynamicMediaHandlerBase()
        {
            _defaultResultHandler = new CompositeResultHandler(
                new SetCacheHeadersResultHandler(), 
                new NotModifiedResultHandler(),
                new SetContentTypeHeaderResultHandler(),
                new BytesRangeResultHandler(),
                new DefaultResultHandler());
        }

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

            IResult result;
            if(TryGetResult(request, outputFormat, transformMedia, mediaTransformer, out result))
            {
                ResultHandler.HandleResult(result, outputFormat, request, response);
                result.Dispose();
            }
            else
            {
                ServeNotFoundResult(response);
            }
        }

        private bool TryGetResult(HttpRequestBase request, IFormatInfo outputFormat, bool transformMedia, IMediaTransformer mediaTransformer, out IResult result)
        {
            if (MediaCache.TryServeRequestFromCache(request, outputFormat, out result))
            {
                return true;
            }

            if (!transformMedia && !CacheOriginals)
            {
                return StorageBackend.TryServeOriginal(request, out result);
            }

            IOriginal original;
            if (!StorageBackend.TryGetOriginalStream(request, mediaTransformer, out original))
            {
                return false;
            }

            if (transformMedia)
            {
                result = TransformMedia(original.Stream, outputFormat, request, mediaTransformer);
                return true;
            }
            
            result = ServeOriginal(request, original, outputFormat);
            return true;
        }

    	protected abstract bool CacheOriginals { get; }

    	private IResult ServeOriginal(HttpRequestBase request, IOriginal original, IFormatInfo outputFormat)
        {
            original.Stream.Seek(0, SeekOrigin.Begin);

            IAddToCacheResult cacheResult;
            if (MediaCache.TryAddToCache(request, original.Stream, outputFormat, out cacheResult))
            {
                return new CopyToOutputStreamResult(cacheResult.LastModified, cacheResult.ETag, original.Stream.Length, original.Stream);
            }

            return new CopyToOutputStreamResult(original.LastModified, original.ETag, original.Stream.Length, original.Stream);
        }

        private IResult TransformMedia(Stream original, IFormatInfo outputFormat, HttpRequestBase request, IMediaTransformer mediaTransformer)
        {
            Stream stream;
            var transformResult = mediaTransformer.TransformStream(request, original, out stream);
            original.Dispose();

            if (transformResult == MediaTransformResult.Success)
            {
                IAddToCacheResult cacheResult;
                if(MediaCache.TryAddToCache(request, stream, outputFormat, out cacheResult))
                {
                    return new CopyToOutputStreamResult(cacheResult.LastModified, cacheResult.ETag, stream.Length, stream);
                }
            }

            return new CopyToOutputStreamResult(DateTime.Now, null, stream.Length, stream);
        }

        protected abstract IMediaCache MediaCache { get; }
        protected abstract IStorageBackend StorageBackend { get; }
        protected abstract IMediaTransformerFactory MediaTransformerFactory { get; }
        protected abstract IFormatInfoResolver FormatInfoResolver { get; }
        protected virtual IResultHandler ResultHandler { get { return _defaultResultHandler; } }

        protected virtual void ServeNotFoundResult(HttpResponseBase response)
        {
            response.StatusCode = 404;
            response.Output.Write("Resource not found.");
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}