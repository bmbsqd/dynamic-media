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
        private readonly IResultHandler m_defaultResultHandler;

        protected DynamicMediaHandlerBase()
        {
            m_defaultResultHandler = new CompositeResultHandler(
                new CompressionResultHandler(),
                new SetCacheHeadersResultHandler(), 
                new NotModifiedResultHandler(),
                new SetContentTypeHeaderResultHandler(),
                new BytesRangeResultHandler(),
                new DefaultResultHandler());
        }

        public void ProcessRequest(HttpContext context)
        {
            switch (context.Request.HttpMethod.ToUpper())
            {
                case "GET":
                    HandleGetRequest(context);
                    return;

                default:
                    ServeInvalidMethod(context.Response);
                    return;
            }
        }

        private void HandleGetRequest(HttpContext context)
        {
            var request = new HttpRequestWrapper(context.Request);
            var response = new HttpResponseWrapper(context.Response);

            var originalFormat = FormatInfoProvider.ResolveFromExtension(Path.GetExtension(request.Url.AbsolutePath));

            if (originalFormat == null)
            {
                ServeNotFoundResult(response);
                return;
            }

            IMediaTransformer mediaTransformer;
            var transformMedia = MediaTransformerFactory.TryCreateTransformer(request, originalFormat, FormatInfoProvider,
                                                                              out mediaTransformer);
            var outputFormat = transformMedia ? mediaTransformer.OutputFormat : originalFormat;

            IResult result;
            if (TryGetResult(request, outputFormat, transformMedia, mediaTransformer, out result))
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
            return request.Url.PathAndQuery;
        }

        private static string GetOriginalPath(HttpRequestBase request, IMediaTransformer mediaTransformer)
        {
            var path = request.Url.AbsolutePath;

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
    	    var stream = storageFile.GetStream();

            IAddToCacheResult cacheResult;
    	    if( MediaCache.TryAddToCache( path, stream, outputFormat, out cacheResult ) )
            {
				return new CopyToOutputStreamResult( cacheResult.LastModified, cacheResult.ETag, storageFile.ContentLength, stream);
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
                if(MediaCache.TryAddToCache(path, stream, outputFormat, out cacheResult))
                {
                    return new CopyToOutputStreamResult(cacheResult.LastModified, cacheResult.ETag, stream.Length, stream);
                }
            }

            return new CopyToOutputStreamResult(null, null, stream.Length, stream);
        }

        protected abstract IMediaCache MediaCache { get; }

        protected abstract IStorageBackend StorageBackend { get; }

        protected abstract IMediaTransformerFactory MediaTransformerFactory { get; }

        protected abstract IFormatInfoProvider FormatInfoProvider { get; }

        protected virtual IResultHandler ResultHandler { get { return m_defaultResultHandler; } }

        protected virtual void ServeNotFoundResult(HttpResponseBase response)
        {
            response.StatusCode = 404;
            response.Output.Write("Resource not found.");
        }

        public virtual void ServeInvalidMethod(HttpResponse response)
        {
            response.StatusCode = 405;
            response.Output.Write("The page you are looking for cannot be displayed because an invalid method (HTTP verb) was used to attempt access.");
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}