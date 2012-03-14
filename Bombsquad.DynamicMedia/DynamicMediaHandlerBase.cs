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
        private readonly IResultHandler _defaultResultHandler;

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

        private static string GetRequestPath(HttpRequestBase request)
        {
            return request.Url.AbsolutePath;
        }

        private static string GetOriginalPath(HttpRequestBase request, IMediaTransformer mediaTransformer)
        {
            var path = GetRequestPath(request);

            if (mediaTransformer != null)
            {
                path = mediaTransformer.ModifyAbsolutePath(path);
            }

            return path;
        }

        private bool TryGetResult(HttpRequestBase request, IFormatInfo outputFormat, bool transformMedia, IMediaTransformer mediaTransformer, out IResult result)
        {
            var path = GetRequestPath(request) ;
            var originalPath = GetOriginalPath(request, mediaTransformer);

            if (MediaCache.TryServeRequestFromCache(transformMedia ? path : originalPath, outputFormat, out result))
            {
                return true;
            }

			IStorageFile storageFile;
			if(!StorageBackend.TryGetStorageFile(originalPath , out storageFile ))
			{
				return false;
			}

            if (!transformMedia && !CacheOriginals)
            {
            	result = storageFile;
            	return true;
            }
            
            if (transformMedia)
            {
                result = TransformMedia(storageFile.GetStream(), outputFormat, path, mediaTransformer);
                return true;
            }
            
            result = ServeOriginal(originalPath, storageFile, outputFormat);
            return true;
        }

        protected abstract bool CacheOriginals { get; }

    	private IResult ServeOriginal(string path, IStorageFile storageFile, IFormatInfo outputFormat)
    	{
            IAddToCacheResult cacheResult;
			if( MediaCache.TryAddToCache( path, storageFile.GetStream, outputFormat, out cacheResult ) )
            {
				return new CopyToOutputStreamResult( cacheResult.LastModified, cacheResult.ETag, storageFile.ContentLength, storageFile.GetStream());
            }

            return storageFile;
        }

        private IResult TransformMedia(Stream original, IFormatInfo outputFormat, string path, IMediaTransformer mediaTransformer)
        {
            Stream stream;
            var transformResult = mediaTransformer.TransformStream(original, out stream);
            original.Dispose();

            if (transformResult == MediaTransformResult.Success)
            {
                IAddToCacheResult cacheResult;
                if(MediaCache.TryAddToCache(path, () => stream, outputFormat, out cacheResult))
                {
                    return new CopyToOutputStreamResult(cacheResult.LastModified, cacheResult.ETag, stream.Length, stream);
                }
            }

            return new CopyToOutputStreamResult(null, null, stream.Length, stream);
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